using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter.Xml;
using ConsoleTables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using OfficeOpenXml.ConditionalFormatting;
using ScheduleWebApp.Types;
using Xceed.Document.NET;
using static ScheduleWebApp.Types.Enums;

namespace ScheduleWebApp.Services
{
    public class BackgroundWorker
    {
        public static List<Types.TimesPair> ChangeTimesPair { get; set; }
        public static TypeChangesToCallScheduleForLocation TypeChangesToCallScheduleForLocation { get; set; }
        public static DateTime DateTimeChangeSchedule { get; set; }
        public static int DayOfWeekIntChangeSchedule { get; set; }

        public static Dictionary<DayOfWeekRusShort, List<Types.TimesPair>> DayOfWeekForPairDateTime { get; set; }

        public static bool ClearingAnOutdatedScheduleChangeStatus { get; set; } = false;
        public static bool IsDateIncludedInRangeOfDateList(DateTime dateTime, ListDateRange listDateRange)
        {
            if (listDateRange?.dates != null)
            {
                foreach (ListDateRange.Date date in listDateRange.dates)
                {
                    if (date.dateTimeStart <= dateTime && date.dateTimeEnd >= dateTime)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static DayOfWeek ConvertToDayOfWeek(DayOfWeekRusShort dayOfWeekRusShort)
        {
            switch (dayOfWeekRusShort)
            {
                case DayOfWeekRusShort.ПН: return DayOfWeek.Monday;
                case DayOfWeekRusShort.ВТ: return DayOfWeek.Tuesday;
                case DayOfWeekRusShort.СР: return DayOfWeek.Wednesday;
                case DayOfWeekRusShort.ЧТ: return DayOfWeek.Thursday;
                case DayOfWeekRusShort.ПТ: return DayOfWeek.Friday;
                case DayOfWeekRusShort.СБ: return DayOfWeek.Saturday;
                case DayOfWeekRusShort.ВС: return DayOfWeek.Saturday;
                default: return DayOfWeek.Monday;
            }
        }
        public static bool IsTimeIncludedInTimeRange(DateTime dateTimeNow, DateTime timeStart, DateTime timeEnd)
        {
            timeStart = DateTime.Parse(DateTime.Now.ToString("dd.MM.yyyy") + " " + timeStart.ToString("HH:mm:ss"));
            timeEnd = DateTime.Parse(DateTime.Now.ToString("dd.MM.yyyy") + " " + timeEnd.ToString("HH:mm:ss"));

            if (timeStart <= dateTimeNow && dateTimeNow < timeEnd)
                return true;
            else return false;

        }

        internal static void FillCellScheduleNewTimesPair(CellSchedule cellSchedule, int index)
        {
            var timesPair = BackgroundWorker.ChangeTimesPair[index];
            if (timesPair.NumberPair == cellSchedule.NumberPair)
            {
                cellSchedule.TimesPair.TimeStart = timesPair.TimeStart;
                cellSchedule.TimesPair.TimeEnd = timesPair.TimeEnd;
                cellSchedule.TimesPair.IsChange = true;
            }
        }

        



        


        
        

        internal static string RemoveAllCharactersFromList(string text, List<string> words)
        {
            foreach (var word in words)
            {
                if (text.Contains(word))
                {
                    text.Replace(word, "");
                }
            }


            return text;
        }

        internal static bool GetValidAbbreviatedName(string name)
        {
            if (GetAbbreviatedNameByFullName(name) != null)
                return true;

            return false;
        }


        internal static string GetAbbreviatedNameByFullName(string fullName)
        {

            if (fullName != null)
            {
                string[] temp = fullName.Split(" ");

                if (temp.Length == 3)
                {
                    var charsSurname = temp[0].ToLower().ToArray();

                    if (temp[0].Contains("-"))
                    {
                        StringBuilder stringBuilder = new StringBuilder();


                        var tempSurnamePartArray = temp[0].Split("-");

                        for (int i = 0; i < tempSurnamePartArray.Length; i++)
                        {
                            var tempSurnameArray = tempSurnamePartArray[i].ToArray();
                            tempSurnameArray[0] = char.ToUpper(tempSurnameArray[0]);


                            stringBuilder.Append(new string(tempSurnameArray));

                            if (i < tempSurnamePartArray.Length - 1)
                            {
                                stringBuilder.Append("-");
                            }
                        }

                        temp[0] = stringBuilder.ToString();

                    }
                    else
                    {
                        var tempSurnameArray = temp[0].ToArray();
                        tempSurnameArray[0] = char.ToUpper(tempSurnameArray[0]);

                        temp[0] = new string(tempSurnameArray);
                    }

                    //charsSurname[0] = char.ToUpper(charsSurname[0]);
                    //var surname = new string(charsSurname);

                    string abbreviatedName = $"{temp[0]} {temp[1][0].ToString().ToUpper()}. {temp[2][0].ToString().ToUpper()}.";

                    return abbreviatedName;
                }
            }

            return null;

        }

        public static string GetTitleDayOfWeek(DateTime dateTime, bool isFull)
        {
            if (isFull)
            {
                switch (dateTime.DayOfWeek)
                {
                    case DayOfWeek.Monday: return "Понедельник";
                    case DayOfWeek.Tuesday: return "Вторник";
                    case DayOfWeek.Wednesday: return "Среда";
                    case DayOfWeek.Thursday: return "Четверг";
                    case DayOfWeek.Friday: return "Пятница";
                    case DayOfWeek.Saturday: return "Суббота";
                    case DayOfWeek.Sunday: return "Воскресенье";
                    default: return "Нет данных";
                }
            }
            else
            {
                switch (dateTime.DayOfWeek)
                {
                    case DayOfWeek.Monday: return "Пн";
                    case DayOfWeek.Tuesday: return "Вт";
                    case DayOfWeek.Wednesday: return "Ср";
                    case DayOfWeek.Thursday: return "Чт";
                    case DayOfWeek.Friday: return "Пт";
                    case DayOfWeek.Saturday: return "Сб";
                    case DayOfWeek.Sunday: return "Вс";
                    default: return "Нет данных";
                }
            }
        }
        internal static ScheduleWebApp.Types.Enums.DayOfWeekRusShort GetDayOfWeekRusShort(DateTime dateTime)
        {
            switch (dateTime.DayOfWeek)
            {
                case DayOfWeek.Monday: return Enums.DayOfWeekRusShort.ПН;
                case DayOfWeek.Tuesday: return Enums.DayOfWeekRusShort.ВТ;
                case DayOfWeek.Wednesday: return Enums.DayOfWeekRusShort.СР;
                case DayOfWeek.Thursday: return Enums.DayOfWeekRusShort.ЧТ;
                case DayOfWeek.Friday: return Enums.DayOfWeekRusShort.ПТ;
                case DayOfWeek.Saturday: return Enums.DayOfWeekRusShort.СБ;
                case DayOfWeek.Sunday: return Enums.DayOfWeekRusShort.ВС;
                default: return Enums.DayOfWeekRusShort.ПН;
            }
        }
       
    

        internal static long GetUserIdByUserIdString(string userIdString)
        {
            if (userIdString != null)
            {
                if (DeriLibrary.Check.IsNumber(userIdString, DeriLibrary.Check.TypeNumbers.Int32))
                {
                    long userId = Convert.ToInt64(userIdString);
                    return userId;
                }
            }

            return 0;

        }
        internal static string GetUrlRedirectSchedule(int userId)
        {
            var user = LibrarySchedule.Services.DateBase.Worker.GetUser(userId);
            var typePerson = user.TypePerson;

			switch (typePerson)
            {
                case  LibrarySchedule.Types.Enums.TypePerson.Student:
                    {
                        var student = LibrarySchedule.Services.DateBase.Worker.GetStudentById(userId);
                        //Student student = WorkerDB.GetStudentById(userId);
                        return $"/Schedule?group_id={student.Group.Id}";

                    }
                case LibrarySchedule.Types.Enums.TypePerson.Teacher:
                    {
                        //Teacher teacher = WorkerDB.GetTeacherByUserID(userId);

                        return "";
                        //return $"/Schedule?teacher_id={teacher.Id}";
                    }
                default:
                    {
                        return $"/SelectSchedule";

                    }
            }


        }

        internal static Types.InfoDefaultAccount UpdateDefaultDateAccountUser(IRequestCookieCollection requestCookies, IResponseCookies responseCookies, ViewDataDictionary viewData)
        {
            Types.InfoDefaultAccount infoAccount = new Types.InfoDefaultAccount();




            infoAccount.User = LibrarySchedule.Services.BackgroundWorker.CheckUserForAuthorization(requestCookies["user_id"]);

            if (infoAccount.User != null)
            {
                infoAccount.UrlScheduleUserForProfile = BackgroundWorker.UrlScheduleUserForProfile(infoAccount.User.Id);


                if (infoAccount.User.TypePerson ==  LibrarySchedule.Types.Enums.TypePerson.Student)
                {
                    //infoAccount.User.GroupInfo = WorkerDB.GetGroupInfoForUser(infoAccount.User.Id);
                }
                else if (infoAccount.User.TypePerson ==  LibrarySchedule.Types.Enums.TypePerson.Teacher)
                {
                    //infoAccount.User.GroupInfo = WorkerDB.GetTeacherForUser(infoAccount.User.Id);
                }


                infoAccount.UserSettings = BackgroundWorker.GetUserSettings(infoAccount.User.Id);

                BackgroundWorker.FillViewDataThemeName(viewData, infoAccount.UserSettings.ThemeApp);

                BackgroundWorker.UpdateCookieDefault(responseCookies, userId: infoAccount.User.Id);
            }



            return infoAccount;

        }

        

        internal static List<long> ConvertStringIdToLongId(List<string> teachersPair)
        {
            List<long> teachersId = new List<long>();

            foreach (string teacher in teachersPair)
            {
                try
                {
                    if (DeriLibrary.Check.IsNumber(teacher, DeriLibrary.Check.TypeNumbers.Int64))
                        teachersId.Add(Convert.ToInt64(teacher));
                }
                catch (Exception error)
                {
                    DeriLibrary.Console.Worker.NotifyErrorMessageCall(error.ToString());
                }
            }

            return teachersId;
        }

        

        internal static List<DateTime> GetDateTimeFromDocxChangeSchedule(ReadOnlyCollection<Paragraph> allParagraphs)
        {

            List<DateTime> dateTimes = new List<DateTime>();

            var currentYear = DateTime.Now.Year;

            foreach (var paragraph in allParagraphs)
            {

                string text = paragraph.Text;
                if (text.Contains(",") && text.Contains(".") && text.Contains("(") && text.Contains(")"))
                {
                    try
                    {
                        dateTimes.Add(DateTime.Parse($"{text.Replace(" ", "").Split("(")[0].Trim()}.{currentYear}"));
                    }
                    catch (Exception error)
                    {
                        continue;
                    }
                }
            }

            return dateTimes;
        }

        internal static bool CheckDateAndDayOfWeekAndTypeCell(string text)
        {
            if (text.Contains(",") && text.Contains(".") && text.Contains("(") && text.Contains(")"))
            {
                return true;
            }

            return false;
        }

        internal static ChangeScheduleDateInfo GetChangeScheduleDateInfo(string text)
        {
            if (text == null)
                return null;

            text = text.ToLower().Trim();

            if (string.Empty == text)
                return null;



            var tempParts = text.Split(" ");

            if (tempParts.Length != 4 && tempParts.Length > 0)
                return null;

            if (DeriLibrary.Check.IsNumber(tempParts[0], DeriLibrary.Check.TypeNumbers.Int32))
            {
                ChangeScheduleDateInfo dateInfo = new ChangeScheduleDateInfo();
                dateInfo.NumberDay = Convert.ToInt32(tempParts[0]);

                tempParts[3] = tempParts[3].Trim().ToLower().Replace(")", "").Replace(",", "").Replace(" ", "");

                if (tempParts[3].Equals("числитель"))
                {
                    dateInfo.CellScheduleType = CellScheduleType.numerator;
                }
                else if (tempParts[3].Equals("знаменатель"))
                {
                    dateInfo.CellScheduleType = CellScheduleType.denominator;
                }

                var tempMonthRus = tempParts[1];

                dateInfo.MonthRus = tempMonthRus;

                if (tempMonthRus.Equals("января"))
                {
                    dateInfo.TypeMonth = TypeMonth.January;
                }
                else if (tempMonthRus.Equals("февраля"))
                {
                    dateInfo.TypeMonth = TypeMonth.February;

                }
                else if (tempMonthRus.Equals("марта"))
                {
                    dateInfo.TypeMonth = TypeMonth.March;

                }
                else if (tempMonthRus.Equals("апреля"))
                {
                    dateInfo.TypeMonth = TypeMonth.April;

                }
                else if (tempMonthRus.Equals("мая"))
                {
                    dateInfo.TypeMonth = TypeMonth.May;

                }
                else if (tempMonthRus.Equals("июня"))
                {
                    dateInfo.TypeMonth = TypeMonth.June;

                }
                else if (tempMonthRus.Equals("июля"))
                {
                    dateInfo.TypeMonth = TypeMonth.July;

                }
                else if (tempMonthRus.Equals("августа"))
                {
                    dateInfo.TypeMonth = TypeMonth.August;

                }
                else if (tempMonthRus.Equals("сентября"))
                {
                    dateInfo.TypeMonth = TypeMonth.September;

                }
                else if (tempMonthRus.Equals("октября"))
                {
                    dateInfo.TypeMonth = TypeMonth.October;

                }
                else if (tempMonthRus.Equals("ноября"))
                {
                    dateInfo.TypeMonth = TypeMonth.November;

                }
                else if (tempMonthRus.Equals("декабря"))
                {
                    dateInfo.TypeMonth = TypeMonth.December;

                }
                else
                {
                    return null;
                }

                var stringDay = (dateInfo.NumberDay).ToString();
                if (stringDay.Length == 1) { stringDay = "0" + stringDay; }

                var stringMonth = ((int)dateInfo.TypeMonth).ToString();
                if (stringMonth.Length == 1) { stringMonth = "0" + stringMonth; }

                dateInfo.DateTime = DateTime.Parse($"{stringDay}.{stringMonth}.{DateTime.Now.Year}");

                return dateInfo;

            }


            return null;
        }



       
        internal static LibrarySchedule.Models.Teacher CheckExistTeacherNamePairInAllTeacher(string teacherSearch, LibrarySchedule.Models.Teacher[] teachers)
        {


            foreach (var teacher in teachers)
            {
                if (teacherSearch.ToLower().Contains(teacher.GetNameInitials().ToLower())) return teacher;
                //else if (teacher.AbbreviatedName.Equals(teacherSearch)) return teacher;
                //else if (teacherSearch.Contains(teacher.AbbreviatedName)) return teacher;
                //else if (teacher.FullName.Equals(teacherSearch)) return teacher;
                //else if (teacherSearch.Contains(teacher.FullName)) return teacher;
            }


            return null;
        }

        internal static bool AreThereAnyGivenWordsInText(string text, List<string> words)
        {

            words = words.ConvertAll(d => d.ToLower());

            foreach (string word in words)
            {

                if (text.Contains(word))
                    return true;

            }

            return false;
        }

        internal static bool AreThereAnyGivenWordsInText(List<string> texts, List<string> words)
        {

            texts = texts.ConvertAll(d => d.ToLower());
            words = words.ConvertAll(d => d.ToLower());

            foreach (string text in texts)
            {
                foreach (string word in words)
                {

                    if (text.Contains(word))
                        return true;

                }
            }

            return false;
        }

        internal static string CheckExistNamePairInAllNamePairs(string namePairSearch, List<string> namePairs)
        {

            if (namePairs.Contains(namePairSearch)) return namePairSearch;

            return null;
        }

        


       

        internal static void ExtractFullNameOfTeacherFromText(List<LibrarySchedule.Models.Teacher> teachers1, string text, LibrarySchedule.Models.Teacher[] teachers2)
        {

            foreach (var teacher in teachers2)
            {
                if (text.ToLower().Contains(teacher.GetNameInitials().ToLower()))
                {
                    teachers1.Add(teacher);
                }
            }
        }
        internal static void ExtractFullNameOfTeacherFromText(List<LibrarySchedule.Models.Teacher> teachers1, string text, List<LibrarySchedule.Models.Teacher> teachers)
        {

            foreach (var teacher in teachers)
            {
                if (text.ToLower().Contains(teacher.GetNameInitials().ToLower()))
                {
                    teachers1.Add(teacher);
                }
            }
        }
        internal static string RemoveFullNameOfTeachersFromText(string text, LibrarySchedule.Models.Teacher[] teachers)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    text = text.ToLower();

                    foreach (var teacher in teachers)
                    {
                        var abbreviatedName = teacher.GetNameInitials().ToLower();


                        if (text.Contains(abbreviatedName))
                            text = text.Replace(abbreviatedName, "");
                    }

                    text = text.Trim();


                    if (text[text.Length - 1].Equals('+'))
                        text = text.Substring(0, text.Length - 1).Trim();

                }
            }
            catch (Exception error)
            {
                DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
            }
            return text;
        }

        internal static bool IsThereSuchAnIdInIdsList(long id, List<string> rows)
        {
            bool result = false;

            foreach (string selectId in rows)
            {
                if (DeriLibrary.Check.IsNumber(selectId, DeriLibrary.Check.TypeNumbers.Int64))
                {
                    if (Convert.ToInt64(selectId) == id)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;

        }

        internal static bool IsThereSuchAnIdInIdsList(long id, List<Teacher> Teachers)
        {
            bool result = false;

            foreach (var Teacher in Teachers)
            {
                if (Teacher.Id == id)
                {
                    result = true;
                    break;
                }
            }

            return result;

        }


        internal static bool CheckExistGroup(string nameGroup, List<GroupInfo> groupInfos)
        {
            foreach (var groupInfo in groupInfos)
            {
                if (groupInfo.Name.ToLower().Trim().Equals(nameGroup))
                    return true;
            }


            return false;
        }

        internal static string UrlScheduleUserForProfile(int userId)
        {
            var user = LibrarySchedule.Services.DateBase.Worker.GetUser(userId);
            
            switch (user.TypePerson)
            {
                case  LibrarySchedule.Types.Enums.TypePerson.Student:
                    {
                        LibrarySchedule.Models.StudentAccount student = LibrarySchedule.Services.DateBase.Worker.GetStudentById(userId);

                        if (student != null)
                        {
                            return $"/Schedule?group_id={student.Group.Id}";
                        }
                        else return "/";
                        
                    }
                case  LibrarySchedule.Types.Enums.TypePerson.Teacher:
                    {
                        //добавить как выше у группы  
                        //Teacher teacher = WorkerDB.GetTeacherByUserID(userId);

                        return "";
                        //return $"/Schedule?teacher_id={teacher.Id}";
                    }



            }

            return null;
        }

        internal static Dictionary<DayOfWeekRusShort, List<CellSchedule>> SortScheduleCells(Dictionary<DayOfWeekRusShort, List<CellSchedule>> scheduleCell)
        {
            Dictionary<DayOfWeekRusShort, List<CellSchedule>> scheduleCellNew = new Dictionary<DayOfWeekRusShort, List<CellSchedule>>()
            {
                { DayOfWeekRusShort.ПН, new List<CellSchedule>() },
                { DayOfWeekRusShort.ВТ, new List<CellSchedule>() },
                { DayOfWeekRusShort.СР, new List<CellSchedule>() },
                { DayOfWeekRusShort.ЧТ, new List<CellSchedule>() },
                { DayOfWeekRusShort.ПТ, new List<CellSchedule>() },
                { DayOfWeekRusShort.СБ, new List<CellSchedule>() }
            };



            foreach (var keyValuePair in scheduleCell)
            {
                var dayOfWeek = keyValuePair.Key;
                var cells = keyValuePair.Value;


                int minNumPair = int.MaxValue;
                int maxNumPair = 0;

                if (cells.Count > 0)
                {
                    minNumPair = cells[0].NumberPair;



                    List<int> numberPairOrder = new List<int>();
                    List<int> numberPairOrderSort = new List<int>();


                    foreach (var cell in cells)
                    {

                        numberPairOrder.Add(cell.NumberPair);
                        numberPairOrderSort.Add(cell.NumberPair);




                        if (cell.NumberPair > maxNumPair)
                        {
                            maxNumPair = cell.NumberPair;
                        }

                        if (cell.NumberPair < minNumPair)
                        {
                            minNumPair = cell.NumberPair;
                        }

                    }


                    numberPairOrderSort.Sort();



                    if (!numberPairOrderSort.SequenceEqual(numberPairOrder))
                    {
                        for (int i = minNumPair; i < maxNumPair + 1; i++)
                        {
                            foreach (var cell in cells)
                            {
                                if (cell.NumberPair == i)
                                {
                                    scheduleCellNew[dayOfWeek].Add(cell);
                                }
                            }
                        }
                    }
                    else
                    {
                        scheduleCellNew[dayOfWeek] = scheduleCell[dayOfWeek];
                    }
                }
            }



            return scheduleCellNew;
        }

        internal static string RemoveListOfWordsFromText(string text, List<string> wordsRemove)
        {
            text = text.Trim();

            foreach (var word in wordsRemove)
            {
                if (text.Contains(word))
                {
                    text = text.Replace(word, "").Trim();
                }
            }

            return text;
        }




        internal static Dictionary<DayOfWeekRusShort, List<CellSchedule>> RemoveIdenticalCells(Dictionary<DayOfWeekRusShort, List<CellSchedule>> scheduleCell)
        {
            foreach (var keyValuePair in scheduleCell)
            {
                var dayOfWeek = keyValuePair.Key;
                var cells = keyValuePair.Value;

                bool areThereIdenticalCellsByPairNumber = false;

                List<int> numberPair = new List<int>();
                List<CellSchedule> changeCellSchedulesTemp = new List<CellSchedule>();


                foreach (var cell in cells)
                {

                    if (cell.IsChange)
                    {
                        changeCellSchedulesTemp.Add(cell);
                    }



                    int numPair = cell.NumberPair;



                    if (numberPair.Contains(numPair))
                    {
                        areThereIdenticalCellsByPairNumber = true;
                        break;
                    }
                    else
                    {
                        numberPair.Add(numPair);
                    }




                }



                if (areThereIdenticalCellsByPairNumber)
                {
                    foreach (var cellChangeSelect in changeCellSchedulesTemp)
                    {
                        for (int i = 0; i < cells.Count; i++)
                        {
                            var cellSelectTemp = cells[i];



                            if (cellChangeSelect.Id != cellSelectTemp.Id)
                            {
                                if (cellChangeSelect.NumberPair == cellSelectTemp.NumberPair)
                                {
                                    cells.RemoveAt(i);
                                }
                            }
                        }
                    }
                }

            }

            return scheduleCell;
        }

        internal static int IndexNumPairOf(List<CellSchedule> cellsScheduleForDayOfWeek, int numberPair)
        {
            int index = -1;

            for (int i = 0; i < cellsScheduleForDayOfWeek.Count; i++)
            {
                if (cellsScheduleForDayOfWeek[i].NumberPair == numberPair)
                {
                    index = i;
                    break;
                }
            }


            return index;
        }

        /*internal static void FillCellScheduleInfoTeachersById(CellSchedule cellSchedule, string teacherPair)
        {
            foreach (string teacherIdString in teacherPair.Split(",").ToList())
            {
                if (DeriLibrary.Check.IsNumber(teacherIdString, DeriLibrary.Check.TypeNumbers.Int64))
                {
                    var Teacher = new Teacher()
                    {
                        Id = Convert.ToInt64(teacherIdString)
                    };
                    Teacher.FullName = WorkerDB.GetFullNameTeacherByID(Teacher.Id);
                    Teacher.AbbreviatedName = BackgroundWorker.GetAbbreviatedNameByFullName(Teacher.FullName);

                    cellSchedule.TeachersPair.Add(Teacher);
                }
            }
        }*/


       

        

       

        

        internal static bool CheckExistTeacherIdInArray(long teacherId, string[] teachersPair)
        {
            foreach (var selectTeacher in teachersPair)
            {
                try
                {
                    if (teacherId == Convert.ToInt64(selectTeacher.Trim()))
                    {
                        return true;
                    }
                }
                catch (Exception error)
                {
                    DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
                }
            }

            return false;
        }

        internal static bool AreTeachersInCellSame(CellSchedule cellSchedule, CellSchedule selectCellSchedule)
        {

            if (cellSchedule.TeachersPair.Count == selectCellSchedule.TeachersPair.Count && cellSchedule.TeachersPair.Count > 0)
            {


                for (int i = 0; i < selectCellSchedule.TeachersPair.Count; i++)
                {
                    if (selectCellSchedule.TeachersPair[i].Id != cellSchedule.TeachersPair[i].Id)
                        return false;
                }

                return true;

            }
            else
            {
                return false;
            }
        }

        internal static bool IsNameOfAPairOfCellsSame(CellSchedule cellSchedule, CellSchedule selectCellSchedule)
        {

            if (selectCellSchedule.NamesPair.Count == cellSchedule.NamesPair.Count && cellSchedule.NamesPair.Count > 0)
            {
                for (int i = 0; i < selectCellSchedule.NamesPair.Count; i++)
                {
                    var oneNamePair = BackgroundWorker.RemoveAllCharactersFromList(selectCellSchedule.NamesPair[i].ToLower(), Config.RemoveWords).Trim();
                    var twoNamePair = BackgroundWorker.RemoveAllCharactersFromList(cellSchedule.NamesPair[i].ToLower(), Config.RemoveWords).Trim();

                    if (!oneNamePair.Equals(twoNamePair))
                        return false;
                }

                return true;
            }
            else
            {
                return false;
            }

        }



        internal static bool IsNameOfPairAndTeacherIncludedInCell(CellSchedule cellSchedule, CellSchedule selectCellSchedule)
        {
            foreach (var namePair in cellSchedule.NamesPair)
            {
                foreach (var selectNamePair in selectCellSchedule.NamesPair)
                {
                    var oneNamePair = BackgroundWorker.RemoveAllCharactersFromList(namePair.ToLower(), Config.RemoveWords).Trim();
                    var twoNamePair = BackgroundWorker.RemoveAllCharactersFromList(selectNamePair.ToLower(), Config.RemoveWords).Trim();

                    if (oneNamePair.Equals(twoNamePair))
                    {
                        foreach (var Teacher in cellSchedule.TeachersPair)
                        {
                            foreach (var selectTeacher in selectCellSchedule.TeachersPair)
                            {
                                if (Teacher.Id == selectTeacher.Id)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

       



        internal static int GetIndexTableChangeTimes(List<Table> tables)
        {
            for (int i = 0; i < tables.Count; i++)
            {
                var currentTable = tables[i];

                if (currentTable.Rows[0].Cells.Count == 4)
                {
                    if (currentTable.Rows[0].Cells[0].Paragraphs[0].Text.Trim().ToLower().Contains("пара"))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }



        internal static void UpdateCookieDefault(IResponseCookies cookies, long userId = 0)
        {
            if (cookies != null)
            {

                if (userId > 0)
                {
                    cookies.Append("user_id", userId.ToString(), new Microsoft.AspNetCore.Http.CookieOptions()
                    {
                        Expires = DateTimeOffset.Now.AddMonths(1),
                        IsEssential = true
                    });
                }
            }
        }

        

       

        

        internal static void FillTimesPairForSchedule(Dictionary<DayOfWeekRusShort, List<CellSchedule>> dayOfWeeksScheduleCells)
        {
            foreach (var dayOfWeekScheduleCells in dayOfWeeksScheduleCells)
            {
                DayOfWeekRusShort dayOfWeek = dayOfWeekScheduleCells.Key;
                var cellsSchedule = dayOfWeekScheduleCells.Value;

                foreach (var cellSchedule in cellsSchedule)
                {
                    int numberPair = cellSchedule.NumberPair;

                    cellSchedule.TimesPair.NumberPair = numberPair;


                    FillTimesPairForCellSchedule(dayOfWeek, numberPair, cellSchedule);


                    var changeCellSchedule = cellSchedule.ChangeCellSchedule;

                    if (changeCellSchedule != null)
                    {
                        FillTimesPairForCellSchedule(dayOfWeek, numberPair, changeCellSchedule);
                    }

                }
            }
        }

        private static void FillTimesPairForCellSchedule(DayOfWeekRusShort dayOfWeek, int numberPair, CellSchedule cellSchedule)
        {
            var timesPair = BackgroundWorker.DayOfWeekForPairDateTime[dayOfWeek][numberPair - 1];

            cellSchedule.TimesPair.TimeStart = timesPair.TimeStart;
            cellSchedule.TimesPair.TimeEnd = timesPair.TimeEnd;
        }

        internal static bool CheckAccess(IRequestCookieCollection cookies, TypePrivilege typePrivilege)
        {
            string userIdString = cookies["user_id"];

            if (userIdString != null)
            {

                int userId = Convert.ToInt32(userIdString);

                if (userId > 0)
                {
                    var user = LibrarySchedule.Services.DateBase.Worker.GetUser(userId);

                    switch (user.TypePerson)
                    {
                        case LibrarySchedule.Types.Enums.TypePerson.Student:
                            {
                                /*var studentInfo = WorkerDB.GetStudentById(userId);

                                if (studentInfo.TypePrivilege >= typePrivilege)
                                {
                                    return true;
                                }*/

                                break;
                            }
                    }
                }
            }

            return false;
        }

        internal void SortGroupInfosForReport(List<Types.ModelGroupInfoForReport> groupInfosForReport)
        {

        }

       

        internal static GroupInfo GetGroupInfo(string nameGroup, List<GroupInfo> groupInfos)
        {
            GroupInfo groupInfo = null;

            foreach (var selectGroupInfo in groupInfos)
            {
                if (selectGroupInfo.Name.ToLower().Equals(nameGroup))
                {
                    groupInfo = selectGroupInfo;
                    break;
                }
            }


            return groupInfo;
        }



        internal static bool CheckValidNameGroup(string nameGroup)
        {
            if (!nameGroup.Contains("-"))
                return false;

            var nameGroupSplit = nameGroup.Split("-");

            if (nameGroupSplit.Length != 2)
                return false;

            if (!DeriLibrary.Check.IsNumber(nameGroupSplit[0], DeriLibrary.Check.TypeNumbers.Int32))
                return false;

            return true;

        }

        internal static CellCoordinates.Coord GetIntersectionСoordinates(CellCoordinates.Coord startCoord1, CellCoordinates.Coord startCoord2)
        {
            CellCoordinates.Coord coord = null;
            if (startCoord1 == null || startCoord2 == null)
            {
                coord = null;
            }
            else if (startCoord1 == startCoord2)
            {
                coord = startCoord1;
            }
            else
            {
                var column1 = startCoord1.Column; //x1 2
                var row1 = startCoord1.Row; //y1  6

                var column2 = startCoord2.Column; //x2  1
                var row2 = startCoord2.Row; //y2  7




                //1..6 row
                //3..8 column


                // row = column2   3
                // col = row1 8


                var rowResult = row2;
                var columnResult = column1;

                coord = new CellCoordinates.Coord()
                {
                    Row = rowResult,
                    Column = columnResult
                };

            }


            return coord;
        }

        

        internal static UserSettings GetUserSettings(int userId)
        {
            UserSettings userSettings = new UserSettings();
            userSettings.ThemeApp = LibrarySchedule.Services.BackgroundWorker.GetThemeAppByUserId(userId);


            return userSettings;
        }

        internal static void FillViewDataThemeName(ViewDataDictionary viewData, LibrarySchedule.Types.Enums.ThemeApp theme)
        {
            viewData.Add("ThemeName", theme == LibrarySchedule.Types.Enums.ThemeApp.Dark ? "dark" : "light");
        }

        internal static void FillDateNextDay(List<DateTime> dateTimeFromTables, int countTable)
        {
            if (dateTimeFromTables.Count > 0)
            {
                var dateTimeLast = dateTimeFromTables[dateTimeFromTables.Count - 1];

                while (countTable > dateTimeFromTables.Count)
                {
                    dateTimeLast = dateTimeLast.AddDays(1);
                    dateTimeFromTables.Add(dateTimeLast);
                }
            }
        }


        internal static List<string> GetAbbreviatedNamesFromText(string text)
        {
            List<string> abbreviatedNames = new List<string>();

            var match = Regex.Match(text, @"^([\w]+) (\w{1}).* (\w{1}).*$");
            var groupCollection = match.Groups;

            foreach (object dsf in groupCollection)
            {
                var sad = match.Groups[1].Value;
            }

            return abbreviatedNames;
        }


        

        internal static long GetIdLocationCollegeByGroupId(Dictionary<long, List<GroupInfo>> idLocationCollegeForGroupsInfo, long groupId)
        {
            foreach (var keyValuePair in idLocationCollegeForGroupsInfo)
            {
                var idLocationCollege = keyValuePair.Key;
                var groupInfos = keyValuePair.Value;

                foreach (var groupInfo in groupInfos)
                {
                    if (groupInfo.Id == groupId) return idLocationCollege;
                }
            }

            DeriLibrary.Console.Worker.NotifyMessageCall($"GetIdLocationCollegeByGroupId -> Группа с id: {groupId} не нашла в списке idLocationCollegeForGroupsInfo свой idLocation");

            return 0;
        }

        internal static TypeChangesToCallScheduleForLocation GetValidChangesToCallScheduleForLocation(string text)
        {
            TypeChangesToCallScheduleForLocation typeLocation = TypeChangesToCallScheduleForLocation.All;


            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    return typeLocation;
                }

                text = text.Trim().ToLower();


                if (text.Contains("изменения в расписании звонков на шевченко"))
                {
                    typeLocation = TypeChangesToCallScheduleForLocation.Shevchenko;
                }
                else if (text.Contains("изменения в расписании звонков на гагарина"))
                {
                    typeLocation = TypeChangesToCallScheduleForLocation.Gagarino;
                }
                else
                {
                    typeLocation = TypeChangesToCallScheduleForLocation.All;
                }
            }
            catch (Exception error)
            {
                DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
            }

            return typeLocation;
        }

        internal static string NormalizationNameGroup(string nameGroup)
        {
            Regex regex = new Regex(".*\\w-[в][\\/][б].");

            nameGroup = regex.Replace(nameGroup, "").Trim();

            return nameGroup;
        }

        internal static void ShowTabelChangeScheduleV2(List<List<ModelDataTableChangeSchedule>> dataTablesChangeSchedule)
        {
            foreach (var dataTableChangeSchedules in dataTablesChangeSchedule)
            {
                try
                {
                    var consoleTable = new ConsoleTable("Группа", "Пара", "Новая пара", "Новый препод", "Аудитория");

                    foreach (var dataCell in dataTableChangeSchedules)
                    {
                        try
                        {
                            consoleTable.AddRow(dataCell.NameGroup, dataCell.NumberPair, dataCell.NewNamePairs != null ? string.Join(" / ", dataCell.NewNamePairs) : "", dataCell.NewNameTeachers != null ? string.Join(" / ", dataCell.NewNameTeachers) : "", dataCell.Audiences != null ? string.Join(" / ", dataCell.Audiences) : "");

                        }
                        catch (Exception error)
                        {
                            DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
                        }
                    }

                    Console.WriteLine(consoleTable.ToString());
                }
                catch (Exception error)
                {
                    DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
                }
            }
        }

        internal static void ShowTabelChangeScheduleV1(Dictionary<DateTime, List<ModelDataTableChangeSchedule>> dataTablesChangeSchedule)
        {
            foreach (var dataTableCells in dataTablesChangeSchedule)
            {
                //DeriLibrary.Console.Worker.NotifyMessageCall(dataTableCells.Key.ToString("dd.MM.yyyy"));

                var consoleTable = new ConsoleTable("Группа", "Пара", "Старая пара", "Старый препод", "Новая пара", "Новый препод", "Аудитория");

                foreach (var dataCell in dataTableCells.Value)
                {
                    consoleTable.AddRow(dataCell.NameGroup, dataCell.NumberPair, string.Join(" / ", dataCell.OldNamePairs), string.Join(" / ", dataCell.OldNameTeachers), string.Join(" / ", dataCell.NewNamePairs), string.Join(" / ", dataCell.NewNameTeachers), string.Join(" / ", dataCell.Audiences));
                }

                Console.WriteLine(consoleTable.ToString());
            }
        }

        internal static string ClearAllIncomingWordsInText(string tempNameTeacher, List<string> exclusionWords)
        {
            if (string.IsNullOrEmpty(tempNameTeacher))
                return "";

            if (exclusionWords.Count == 0)
                return tempNameTeacher;

            tempNameTeacher = tempNameTeacher.Trim().ToLower();

            if (tempNameTeacher == String.Empty) return "";

            foreach (string word in exclusionWords)
            {
                tempNameTeacher = tempNameTeacher.Replace(word, String.Empty).Trim();
            }

            return tempNameTeacher;
        }

        internal static string[] TextSplits(string text, List<string> listWordSplit)
        {
            List<string> resultsWord = new List<string>();

            foreach (var wordSplit in listWordSplit)
            {
                text = text.Replace(wordSplit, "[=]");
            }


            foreach (var word in text.Split("[=]"))
            {
                resultsWord.Add(word.Trim());
            }

            return resultsWord.ToArray();
        }

        internal static LibrarySchedule.Types.Enums.CellScheduleType ConvertTypeCell(CellScheduleType type)
        {
            switch (type)
            {
                case CellScheduleType.common: return LibrarySchedule.Types.Enums.CellScheduleType.common;
                case CellScheduleType.numerator: return LibrarySchedule.Types.Enums.CellScheduleType.numerator;
                case CellScheduleType.denominator: return LibrarySchedule.Types.Enums.CellScheduleType.denominator;
                default: return LibrarySchedule.Types.Enums.CellScheduleType.common;
            }
        }

        

        

        
    }

}
