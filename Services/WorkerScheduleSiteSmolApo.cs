using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting;
using ScheduleWebApp.Types;
using static ScheduleWebApp.Types.Enums;

namespace ScheduleWebApp.Services
{
    public class WorkerScheduleSiteSmolApo : Interfaces.IWorkerDocumentSiteSmolAPO
    {
        public string UrlDocument { get; set; }
        public string PathSaveFile { get; set; }
        public string FullPathDocument { get; set; }
        public string FullPathDocumentTemp { get; set; }
        public Enums.TypeDocument TypeDocument { get; set; }
        public string NameFile { get; set; }
        public bool IsReady { get; set; }
        public bool IsRun { get; set; }
        public int Version { get; set; }
        public bool StatusWork { get; set; }
        public bool IsDataUpdateProcessUnderway { get; set; }
        public DateTime? DateUpdateData { get; set; }



        private Dictionary<DayOfWeekRusShort, Dictionary<int, Types.TimesPair>> TimesPairForDayOfWeek { get; set; }
        private int CountRows { get; set; }
        private int CountColumns { get; set; }

        Dictionary<Types.Enums.DayOfWeekRusShort, int> CountPairForDayOfWeek { get; set; }

        private static string TitleDayOfWeeks { get; } = "дни недели";

        public WorkerScheduleSiteSmolApo(ModelConfigJson.WorkerSchedule workerSchedule)
        {
            UrlDocument = workerSchedule.Settings.Url;
            Version = workerSchedule.Settings.Version;
            StatusWork = workerSchedule.Settings.StatusWork;
            TypeDocument = Enums.TypeDocument.Xlsx;
            PathSaveFile = DeriLibrary.Console.Worker.GetEnvironmentCurrentDirectory() + "\\TempDocument\\";
            NameFile = "Schedule.xlsx";
            FullPathDocument = PathSaveFile + NameFile;
            FullPathDocumentTemp = PathSaveFile + "_" + NameFile;
            IsReady = true;
            IsRun = false;
        }

        public bool DownloadDocument()
        {
            try
            {
                if (Program.MainConfigJson.WorkersSiteSmolApo.WorkerSchedule.Settings.WhetherToUploadFile)
                {
                    DeriLibrary.Console.Worker.NotifyMessageCall($"Скачиваю {NameFile}");

                    using (var webClient = new WebClient())
                    {
                        webClient.DownloadFile(UrlDocument, FullPathDocument);
                    }

                    DeriLibrary.Console.Worker.NotifyMessageCall($"Успешно скачан {NameFile}");
                }
                else
                {
                    DeriLibrary.Console.Worker.NotifyMessageCall($"Скачивание {NameFile} невозможно из-за конфигурации worker");
                }

                return true;
            }
            catch (Exception error)
            {
                DeriLibrary.Console.Worker.NotifyMessageCall($"Произошла ошибка при скачке {NameFile}");

                DeriLibrary.Console.Worker.NotifyErrorMessageCall(error.ToString());

                return false;
            }
        }

        public void Init()
        {

        }
        public void Start()
        {
            if (StatusWork)
            {
                if (!IsRun)
                {
                    Task.Run(() =>
                    {
                        while (true)
                        {
                            DateTime startTime = DateTime.Now;

                            IsDataUpdateProcessUnderway = true;


                            if (!IsReady)
                                new Exception("Не готов!");

                            if (!DeriLibrary.Check.ExistsFolder(PathSaveFile))
                            {
                                DeriLibrary.Console.Worker.CreateFolder(PathSaveFile);
                            }


                            if (DownloadDocument() == true)
                            {
                                try
                                {
                                    if (File.Exists(FullPathDocumentTemp))
                                        File.Delete(FullPathDocumentTemp);

                                    File.Copy(FullPathDocument, FullPathDocumentTemp);

                                    Dictionary<DayOfWeekRusShort, List<CellSchedule>> DayOfWeekCellSchedule = GetDayOfWeekCellSchedule();
                                    FillInScheduleInDatabase(DayOfWeekCellSchedule);

                                    if (File.Exists(FullPathDocumentTemp))
                                        File.Delete(FullPathDocumentTemp);

                                    DayOfWeekCellSchedule.Clear();

                                }
                                catch (Exception error)
                                {
                                    DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
                                }

                            }

                            DateUpdateData = DateTime.Now;

                            IsDataUpdateProcessUnderway = false;

                            DeriLibrary.Console.Worker.NotifyMessageCall($"WorkerScheduleSiteSmolApo закончил парсинг и сохранения дынных за {DateTime.Now - startTime} сек.");
                            Thread.Sleep(TimeSpan.FromMinutes(60));
                        }
                    });
                }
            }
        }

        private async void FillInScheduleInDatabase(Dictionary<DayOfWeekRusShort, List<CellSchedule>> dayOfWeekCellSchedule)
        {
            var allGroups = LibrarySchedule.Services.DateBase.Worker.GetGroups();

            Dictionary<string, LibrarySchedule.Models.Group> groupsTemp = new Dictionary<string, LibrarySchedule.Models.Group>();

            

            foreach (var item in dayOfWeekCellSchedule)
            {
                var selectDayOfweek = item.Key;
                DeriLibrary.Console.Worker.NotifyMessageCall($"Заполнение данных для дня недели: {selectDayOfweek}");
                List<CellSchedule> cellsSchedules = item.Value;

                int countCell = cellsSchedules.Count;
                int currentNumCell = 1;

                Dictionary<int, List<CellSchedule>> GroupIdForcellSchedule = new Dictionary<int, List<CellSchedule>>();

                foreach (CellSchedule cellSchedule in cellsSchedules)
                {
                    try
                    {
                        cellSchedule.DayOfWeek = selectDayOfweek;

                        if (cellSchedule.Group.Id == 84 && cellSchedule.DayOfWeek == DayOfWeekRusShort.ПТ)
                        {

                        }

                        DeriLibrary.Console.Worker.NotifyMessageCall($"{currentNumCell++} / {countCell}");

                        LibrarySchedule.Models.Group group = allGroups.Where(g => g.Name.Equals(cellSchedule?.Group?.Name)).FirstOrDefault();

                        var dayOfWeek = Services.BackgroundWorker.ConvertToDayOfWeek(selectDayOfweek);
                        var typeCell = Services.BackgroundWorker.ConvertTypeCell(cellSchedule.Type);

                        LibrarySchedule.Models.CellSchedule cellScheduleNew = new LibrarySchedule.Models.CellSchedule()
                        {
                            Group = group,
                            IsChange = false,
                            DayOfWeek = dayOfWeek,
                            TypeCell = typeCell,
                            NumberPair = cellSchedule.NumberPair,
                            AcademicSubjects = new List<LibrarySchedule.Models.AcademicSubject>(),
                            Audiences = new List<LibrarySchedule.Models.Audience>(),
                            Teachers = new List<LibrarySchedule.Models.Teacher>() 
                        };

                        foreach(var namePair in cellSchedule.NamesPair)
                        {
                            cellScheduleNew.AcademicSubjects.Add(new LibrarySchedule.Models.AcademicSubject()
                            {
                                Name = namePair
                            });
                        }

                        foreach (var audiencePair in cellSchedule.AudiencesPair)
                        {
                            cellScheduleNew.Audiences.Add(new LibrarySchedule.Models.Audience()
                            {
                                 Name = audiencePair
                            });
                        }

                        foreach (var teacherPair in cellSchedule.TeachersPair)
                        {
                            var teaherInfo = teacherPair.GetNameInitials().Split(" ");

                            if (teaherInfo.Length == 3)
                            {

                                cellScheduleNew.Teachers.Add(new LibrarySchedule.Models.Teacher(teaherInfo[1], teaherInfo[0], teaherInfo[2]));
                            }
                            else
                            {
                                DeriLibrary.Console.Worker.NotifyErrorMessageCall($"Преподаватель не был добавлен к расписанию ,  так как фио не коректное -> FullName: {teacherPair.GetFullName()}");
                            }
                        }


                        var tempTimePair = LibrarySchedule.Services.DateBase.Worker.GetTimePair(cellSchedule.NumberPair, dayOfWeek);
                        if (tempTimePair != null)
                        {
                            cellScheduleNew.TimesPairId = tempTimePair.Id;
                        }
                        else
                        {
                            new Exception($"Время для пары не найдено в базе данных -> {cellSchedule.Group.Name} {cellSchedule.NumberPair} {dayOfWeek}");
                        }

                        await LibrarySchedule.Services.BackgroundWorker.AddCellScheduleAsync(cellScheduleNew);
                    
                        //old 
                        //WorkerDB.AddCellSchedule(cellSchedule, cellSchedule.Type, (int)selectDayOfweek);

                    }
                    catch (Exception error)
                    {
                        DeriLibrary.Console.Worker.NotifyMessageCall(error.ToString());
                    }
                }

                
            }
        }

        private Dictionary<DayOfWeekRusShort, List<CellSchedule>> GetDayOfWeekCellSchedule()
        {
            Dictionary<DayOfWeekRusShort, List<CellSchedule>> dayOfWeekCellSchedule = new Dictionary<DayOfWeekRusShort, List<CellSchedule>>()
            {
                { DayOfWeekRusShort.ПН, new List<CellSchedule>() },
                { DayOfWeekRusShort.ВТ, new List<CellSchedule>() },
                { DayOfWeekRusShort.СР, new List<CellSchedule>() },
                { DayOfWeekRusShort.ЧТ, new List<CellSchedule>() },
                { DayOfWeekRusShort.ПТ, new List<CellSchedule>() },
                { DayOfWeekRusShort.СБ, new List<CellSchedule>() }
            };

            //TimesPairForDayOfWeek = WorkerDB.GetAllTimesPairForDayOfWeek();


            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(new FileInfo(FullPathDocumentTemp)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

                CountColumns = worksheet.Dimension.Columns;
                CountRows = worksheet.Dimension.Rows;

                var cells = worksheet.Cells;

                // List<GroupInfo> allGroupInfo = WorkerDB.GetAllGroupsInfo();
                LibrarySchedule.Models.Group[] allGroupInfo = LibrarySchedule.Services.DateBase.Worker.GetGroups();

                List<Types.CellCoordinatesForGroup> cellCoordinatesForGroups = GetCellCoordinatesForGroups(cells, allGroupInfo);

                Dictionary<Types.Enums.DayOfWeekRusShort, int> CountPairForDayOfWeek = GetCountPairForDayOfWeek(cells);

                // узнав все координаты групп и кол-во пар для дня недели, теперь надо найти расписание и там узгать не пустая ли она и найти тип ячейки 
                var cellSchedule = GetDayOfWeeksCellScheduleForGroup(cells, dayOfWeekCellSchedule, cellCoordinatesForGroups, CountPairForDayOfWeek);
            }


            return dayOfWeekCellSchedule;
        }

        private Dictionary<Types.Enums.DayOfWeekRusShort, List<CellSchedule>> GetDayOfWeeksCellScheduleForGroup(ExcelRange cells, Dictionary<DayOfWeekRusShort, List<CellSchedule>> dayOfWeeksScheduleForGroup, List<CellCoordinatesForGroup> cellCoordinatesForGroups, Dictionary<DayOfWeekRusShort, int> countPairForDayOfWeek)
        {
            try
            {
                LibrarySchedule.Models.Teacher[] teachers = LibrarySchedule.Services.DateBase.Worker.GetTeachers();

                for (int indexGroup = 0; indexGroup < cellCoordinatesForGroups.Count; indexGroup++)
                {
                    CellCoordinatesForGroup selectGroupInfo = cellCoordinatesForGroups[indexGroup];

                    DeriLibrary.Console.Worker.NotifyMessageCall("Группа: " + selectGroupInfo.Group.Name);

                    

                    //if (selectGroupInfo.NameGroup.ToLower().Equals("013-пр"))
                    //{  //удалить
                    int currentRow = selectGroupInfo.StartCoord.Row + 1;
                    int currentColumn = selectGroupInfo.StartCoord.Column;


                    foreach (var selectDayOfWeekRusShort in countPairForDayOfWeek.Keys)
                    {
                        //DeriLibrary.Console.Worker.NotifyMessageCall("День недели: " + selectDayOfWeekRusShort.ToString());

                        try
                        {
                            for (int numPair = 0; numPair < countPairForDayOfWeek[selectDayOfWeekRusShort]; numPair++)
                            {
                                int numberPair = numPair + 1;

                                string addressOfTopRightCell = "";
                                string addressOfTopRightCellTemp = "";
                                string addressOfBottomRightCell = "";
                                string addressOfBottomRightCellTemp = "";

                                bool areAudienceCellsCombined = false;
                                bool CellHasNumerator = false;
                                bool CellHasDenominator = false;
                                bool CellHasNumeratorAndDenominator = false;

                                //DeriLibrary.Console.Worker.NotifyMessageCall("Номер пары: " + numberPair.ToString());

                                // Текст из ячейки слева сверху
                                ExcelRange selectCellLeftTop = cells[currentRow, currentColumn];
                                selectCellLeftTop = cells[currentRow, currentColumn, selectCellLeftTop.End.Row, selectCellLeftTop.End.Column];

                                string selectCellTextLeftTop = selectCellLeftTop.Text;
                                //CellCoordinatesForGroup selectCellCoordinateLeftTop = new CellCoordinatesForGroup() { StartCoord = };

                                // Текст из ячейки справо сверху
                                ExcelRange selectCellRightTop = cells[currentRow, currentColumn + 1];
                                string selectCellTextRightTop = selectCellRightTop.Text;

                                addressOfTopRightCell = selectCellRightTop.Address;

                                selectCellRightTop = cells[currentRow, currentColumn + 1, selectCellRightTop.End.Row, selectCellRightTop.End.Column];

                                addressOfTopRightCellTemp = selectCellRightTop.Address;

                                string selectCellTextRightTopTemp = selectCellRightTop.Text;


                                currentRow++;


                                // Текст из ячейки слева снизу
                                ExcelRange selectCellLeftBottom = cells[currentRow, currentColumn];
                                selectCellLeftBottom = cells[currentRow, currentColumn, selectCellLeftBottom.End.Row, selectCellLeftBottom.End.Column];

                                string selectCellTextLeftBottom = selectCellLeftBottom.Text;


                                // Текст из ячейки справа снизу
                                ExcelRange selectCellRightBottom = cells[currentRow, currentColumn + 1];
                                selectCellRightBottom = cells[currentRow, currentColumn + 1, selectCellRightBottom.End.Row, selectCellRightBottom.End.Column];

                                addressOfBottomRightCell = selectCellRightBottom.Address;
                                string selectCellTextRightBottom = selectCellRightBottom.Text;










                                if (!string.IsNullOrEmpty(selectCellTextLeftTop) || !string.IsNullOrEmpty(selectCellTextRightTop) || !string.IsNullOrEmpty(selectCellTextLeftBottom) || !string.IsNullOrEmpty(selectCellTextRightBottom))
                                {
                                    CellScheduleType cellScheduleType = CellScheduleType.common;

                                    var exsistTeacherCellTopLeft = BackgroundWorker.CheckExistTeacherNamePairInAllTeacher(selectCellTextLeftTop, teachers);
                                    var exsistTeacherCellTBottomLeft = BackgroundWorker.CheckExistTeacherNamePairInAllTeacher(selectCellTextLeftBottom, teachers);

                                    if (!selectCellRightTop.Merge && exsistTeacherCellTopLeft == null)
                                    {
                                        areAudienceCellsCombined = false;
                                    }
                                    else if (selectCellRightTop.Merge)
                                    {
                                        areAudienceCellsCombined = true;
                                    }
                                    else
                                    {
                                        areAudienceCellsCombined = false;
                                    }

                                    if (!areAudienceCellsCombined)
                                    {
                                        //если не объединеные ячейки тогда ищем тип ячейки

                                        //если пустые снизу ячейки то значит тип числитель
                                        if (string.IsNullOrEmpty(selectCellTextLeftBottom) && string.IsNullOrEmpty(selectCellTextRightBottom))
                                        {
                                            cellScheduleType = CellScheduleType.numerator;
                                        }
                                        else if (exsistTeacherCellTopLeft != null)
                                        {
                                            CellHasNumerator = true;
                                        }
                                        //если пустые сверху ячейки то значит тип знаменатель
                                        if (string.IsNullOrEmpty(selectCellTextLeftTop) && string.IsNullOrEmpty(selectCellTextRightTop))
                                        {
                                            cellScheduleType = CellScheduleType.denominator;
                                        }
                                        else if (exsistTeacherCellTBottomLeft != null)
                                        {
                                            CellHasDenominator = true;
                                        }


                                        if (CellHasNumerator && CellHasDenominator)
                                        {
                                            CellHasNumeratorAndDenominator = true;
                                        }
                                    }

                                    /*if ()
                                     {
                                         cellScheduleType = CellScheduleType.numerator
                                     }
                                     else if ()
                                     {
                                         cellScheduleType = CellScheduleType.denominator;
                                     }*/

                                    //CellHasNumeratorAndDenominator;

                                    



                                    if (CellHasNumeratorAndDenominator)
                                    {
                                        FillInCellWithDataAndAddIt(Enums.CellScheduleType.numerator, selectGroupInfo, numberPair, selectCellTextLeftTop, selectCellTextRightTop, selectCellTextLeftBottom, selectCellTextRightBottom, selectDayOfWeekRusShort, dayOfWeeksScheduleForGroup, false, selectCellTextRightTopTemp);
                                        FillInCellWithDataAndAddIt(Enums.CellScheduleType.denominator, selectGroupInfo, numberPair, selectCellTextLeftTop, selectCellTextRightTop, selectCellTextLeftBottom, selectCellTextRightBottom, selectDayOfWeekRusShort, dayOfWeeksScheduleForGroup, false, selectCellTextRightTopTemp);
                                    }
                                    else
                                    {
                                        FillInCellWithDataAndAddIt(cellScheduleType, selectGroupInfo, numberPair, selectCellTextLeftTop, selectCellTextRightTop, selectCellTextLeftBottom, selectCellTextRightBottom, selectDayOfWeekRusShort, dayOfWeeksScheduleForGroup, areAudienceCellsCombined, selectCellTextRightTopTemp);
                                    }
                                }
                                currentRow++;

                            }


                        }
                        catch (Exception error)
                        {
                            DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
                        }

                        //}
                    }
                }
            }
            catch (Exception error)
            {
                DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
            }


            return dayOfWeeksScheduleForGroup;
        }

        private void FillInCellWithDataAndAddIt(Types.Enums.CellScheduleType cellScheduleType, CellCoordinatesForGroup selectGroupInfo, int numberPair, string selectCellTextLeftTop, string selectCellTextRightTop, string selectCellTextLeftBottom, string selectCellTextRightBottom, Types.Enums.DayOfWeekRusShort selectDayOfWeekRusShort, Dictionary<DayOfWeekRusShort, List<CellSchedule>> dayOfWeeksScheduleForGroup, bool areAudienceCellsCombined, string selectCellTextRightTopTemp)
        {
            LibrarySchedule.Models.Teacher[] teachers = LibrarySchedule.Services.DateBase.Worker.GetTeachers();

            var cellSchedule = new CellSchedule()
            {
                Group =  selectGroupInfo.Group,
                NumberPair = numberPair,
                IsChange = false,
                Type = cellScheduleType,
                NamesPair = new List<string>(),
                TeachersPair = new List<LibrarySchedule.Models.Teacher>(),
                AudiencesPair = new List<string>()

            };

            var teachersNew = new List<LibrarySchedule.Models.Teacher>();

            switch (cellSchedule.Type)
            {
                case CellScheduleType.numerator:
                    {
                        cellSchedule.NamesPair.Add(BackgroundWorker.RemoveFullNameOfTeachersFromText(selectCellTextLeftTop, teachers));
                        
                        BackgroundWorker.ExtractFullNameOfTeacherFromText(teachersNew, selectCellTextLeftTop, teachers);
                        cellSchedule.TeachersPair.AddRange(teachersNew);
                        cellSchedule.AudiencesPair.Add(selectCellTextRightTop);
                        //DeriLibrary.Console.Worker.NotifyMessageCall(selectCellTextRightTop);


                        break;
                    }
                case CellScheduleType.denominator:
                    {
                        cellSchedule.NamesPair.Add(BackgroundWorker.RemoveFullNameOfTeachersFromText(selectCellTextLeftBottom, teachers));
                        
                        BackgroundWorker.ExtractFullNameOfTeacherFromText(teachersNew, selectCellTextLeftBottom, teachers);
                        cellSchedule.TeachersPair.AddRange(teachersNew);
                        cellSchedule.AudiencesPair.Add(selectCellTextRightBottom);
                        //DeriLibrary.Console.Worker.NotifyMessageCall(selectCellTextRightBottom);


                        break;
                    }
                case CellScheduleType.common:
                    {
                        cellSchedule.NamesPair.Add(BackgroundWorker.RemoveFullNameOfTeachersFromText(selectCellTextLeftTop, teachers));
                       
                        BackgroundWorker.ExtractFullNameOfTeacherFromText(teachersNew, selectCellTextLeftBottom, teachers);
                        cellSchedule.TeachersPair.AddRange(teachersNew);
                        if (!areAudienceCellsCombined)
                        {
                            cellSchedule.AudiencesPair.Add(selectCellTextRightTop);
                            if (!selectCellTextRightTop.Equals(selectCellTextRightBottom) && !string.IsNullOrEmpty(selectCellTextRightBottom))
                            {
                                cellSchedule.AudiencesPair.Add(selectCellTextRightBottom);
                            }

                        }
                        else
                        {
                            cellSchedule.AudiencesPair.Add(selectCellTextRightTop);

                        }
                        //DeriLibrary.Console.Worker.NotifyMessageCall(selectCellTextRightTop);
                        break;
                    }
            }


            if (TimesPairForDayOfWeek[selectDayOfWeekRusShort].Keys.Contains(cellSchedule.NumberPair))
            {
                cellSchedule.TimesPair.TimeStart = TimesPairForDayOfWeek[selectDayOfWeekRusShort][cellSchedule.NumberPair].TimeStart;
                cellSchedule.TimesPair.TimeEnd = TimesPairForDayOfWeek[selectDayOfWeekRusShort][cellSchedule.NumberPair].TimeEnd;
            }
            else
            {
                cellSchedule.TimesPair.TimeStart = DateTime.Parse("00:00");
                cellSchedule.TimesPair.TimeEnd = DateTime.Parse("00:00");
            }

            if (cellSchedule.NumberPair == 0)
            {

            }


            bool exsistCellTextLeftTop = false;
            bool exsistCellTextRightTop = false;
            bool exsistCellTextLeftBottom = false;
            bool exsistCellTextRightBottom = false;

            /*if (!string.IsNullOrEmpty(selectCellTextLeftTop))
            {
                exsistCellTextLeftTop = true;
            }*/

            /*if (!string.IsNullOrEmpty(selectCellTextLeftBottom))
            {
                exsistCellTextLeftBottom = true;
            }*/

            if (!string.IsNullOrEmpty(selectCellTextRightTop))
            {
                exsistCellTextRightTop = true;
            }

            if (!string.IsNullOrEmpty(selectCellTextRightBottom))
            {
                exsistCellTextRightBottom = true;
            }


            if (exsistCellTextRightTop)
            {
                // DeriLibrary.Console.Worker.NotifyMessageCall($"Адресс: {selectCellRightTop.Address} 1:" + selectCellTextRightTop);
            }

            if (exsistCellTextRightBottom)
            {
                //DeriLibrary.Console.Worker.NotifyMessageCall($"Адресс: {selectCellRightBottom.Address} 2:" + selectCellTextRightBottom);
            }
            // готовые данные внести в ячейку 
            dayOfWeeksScheduleForGroup[selectDayOfWeekRusShort].Add(cellSchedule);
        }

        private Dictionary<DayOfWeekRusShort, int> GetCountPairForDayOfWeek(ExcelRange cells)
        {
            Dictionary<DayOfWeekRusShort, int> countPairForDayOfWeeks = new Dictionary<DayOfWeekRusShort, int>()
            {
                { DayOfWeekRusShort.ПН, 0 },
                { DayOfWeekRusShort.ВТ, 0 },
                { DayOfWeekRusShort.СР, 0 },
                { DayOfWeekRusShort.ЧТ, 0 },
                { DayOfWeekRusShort.ПТ, 0 },
                { DayOfWeekRusShort.СБ, 0 },
            };

            Types.CellCoordinates cellCoordinatesTitleDayOfWeeks = GetCellCoordinatesTitleDayOfWeeks(cells);

            if (cellCoordinatesTitleDayOfWeeks != null)
            {
                var startCoord = cellCoordinatesTitleDayOfWeeks.EndCoord;
                startCoord.Row++;
                startCoord.Column++;

                for (int indexDayOfWeek = 0; indexDayOfWeek < countPairForDayOfWeeks.Count; indexDayOfWeek++)
                {
                    //var dayOfWeekRusShort = countPairForDayOfWeeks[(DayOfWeekRusShort)indexDayOfWeek];
                    int countPair = 0;

                    for (int rowIndex = startCoord.Row; rowIndex < CountRows; rowIndex++)
                    {
                        var selectCell = cells[rowIndex, startCoord.Column];

                        if (!string.IsNullOrEmpty(selectCell.Text))
                        {
                            string selectCellText = selectCell.Text;
                            if (DeriLibrary.Check.IsNumber(selectCellText, DeriLibrary.Check.TypeNumbers.Int32))
                            {
                                int numPair = Convert.ToInt32(selectCellText);

                                if (countPair == 0)
                                {
                                    countPair++;
                                }
                                else if (numPair != 1)
                                {
                                    countPair++;
                                }
                                else
                                {
                                    startCoord.Row = rowIndex;
                                    break;
                                }

                            }
                        }
                    }

                    countPairForDayOfWeeks[(DayOfWeekRusShort)indexDayOfWeek] = countPair;
                }
            }

            return countPairForDayOfWeeks;
        }

        private CellCoordinates GetCellCoordinatesTitleDayOfWeeks(ExcelRange cells)
        {
            Types.CellCoordinates cellCoordinatesTitleDayOfWeeks = null;

            foreach (var cell in cells)
            {
                if (!string.IsNullOrEmpty(cell.Text))
                {
                    string cellTextOld = cell.Text;
                    string cellText = cell.Text.ToLower().Trim();

                    if (cellText.Length == TitleDayOfWeeks.Length)
                    {
                        if (cellText.Equals(TitleDayOfWeeks))
                        {
                            cellCoordinatesTitleDayOfWeeks = new CellCoordinates()
                            {
                                StartCoord = new CellCoordinates.Coord()
                                {
                                    Row = cell.Start.Row,
                                    Column = cell.Start.Column
                                },
                                EndCoord = new CellCoordinates.Coord()
                                {
                                    Row = cell.End.Row,
                                    Column = cell.End.Column
                                }
                            };
                            break;
                        }
                    }
                }
            }


            return cellCoordinatesTitleDayOfWeeks;
        }

        private List<CellCoordinatesForGroup> GetCellCoordinatesForGroups(ExcelRange cells, LibrarySchedule.Models.Group[] groups)
        {
            List<Types.CellCoordinatesForGroup> cellCoordinatesForGroups = new List<CellCoordinatesForGroup>();


            foreach (var cell in cells)
            {
                if (!string.IsNullOrEmpty(cell.Text))
                {
                    var cellText = BackgroundWorker.NormalizationNameGroup(cell.Text.ToLower().Trim());

                    if (cellText.Contains("в/б"))
                    {
                        DeriLibrary.Console.Worker.NotifyMessageCall($"Группа имеет в/б: {cellText}");
                    }

                    bool groupFound = false;

                    foreach (var groupInfo in groups)
                    {
                        if (cellText.Equals(groupInfo.Name.ToLower().Trim()))
                        {
                            cellCoordinatesForGroups.Add(new CellCoordinatesForGroup()
                            {
                                Group = new LibrarySchedule.Models.Group()
                                {
                                    Id = groupInfo.Id,
                                    Name = groupInfo.Name
                                },
                                StartCoord = new CellCoordinates.Coord()
                                {
                                    Row = cell.Start.Row,
                                    Column = cell.Start.Column
                                },
                                EndCoord = new CellCoordinates.Coord()
                                {
                                    Row = cell.End.Row,
                                    Column = cell.End.Column
                                }
                            });

                            groupFound = true;
                            break;
                        };
                    }
                    if (!groupFound)
                    {
                        //DeriLibrary.Console.Worker.NotifyMessageCall($"Группа не найдена в бд: {cellText}");

                    }
                }
            }

            return cellCoordinatesForGroups;

        }
    }
}


