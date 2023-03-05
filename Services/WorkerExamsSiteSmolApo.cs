using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using OfficeOpenXml;
using ScheduleWebApp.Types;

namespace ScheduleWebApp.Services
{
    public class WorkerExamsSiteSmolApo : Interfaces.IWorkerDocumentSiteSmolAPO
    {
        public string UrlDocument { get; set; }
        public string PathSaveFile { get; set; }
        public string FullPathDocument { get; set; }
        public string FullPathDocumentTemp { get; set; }
        public Enums.TypeDocument TypeDocument { get; set; }
        public string NameFile { get; set; }
        public bool IsReady { get; set; }
        public bool IsRun { get; set; }
        public bool StatusWork { get; set; }
        public bool IsDataUpdateProcessUnderway { get; set; }
        public DateTime? DateUpdateData { get; set; }


        public int Version { get; set; }


        ExcelPackage DataExcel { get; set; }


        private static LibrarySchedule.Models.Group[] AllGroups { get; set; }

        public WorkerExamsSiteSmolApo(ModelConfigJson.WorkerExams workerExams)
        {
            UrlDocument = workerExams.Settings.Url;
            Version = workerExams.Settings.Version;
            StatusWork = workerExams.Settings.StatusWork;
            TypeDocument = Enums.TypeDocument.Xlsx;
            PathSaveFile = DeriLibrary.Console.Worker.GetEnvironmentCurrentDirectory() + "\\TempDocument\\";
            NameFile = "Exams.xlsx";
            FullPathDocument = PathSaveFile + NameFile;
            FullPathDocumentTemp = PathSaveFile + "_" + NameFile;
            IsReady = true;
            IsRun = false;
        }

        public bool DownloadDocument()
        {
            try
            {
                if (Program.MainConfigJson.WorkersSiteSmolApo.WorkerExams.Settings.WhetherToUploadFile)
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
            if (File.Exists(FullPathDocumentTemp))
                File.Delete(FullPathDocumentTemp);

            File.Copy(FullPathDocument, FullPathDocumentTemp);





            DataExcel = new ExcelPackage(new FileInfo(FullPathDocumentTemp));






        }

        public void Start()
        {
            if (StatusWork)
            {
                if (!IsRun)
                {
                    Task.Run(() =>
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                        while (true)
                        {
                            try
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
                                    //if (!DeriLibrary.Check.FilesContentsAreEqual(new FileInfo(FullPathDocument), new FileInfo(FullPathDocumentTemp)))
                                    //{
                                    Init();

                                    


                                    foreach (var excelWorksheet in DataExcel.Workbook.Worksheets)
                                    {
                                        try
                                        {
                                            var countColumns = excelWorksheet.Dimension.Columns;
                                            var countRows = excelWorksheet.Dimension.Rows;

                                            var cells = excelWorksheet.Cells;

                                            DeriLibrary.Console.Worker.NotifyMessageCall($"Book: {excelWorksheet.Name}");

                                            AllGroups = LibrarySchedule.Services.DateBase.Worker.GetGroups();

                                            var cellCoordinatesTitleDateAndGroup = GetCellCoordinatesTitleDateAndGroup(cells, countColumns, countRows);

                                            var tableCellsCoordinatesNameGroups = GetStartAndEndRowNameGroup(cells, cellCoordinatesTitleDateAndGroup, countColumns);

                                            var tableCellsCoordinatesDate = GetStartAndEndRowDate(cellCoordinatesTitleDateAndGroup, tableCellsCoordinatesNameGroups, cells, countColumns, countRows);

                                            var cellsExam = FillCellsExam(tableCellsCoordinatesNameGroups, tableCellsCoordinatesDate, cells, countColumns, countRows);

                                            FillCellsExamInDateBase(cellsExam);
                                        } catch (Exception error)
                                        {
                                            DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
                                        }
                                    }
                                    //}
                                }
                                else
                                {
                                    DeriLibrary.Console.Worker.NotifyMessageCall("Файл экзаменов не скачан");
                                }

                                DateUpdateData = DateTime.Now;

                                IsDataUpdateProcessUnderway = false;


                                DeriLibrary.Console.Worker.NotifyMessageCall($"WorkerExamsSiteSmolApo закончил парсинг и сохранения дынных за {DateTime.Now - startTime} сек.");

                                Thread.Sleep(TimeSpan.FromHours(5));
                            }
                            catch (Exception error)
                            {
                                DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
                                Thread.Sleep(TimeSpan.FromMinutes(5));
                            }
                        }
                    });
                }
            }
        }

        private void FillCellsExamInDateBase(List<LibrarySchedule.Models.CellScheduleExam> cellsExam)
        {
            LibrarySchedule.Services.BackgroundWorker.AddCellsExam(cellsExam);
        }

        private List<LibrarySchedule.Models.CellScheduleExam> FillCellsExam(List<List<CellCoordinatesForGroup>> tableCellsCoordinatesForGroups, List<List<Dictionary<CellCoordinates, DateTime>>> tableCellsCoordinatesDate, ExcelRange cells, int countColumns, int countRows)
        {
            List<LibrarySchedule.Models.CellScheduleExam> cellsExam = new List<LibrarySchedule.Models.CellScheduleExam>();

            for (int i = 0; i < tableCellsCoordinatesForGroups.Count; i++)
            {
                for (int j = 0; j < tableCellsCoordinatesForGroups[i].Count; j++)
                {
                    var cellCoordinatesForGroup = tableCellsCoordinatesForGroups[i][j];

                    DeriLibrary.Console.Worker.NotifyMessageCall($"Группа:===========> {cellCoordinatesForGroup.Group.Name}===========================================================");



                    foreach (var cellsCoordinatesDate in tableCellsCoordinatesDate[i])
                    {
                        foreach (var cellCoordinatesDate in cellsCoordinatesDate)
                        {
                            var cellCoordinates = cellCoordinatesDate.Key;
                            var dateTime = cellCoordinatesDate.Value;

                            if (dateTime.DayOfWeek != DayOfWeek.Sunday)
                            {

                                CellCoordinates.Coord intersectionСoordinates = BackgroundWorker.GetIntersectionСoordinates(cellCoordinatesForGroup.StartCoord, cellCoordinates.StartCoord);

                                if (intersectionСoordinates != null)
                                {
                                    //if (intersectionСoordinates.Column != columnNameGroup) continue;

                                    var row = intersectionСoordinates.Row;
                                    var column = intersectionСoordinates.Column;

                                    var cellTypeExam = cells[row, column].Text;
                                    var cellNameExam = cells[row + 1, column].Text;
                                    var cellNameTeacherExam = cells[row + 2, column].Text;
                                    var cellTimeAndAuditoryExam = cells[row + 3, column].Text;



                                    if (!string.IsNullOrEmpty(cellTypeExam) || !string.IsNullOrEmpty(cellNameExam) || !string.IsNullOrEmpty(cellNameTeacherExam) || !string.IsNullOrEmpty(cellTimeAndAuditoryExam))
                                    {
                                        //var audience = LibrarySchedule.Services.BackgroundWorker.GetAudienceByName(cellTimeAndAuditoryExam);
                                        
                                        LibrarySchedule.Models.CellScheduleExam cellExam = new LibrarySchedule.Models.CellScheduleExam()
                                        {
                                            //Type = cellTypeExam,
                                            Group = cellCoordinatesForGroup.Group,
                                            //NameExam = cellNameExam,
                                            //AudiencesPair = ,
                                            Date = dateTime,

                                        };

                                        /*var cellExam = new ModelCellExam()
                                        {
                                            TypeString = cellTypeExam,
                                            //GroupInfo = new GroupInfo()
                                            //{
                                            //Id = cellCoordinatesForGroup.GroupId,
                                            //Name = cellCoordinatesForGroup.NameGroup
                                            //},
                                            NameExam = cellNameExam,
                                            Auditory = cellTimeAndAuditoryExam,
                                            Date = dateTime
                                        };*/

                                        if (cellNameTeacherExam.Contains("+"))
                                        {

                                        }

                                        foreach (string teacherName in cellNameTeacherExam.Split("+"))
                                        {
                                            var teacherNameTemp = teacherName.ToLower().Trim();

                                            if (BackgroundWorker.GetValidAbbreviatedName(teacherNameTemp))
                                            {
                                                var teacherNew = new LibrarySchedule.Models.Teacher(teacherNameTemp);
                                                LibrarySchedule.Services.DateBase.Worker.AddTeacher(teacherNew);

                                                cellExam.Teacher = teacherNew;
                                                break;
                                            }
                                        }

                                        cellsExam.Add(cellExam);


                                        DeriLibrary.Console.Worker.NotifyMessageCall($"========={cellTypeExam}========= {cellNameExam} {cellNameTeacherExam} {cellTimeAndAuditoryExam} Дата: {dateTime.ToString("dd.MM")} cell cord: row: {row} column: {column}");

                                    }

                                }
                            }
                        }
                    }
                }

            }


            return cellsExam;
        }



        internal static List<List<CellCoordinatesForGroup>> GetStartAndEndRowNameGroup(ExcelRange cells, List<CellCoordinates> cellCoordinatesTitleDateAndGroup, int countColumns)
        {
            List<List<CellCoordinatesForGroup>> tableCellsCoordinatesForGroups = new List<List<CellCoordinatesForGroup>>();




            List<CellCoordinatesForGroup> cellCoordinatesForGroups = new List<CellCoordinatesForGroup>();


            foreach (var cellCoordinates in cellCoordinatesTitleDateAndGroup)
            {

                //DeriLibrary.Console.Worker.NotifyMessageCall($"column: {cellCoordinates.StartCoord.Column}, row: {cellCoordinates.StartCoord.Row}");



                var currentColumn = cellCoordinates.StartCoord.Column;

                while (currentColumn <= countColumns)
                {


                    var selectCell = cells[cellCoordinates.StartCoord.Row, ++currentColumn];

                    if (!string.IsNullOrEmpty(selectCell.Text))
                    {
                        var nameGroupTemp = selectCell.Text.Trim().ToLower();

                        if (BackgroundWorker.CheckValidNameGroup(nameGroupTemp))
                        {

                            LibrarySchedule.Models.Group? group = LibrarySchedule.Services.BackgroundWorker.GetGroupByNameFromGroups(nameGroupTemp, AllGroups);

                            if (group != null)
                            {
                                DeriLibrary.Console.Worker.NotifyMessageCall($"Группа найдена: {nameGroupTemp}");



                                if (nameGroupTemp.ToLower().Trim().Equals("013-т"))
                                {

                                }





                                cellCoordinatesForGroups.Add(new CellCoordinatesForGroup()
                                {
                                    Group = group,
                                    StartCoord = new CellCoordinates.Coord()
                                    {
                                        Row = cellCoordinates.StartCoord.Row,
                                        Column = currentColumn
                                    }
                                });



                                //DeriLibrary.Console.Worker.NotifyMessageCall($"row: {cellCoordinates.StartCoord.Row}, column: {currentColumn}, text: {cells[cellCoordinates.StartCoord.Row, currentColumn].Text}");



                                //DeriLibrary.Console.Worker.NotifyMessageCall($"row: {cellCoordinates.StartCoord.Row}, column: {currentColumn}, text: {selectCell.Text}");
                            }
                            else
                            {
                                DeriLibrary.Console.Worker.NotifyMessageCall($"1----- Не валидное имя группы: {nameGroupTemp}");

                                //break;
                            }
                        }
                        else
                        {
                            if (nameGroupTemp.Contains("-"))
                            {
                                DeriLibrary.Console.Worker.NotifyMessageCall($"Не валидное имя группы: {nameGroupTemp}");
                            }

                            break;
                        }
                    }
                }

                tableCellsCoordinatesForGroups.Add(cellCoordinatesForGroups);
                cellCoordinatesForGroups = new List<CellCoordinatesForGroup>();
            }


            return tableCellsCoordinatesForGroups;
        }
        internal static List<List<Dictionary<CellCoordinates, DateTime>>> GetStartAndEndRowDate(List<CellCoordinates> cellCoordinatesTitleDateAndGroup, List<List<CellCoordinatesForGroup>> tableCountGroupsAndCellsCoordinatesNameGroups, ExcelRange cells, int countColumns, int countRows)
        {

            List<List<Dictionary<CellCoordinates, DateTime>>> tableCellsCordinatesDate = new List<System.Collections.Generic.List<Dictionary<CellCoordinates, DateTime>>>();

            List<Dictionary<CellCoordinates, DateTime>> cellsCordDateTemp = new List<Dictionary<CellCoordinates, DateTime>>();

            var currentYear = DateTime.Now.Year;

            foreach (var cellCoordinates in cellCoordinatesTitleDateAndGroup)
            {
                //DeriLibrary.Console.Worker.NotifyMessageCall($"column: {cellCoordinates.StartCoord.Column}, row: {cellCoordinates.StartCoord.Row}");




                var currentRow = cellCoordinates.StartCoord.Row;

                while (currentRow <= countRows)
                {
                    var selectCell = cells[currentRow++, cellCoordinates.StartCoord.Column];

                    if (!string.IsNullOrEmpty(selectCell.Text))
                    {
                        string selectCellText = selectCell.Text.Trim().Replace(" ", "");

                        if (selectCellText.Contains("."))
                        {
                            var selectCellTextSplit = selectCellText.Split(".");

                            if (selectCellTextSplit.Length == 2)
                            {
                                if (DeriLibrary.Check.IsNumber(selectCellTextSplit[0], DeriLibrary.Check.TypeNumbers.Int32))
                                {
                                    if (DeriLibrary.Check.IsNumber(selectCellTextSplit[1], DeriLibrary.Check.TypeNumbers.Int32))
                                    {
                                        DateTime dateTime = DateTime.Parse($"{selectCellText}.{currentYear}");
                                        //DeriLibrary.Console.Worker.NotifyMessageCall(dateTime.ToString());

                                        cellsCordDateTemp.Add(new Dictionary<CellCoordinates, DateTime>()
                                        {
                                            {
                                                new CellCoordinates()
                                                {
                                                    StartCoord = new CellCoordinates.Coord()
                                                    {
                                                        Row = selectCell.Start.Row,
                                                        Column = selectCell.Start.Column
                                                    }
                                                },
                                                dateTime
                                            }
                                        });

                                    }
                                }
                            }
                        }
                    }
                }

                tableCellsCordinatesDate.Add(cellsCordDateTemp);
                cellsCordDateTemp = new List<Dictionary<CellCoordinates, DateTime>>();
            }




            return tableCellsCordinatesDate;
        }
        private List<CellCoordinates> GetCellCoordinatesTitleDateAndGroup(ExcelRange cells, int countColumns, int countRows)
        {
            List<CellCoordinates> cellCoordinatesTitleDateAndGroup = new List<CellCoordinates>();

            string searchTitleCell = "дата/группа";

            for (int column = 1; column <= countColumns; column++)
            {
                for (int row = 1; row <= countRows; row++)
                {
                    var selectCell = cells[row, column];

                    if (!string.IsNullOrEmpty(selectCell.Text))
                    {
                        if (selectCell.Text.Length == searchTitleCell.Length)
                        {
                            var cellText = selectCell.Text.Trim().ToLower();

                            if (cellText.Equals(searchTitleCell))
                            {

                                cellCoordinatesTitleDateAndGroup.Add(new CellCoordinates()
                                {
                                    StartCoord = new CellCoordinates.Coord()
                                    {
                                        Row = row,
                                        Column = column
                                    },
                                    EndCoord = new CellCoordinates.Coord()
                                    {
                                        Row = row,
                                        Column = column
                                    }
                                });

                                //DeriLibrary.Console.Worker.NotifyMessageCall($"column: {column}, row: {row}, address: {selectCell.Address}, text: {selectCell.Text}");

                            }
                        }
                    }
                }
            }


            return cellCoordinatesTitleDateAndGroup;
        }


        private List<CellCoordinatesForGroup> GetCellCoordinatesForGroups(ExcelRange cells, int countColumns, int countRows)
        {
            List<CellCoordinatesForGroup> cellCoordinatesForGroups = new List<CellCoordinatesForGroup>();



            for (int row = 1; row <= countRows; row++)
            {
                for (int column = 1; column <= countColumns; column++)
                {
                    var selectCell = cells[row, column];

                    if (!string.IsNullOrEmpty(selectCell.Text))
                    {
                        var nameGroupTemp = selectCell.Text.Trim().ToLower();

                        var group = LibrarySchedule.Services.BackgroundWorker.GetGroupByNameFromGroups(nameGroupTemp, AllGroups);

                        if (group != null)

                            cellCoordinatesForGroups.Add(new CellCoordinatesForGroup()
                            {
                                Group = group,
                                StartCoord = new CellCoordinates.Coord()
                                {
                                    Row = row,
                                    Column = column
                                },
                                EndCoord = new CellCoordinates.Coord()
                                {
                                    Row = row,
                                    Column = column
                                }
                            });
                        //DeriLibrary.Console.Worker.NotifyMessageCall($"row: {row}, column: {column}, text: {selectCell.Text}");
                    }
                }

            }
        


            return cellCoordinatesForGroups;
        }
    }
}
