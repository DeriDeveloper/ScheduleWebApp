using System;
using System.Collections.Generic;
using LibrarySchedule.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;
using static ScheduleWebApp.Types.Enums;
using static ScheduleWebApp.Types.ModelConfigJson;

namespace ScheduleWebApp.Pages
{
    public class ScheduleModel : PageModel
    {
        private readonly ILogger<ScheduleModel> _logger;

        public DateTime SelectedDate { get; set; }

        //public ScheduleWebApp.Types.User UserInfo { get; set; }

        public InformationSchedule[]? InformationsSchedule { get; private set; }

        public string UrlScheduleUserForProfile { get; set; }

        public LibrarySchedule.Models.CellSchedule[]? CellsSchedule { get; set; }

        public Dictionary<Types.Enums.DayOfWeekRusShort, List<int>> TimeOfChangeBetweenCouples { get; set; }

        public List<Types.DataAndDayOfWeek> DateAndDayOfWeeks { get; set; }
        public LibrarySchedule.Models.DateByNumeratorAndDenominator TypeWeek { get; set; }

        public LibrarySchedule.Models.Teacher Teacher { get; set; }

        public Types.Enums.TypePerson TypePerson { get; set; }
        public LibrarySchedule.Models.Group CurrentGroup { get; set; }
        public LibrarySchedule.Models.Teacher CurrentTeacher { get; set; }
        public LibrarySchedule.Models.Group[] Groups { get; set; }




        public InfoDefaultAccount InfoAccount { get; set; }


        public Types.Enums.TypeScheduleFrom TypeScheduleFrom { get; set; }


        public ScheduleModel(ILogger<ScheduleModel> logger)
        {
            _logger = logger;
        }



        public IActionResult OnGet(DateTime date, string teacher_id, string group_id, Types.Enums.TypePerson typePerson)
        {
            //Нужно для высчитывая времени от начало  до конца функции
            DateTime startTime = DateTime.Now;


            try
            {
                if (date == DateTime.MinValue)
                    date = DateTime.Now;

                if (date.DayOfWeek == DayOfWeek.Sunday)
					date= date.AddDays(1);

                SelectedDate = DateTime.Parse(date.ToString("dd.MM.yyyy"));
                


                InfoAccount = BackgroundWorker.UpdateDefaultDateAccountUser(HttpContext.Request.Cookies, HttpContext.Response.Cookies, ViewData);

                UrlScheduleUserForProfile = InfoAccount.UrlScheduleUserForProfile;

                

                InformationsSchedule = LibrarySchedule.Services.DateBase.Worker.GetInformationsSchedule(SelectedDate);


                DateAndDayOfWeeks = new List<DataAndDayOfWeek>();

                var dateMonday = LibrarySchedule.Services.BackgroundWorker.GetDateNextMonday(SelectedDate);
                
                for(int i =0; i < 6; i++)
                {
                    DateAndDayOfWeeks.Add(new DataAndDayOfWeek()
                    {
                        Date = dateMonday.AddDays(i),
                        DayOfWeekRusShort = Services.BackgroundWorker.GetDayOfWeekRusShort(dateMonday.AddDays(i))
                    });
                }

                TypeWeek = LibrarySchedule.Services.BackgroundWorker.GetDateCellScheduleType(SelectedDate);

                


            

                Dictionary<DayOfWeekRusShort, List<LibrarySchedule.Models.CellSchedule>> scheduleCells = null;

                if (group_id != null)
                {

                    Groups = LibrarySchedule.Services.DateBase.Worker.GetGroups();

                    if (!string.IsNullOrEmpty(group_id))
                    {
                        if (DeriLibrary.Check.IsNumber(group_id, DeriLibrary.Check.TypeNumbers.Int64))
                        {

                            int groupId = Convert.ToInt32(group_id);

                            CurrentGroup = LibrarySchedule.Services.DateBase.Worker.GetGroupById(groupId);




                            if (CurrentGroup != null)
                            {

                                TypePerson = TypePerson.student;


                                CellsSchedule = LibrarySchedule.Services.BackgroundWorker.GetCellsSchedule(groupId: groupId, date: SelectedDate);

								//DayOfWeeksCellsSchedule = LibrarySchedule.Services.BackgroundWorker.GetDayOfWeeksCellsScheduleGroup(DateTime.Now, CurrentGroup.Id);


								/*if (UserInfo != null)
                                {
                                    Services.WorkerDB.AddRecordQueryHistorySchedule(userId: UserInfo.Id, groupId: GroupId);
                                }
                                else
                                {
                                    Services.WorkerDB.AddRecordQueryHistorySchedule(groupId: GroupId);
                                }*/

								TypeScheduleFrom = TypeScheduleFrom.Student;


                            }
                            
                        }

                        DeriLibrary.Console.Worker.NotifyMessageCall($"Расписание для группы: {CurrentGroup.Name}, подготовлены за {DateTime.Now - startTime} сек.");

                    }
                    else
                    {
                        return Redirect(Url.Page("index"));
                    }
                }
                else if (teacher_id != null)
                {

                    if (DeriLibrary.Check.IsNumber(teacher_id, DeriLibrary.Check.TypeNumbers.Int64))
                    {


                        int teacherId = Convert.ToInt32(teacher_id);

                        if (teacherId > 0)
                        {

                            /*FullNameTeacher = WorkerDB.GetFullNameTeacherByID(TeacherID);

                            TypePerson = TypePerson.teacher;


                            scheduleCells = WorkerDB.GetScheduleTeachersCells(TeacherID, TypeCell);

                            WorkerDB.GetChangeScheduleCellsForTeacher(TeacherID, TypeCell, scheduleCells);




                            if (UserInfo != null)
                            {
                                Services.WorkerDB.AddRecordQueryHistorySchedule(userId: UserInfo.Id, teacherId: TeacherID);
                            }
                            else
                            {
                                Services.WorkerDB.AddRecordQueryHistorySchedule(teacherId: TeacherID);
                            }

                            TypeScheduleFrom = TypeScheduleFrom.Teacher;


                            DataStatus = true;


                            DeriLibrary.Console.Worker.NotifyMessageCall($"Расписание для преподавателя: {FullNameTeacher}, подготовлены за {DateTime.Now - startTime} сек.");
                            */

                        }
                        else return Redirect(Url.Page("index"));

                    }
                    else return Redirect(Url.Page("index"));
                }

                if (scheduleCells != null)
                {
                    //BackgroundWorker.FillTimesPairForSchedule(scheduleCells);
                }


                /*if (ScheduleDayOfWeeks != null)
                {
                    TimeOfChangeBetweenCouples = FillTimeOfChangeBetweenCouples(ScheduleDayOfWeeks);

                    Dictionary<long, List<GroupInfo>> idLocationCollegeForGroupsInfo = BackgroundWorker.GetIdLocationCollegeForGroupsInfo();

                    foreach (var keyValuePair in ScheduleDayOfWeeks)
                    {
                        DayOfWeekRusShort dayOfWeek = keyValuePair.Key;
                        List<CellSchedule> cellsSchedule = keyValuePair.Value;

                        if (dayOfWeek == Program.WorkerChangeScheduleSiteSmolApo.DayOfWeekChangeSchedule)
                        {
                            if (BackgroundWorker.ChangeTimesPair != null)
                            {
                                for (int i = 0; i < BackgroundWorker.ChangeTimesPair.Count; i++)
                                {
                                    foreach (var cellSchedule in cellsSchedule)
                                    {
                                        long idLocationCollege = 0;//BackgroundWorker.GetIdLocationCollegeByGroupId(idLocationCollegeForGroupsInfo, cellSchedule.GroupId);


                                        switch (BackgroundWorker.TypeChangesToCallScheduleForLocation)
                                        {
                                            case TypeChangesToCallScheduleForLocation.All:
                                                {
                                                    BackgroundWorker.FillCellScheduleNewTimesPair(cellSchedule, i);
                                                    break;
                                                }
                                            case TypeChangesToCallScheduleForLocation.Gagarino:
                                                {
                                                    if (idLocationCollege == (long)TypeChangesToCallScheduleForLocation.Gagarino)
                                                    {
                                                        BackgroundWorker.FillCellScheduleNewTimesPair(cellSchedule, i);
                                                    }
                                                    break;
                                                }
                                            case TypeChangesToCallScheduleForLocation.Shevchenko:
                                                {
                                                    if (idLocationCollege == (long)TypeChangesToCallScheduleForLocation.Shevchenko)
                                                    {
                                                        BackgroundWorker.FillCellScheduleNewTimesPair(cellSchedule, i);
                                                    }
                                                    break;
                                                }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }*/

                return Page();
            }
            catch (Exception error)
            {
                DeriLibrary.Console.Worker.NotifyErrorMessageCall(error.ToString());
                return Redirect(Url.Page("index"));

            }
        }



        private Dictionary<Types.Enums.DayOfWeekRusShort, List<int>> FillTimeOfChangeBetweenCouples(Dictionary<DayOfWeekRusShort, List<LibrarySchedule.Models.CellSchedule>> scheduleDayOfWeeks)
        {
            var timeOfChangeBetweenCouples = new Dictionary<DayOfWeekRusShort, List<int>>()
            {
                { DayOfWeekRusShort.ПН, new List<int>() },
                { DayOfWeekRusShort.ВТ, new List<int>() },
                { DayOfWeekRusShort.СР, new List<int>() },
                { DayOfWeekRusShort.ЧТ, new List<int>() },
                { DayOfWeekRusShort.ПТ, new List<int>() },
                { DayOfWeekRusShort.СБ, new List<int>() }
            };

            string dateTimeNow = DateTime.Now.ToString("dd.MM.yyyy");

            foreach (KeyValuePair<DayOfWeekRusShort, List<LibrarySchedule.Models.CellSchedule>> keyValuePair in scheduleDayOfWeeks)
            {

                var key = keyValuePair.Key;
                var value = keyValuePair.Value;



                for (int i = 0; i < value.Count; i++)
                {
                    LibrarySchedule.Models.CellSchedule cellSchedule = value[i];
                    LibrarySchedule.Models.CellSchedule cellScheduleChange = value[i].ChangeCellSchedule;
                    LibrarySchedule.Models.CellSchedule nextCellSchedule;
                    LibrarySchedule.Models.CellSchedule nextCellScheduleChange;

                    try
                    {

                        if (i + 1 < value.Count)
                        {
                            nextCellSchedule = value[i + 1];
                            nextCellScheduleChange = nextCellSchedule.ChangeCellSchedule;


                            var cell = cellScheduleChange == null ? cellSchedule : cellScheduleChange;
                            var nextCell = nextCellScheduleChange == null ? nextCellSchedule : nextCellScheduleChange;


                            var minutes = (nextCell.TimesPair.TimeStart - cell.TimesPair.TimeEnd).TotalMinutes;



                            timeOfChangeBetweenCouples[key].Add(Convert.ToInt32(minutes));
                        }
                    }
                    catch (Exception error)
                    {
                        DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
                    }

                }
            }




            return timeOfChangeBetweenCouples;
        }

        private int GetStartDayInDayOfWeek()
        {
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday: return 0;
                case DayOfWeek.Tuesday: return -1;
                case DayOfWeek.Wednesday: return -2;
                case DayOfWeek.Thursday: return -3;
                case DayOfWeek.Friday: return -4;
                case DayOfWeek.Saturday: return -5;
                case DayOfWeek.Sunday: return 0;
                default: return 0;
            }
        }

        private Enums.DayOfWeekRusShort GetCurrentDayOfWeekRus()
        {
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday: return Enums.DayOfWeekRusShort.ПН;
                case DayOfWeek.Tuesday: return Enums.DayOfWeekRusShort.ВТ;
                case DayOfWeek.Wednesday: return Enums.DayOfWeekRusShort.СР;
                case DayOfWeek.Thursday: return Enums.DayOfWeekRusShort.ЧТ;
                case DayOfWeek.Friday: return Enums.DayOfWeekRusShort.ПТ;
                case DayOfWeek.Saturday: return Enums.DayOfWeekRusShort.СБ;
                case DayOfWeek.Sunday: return Enums.DayOfWeekRusShort.ПН;
                default: return Enums.DayOfWeekRusShort.ПН;
            }
        }
    }
}
