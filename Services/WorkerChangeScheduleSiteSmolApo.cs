using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ScheduleWebApp.Types;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace ScheduleWebApp.Services
{
    public class WorkerChangeScheduleSiteSmolApo : Interfaces.IWorkerDocumentSiteSmolAPO
    {
        public string UrlDocument { get; set; }
        public string PathSaveFile { get; set; }
        public string FullPathDocument { get; set; }
        public string FullPathDocumentTemp { get; set; }

        public Enums.TypeDocument TypeDocument { get; set; }
        public string NameFile { get; set; }
        public bool IsReady { get; set; }
        public List<InformationNoteForSchedule> InformationNoteForSchedules { get; set; }
        public bool IsRun { get; set; }



        public DocX DocXData { get; set; }
        public int Version { get; set; }
        public bool StatusWork { get; set; }
        public bool IsDataUpdateProcessUnderway { get; set; }
        public DateTime? DateUpdateData { get; set; } = null;



        public Types.ModelConfigJson.WorkerChangeSchedule ConfigChangeSchedule { get; set; }

        Types.Enums.TypeChangesToCallScheduleForLocation TypeLocation { get; set; }


        internal ChangeScheduleDateInfo ChangeScheduleDateInfo { get; set; }
        public Types.Enums.DayOfWeekRusShort DayOfWeekChangeSchedule { get; set; }


        private List<string> LastWordsInTablesRow { get; set; } = new List<string>();


        public WorkerChangeScheduleSiteSmolApo(ModelConfigJson.WorkerChangeSchedule configChangeSchedule)
        {
            TypeDocument = Enums.TypeDocument.Docx;
            PathSaveFile = DeriLibrary.Console.Worker.GetEnvironmentCurrentDirectory() + "\\TempDocument\\";
            NameFile = "ChangeSchedule.docx";
            FullPathDocument = PathSaveFile + NameFile;
            FullPathDocumentTemp = PathSaveFile + "_" + NameFile;
            IsReady = true;
            IsRun = false;
            ConfigChangeSchedule = configChangeSchedule;
            UrlDocument = ConfigChangeSchedule.Settings.Url;
            Version = ConfigChangeSchedule.Settings.Version;
            StatusWork = ConfigChangeSchedule.Settings.StatusWork;
        }

        public void Start()
        {
            if (StatusWork)
            {
                if (!IsRun)
                {
                    Task.Run(async () =>
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
                                Init();




                                if (Version == 1)
                                {
                                    Dictionary<DateTime, List<ModelDataTableChangeSchedule>> dataTablesChangeSchedule = new Dictionary<DateTime, List<ModelDataTableChangeSchedule>>();
                                    Dictionary<DateTime, List<ModelDataTableChangeScheduleAdvanced>> dateTimeAndFillChangeSchedule = new Dictionary<DateTime, List<ModelDataTableChangeScheduleAdvanced>>();

                                    dataTablesChangeSchedule = GetDataTablesChangeScheduleV1();
                                    dateTimeAndFillChangeSchedule = FillDataForDataTableChangeSchedule(dataTablesChangeSchedule);

                                    foreach (var keyValuePair in dateTimeAndFillChangeSchedule)
                                    {
                                        var dateTime = keyValuePair.Key;

                                        var dayOfWeekInt = (int)BackgroundWorker.GetDayOfWeekRusShort(dateTime);

                                        var DateCellScheduleType = LibrarySchedule.Services.BackgroundWorker.GetDateCellScheduleType(dateTime);
                                        var modelDataTableChangeScheduleAdvanceds = keyValuePair.Value;

                                        foreach (ModelDataTableChangeScheduleAdvanced data in modelDataTableChangeScheduleAdvanceds)
                                        {
                                            var timesPair = LibrarySchedule.Services.DateBase.Worker.GetTimePair(data.NumberPair, dateTime.DayOfWeek);

                                            await LibrarySchedule.Services.BackgroundWorker.AddCellScheduleAsync(new LibrarySchedule.Models.CellSchedule()
                                            {
                                                Group = data.Group,
                                                Date = dateTime,
                                                DayOfWeek = dateTime.DayOfWeek,
                                                IsChange = true,
                                                NumberPair = data.NumberPair,
                                                TimesPairId = timesPair.Id,
                                                TypeCell = DateCellScheduleType.TypeCellSchedule,
                                            });
                                        }
                                    }

                                    if (ConfigChangeSchedule.Settings.OutputTablesToConsole)
                                    {
                                        BackgroundWorker.ShowTabelChangeScheduleV1(dataTablesChangeSchedule);
                                    }

                                }
                                else if (Version == 2)
                                {


                                    foreach (var paragraph in DocXData.Paragraphs)
                                    {
                                        var textCurrentParagraphLower = paragraph.Text.ToLower().Trim();

                                        if (BackgroundWorker.CheckDateAndDayOfWeekAndTypeCell(textCurrentParagraphLower)) continue;

                                        if (ChangeScheduleDateInfo == null)
                                        {
                                            ChangeScheduleDateInfo = BackgroundWorker.GetChangeScheduleDateInfo(textCurrentParagraphLower);

                                            if (ChangeScheduleDateInfo == null)
                                            {
                                                var dateTime = DateTime.Now.AddDays(1);

                                                ChangeScheduleDateInfo = new ChangeScheduleDateInfo()
                                                {
                                                    DateTime = dateTime,
                                                    CellScheduleType = Enums.CellScheduleType.common,
                                                    TypeMonth = (Enums.TypeMonth)dateTime.Month,
                                                    NumberDay = dateTime.Day
                                                };

                                                DeriLibrary.Console.Worker.NotifyErrorMessageCall($"ChangeSchedule -> не найден ChangeScheduleDateInfo: {ChangeScheduleDateInfo.DateTime}");


                                                break;

                                            }
                                            else
                                            {
                                                DeriLibrary.Console.Worker.NotifyErrorMessageCall($"ChangeSchedule -> найден ChangeScheduleDateInfo: {ChangeScheduleDateInfo.DateTime}");
                                                break;
                                            }
                                        }

                                        DayOfWeekChangeSchedule = BackgroundWorker.GetDayOfWeekRusShort(ChangeScheduleDateInfo.DateTime);
                                    }

                                    var dayOfWeekInt = (int)BackgroundWorker.GetDayOfWeekRusShort(ChangeScheduleDateInfo.DateTime);




                                    var dataTablesChangeSchedule = GetDataTablesChangeScheduleV2();
                                    var tableDataCellChangeSchedule = FillDataForDataTableChangeScheduleV2(dataTablesChangeSchedule);

                                    if (ConfigChangeSchedule.Settings.OutputTablesToConsole)
                                    {
                                        BackgroundWorker.ShowTabelChangeScheduleV2(dataTablesChangeSchedule);
                                    }


                                    foreach (List<ModelDataTableChangeScheduleAdvanced> dataTableChangeScheduleAdvanceds in tableDataCellChangeSchedule)
                                    {
                                        foreach (ModelDataTableChangeScheduleAdvanced dataTableChangeScheduleAdvanced in dataTableChangeScheduleAdvanceds)
                                        {
                                            AddCellChangeSchedule(dataTableChangeScheduleAdvanced, ChangeScheduleDateInfo.CellScheduleType, dayOfWeekInt, ChangeScheduleDateInfo.DateTime);
                                        }
                                    }


                                }
                                else
                                {
                                    DeriLibrary.Console.Worker.NotifyErrorMessageCall($"Версия больше чем может быть, выбрана версия: {Version}");
                                }





                                FillLastWords();

                                FillInformationNotes();

                                List<TimesPair> changeTimesPairs = GetChangeTimesPair(true);
                                //WorkerDB.FillChangeTimesPair(changeTimesPairs, TypeLocation);

                            }
                            else
                            {
                                DeriLibrary.Console.Worker.NotifyMessageCall("Файл изменение расписание не скачан");
                            }

                            DeriLibrary.Console.Worker.NotifyMessageCall($"WorkerChangeScheduleSiteSmolApo закончил парсинг и сохранения дынных за {DateTime.Now - startTime} сек.");

                            DateUpdateData = DateTime.Now;
                            IsDataUpdateProcessUnderway = false;

                            Thread.Sleep(TimeSpan.FromMinutes(60));
                        }
                    });
                }
            }
        }

        private void AddCellChangeSchedule(ModelDataTableChangeScheduleAdvanced dataTableChangeScheduleAdvanced, Enums.CellScheduleType cellScheduleType, int dayOfWeekInt, DateTime dateTime)
        {

        }

        private void FillLastWords()
        {
            var allParagraphs = DocXData.Paragraphs;


            foreach (var table in DocXData.Tables)
            {
                foreach (var row in table.Rows)
                {
                    foreach (var cell in row.Cells)
                    {
                        foreach (var paragraph in cell.Paragraphs)
                        {
                            var text = paragraph.Text.Trim().ToLower();
                            LastWordsInTablesRow.Add(text);
                        }
                    }
                }
            }
        }

        private Dictionary<System.DateTime, List<ModelDataTableChangeScheduleAdvanced>> FillDataForDataTableChangeSchedule(Dictionary<DateTime, List<ModelDataTableChangeSchedule>> dataTableChangeSchedule)
        {
            var cellsChangeScheduleDateTime = new Dictionary<System.DateTime, List<ModelDataTableChangeScheduleAdvanced>>();

            foreach (var keyValuePair in dataTableChangeSchedule)
            {
                var dateTime = keyValuePair.Key;
                var dataTableChangeSchedules = keyValuePair.Value;

                var dataTableChangeScheduleAdvanceds = new List<ModelDataTableChangeScheduleAdvanced>();

                foreach (var tempCellSchedule in dataTableChangeSchedules)
                {
                    var selectCellSchedule = new ModelDataTableChangeScheduleAdvanced();

                    selectCellSchedule.Group = LibrarySchedule.Services.DateBase.Worker.GetGroupByName(tempCellSchedule.NameGroup);
                    selectCellSchedule.NumberPair = tempCellSchedule.NumberPair;
                    //selectCellSchedule.OldNamePairs = tempCellSchedule.OldNamePairs;
                    //selectCellSchedule.OldTeachersInfo = BackgroundWorker.GetInfoTeachers(tempCellSchedule.OldNameTeachers);
                    selectCellSchedule.NewNamePairs = tempCellSchedule.NewNamePairs;
                   // selectCellSchedule.NewTeachersInfo = BackgroundWorker.GetInfoTeachers(tempCellSchedule.NewNameTeachers);
                    selectCellSchedule.Audiences = tempCellSchedule.Audiences;


                    dataTableChangeScheduleAdvanceds.Add(selectCellSchedule);
                }




                cellsChangeScheduleDateTime.Add(dateTime, dataTableChangeScheduleAdvanceds);
            }


            return cellsChangeScheduleDateTime;
        }

        private List<List<ModelDataTableChangeScheduleAdvanced>> FillDataForDataTableChangeScheduleV2(List<List<ModelDataTableChangeSchedule>> dataTableChangeSchedule)
        {
            var cellsChangeScheduleTable = new List<List<ModelDataTableChangeScheduleAdvanced>>();

            foreach (var dataTableChangeSchedules in dataTableChangeSchedule)
            {
                var dataTableChangeScheduleAdvanceds = new List<ModelDataTableChangeScheduleAdvanced>();

                foreach (var tempCellSchedule in dataTableChangeSchedules)
                {
                    var selectCellSchedule = new ModelDataTableChangeScheduleAdvanced();

                    //selectCellSchedule.GroupInfo = LibrarySchedule.Services.DateBase.Worker.GetGroupByName(tempCellSchedule.NameGroup);
                    selectCellSchedule.NumberPair = tempCellSchedule.NumberPair;
                    //selectCellSchedule.OldNamePairs = tempCellSchedule.OldNamePairs;
                    //selectCellSchedule.OldTeachersInfo = BackgroundWorker.GetInfoTeachers(tempCellSchedule.OldNameTeachers);
                    selectCellSchedule.NewNamePairs = tempCellSchedule.NewNamePairs;
                    //selectCellSchedule.NewTeachersInfo = BackgroundWorker.GetInfoTeachers(tempCellSchedule.NewNameTeachers);
                    //selectCellSchedule.Audiences = tempCellSchedule.Audiences;
                    selectCellSchedule.IsChanged = true;


                    dataTableChangeScheduleAdvanceds.Add(selectCellSchedule);
                }




                cellsChangeScheduleTable.Add(dataTableChangeScheduleAdvanceds);
            }


            return cellsChangeScheduleTable;
        }

        private List<Types.TimesPair> GetChangeTimesPair(bool isChange)
        {
            List<Types.TimesPair> timesPairs = new List<TimesPair>();


            int indexTableChangeTimes = BackgroundWorker.GetIndexTableChangeTimes(DocXData.Tables);
            if (indexTableChangeTimes >= 0)
            {
                var table = DocXData.Tables[indexTableChangeTimes];


                foreach (var row in table.Rows)
                {
                    timesPairs.Add(new TimesPair()
                    {
                        NumberPair = Convert.ToInt32(row.Cells[0].Paragraphs[0].Text.Split(" ")[0].Trim()),
                        TimeStart = DateTime.Parse(row.Cells[1].Paragraphs[0].Text.Replace(" ", "").Replace(".", ":").Trim()),
                        TimeEnd = DateTime.Parse(row.Cells[3].Paragraphs[0].Text.Replace(" ", "").Replace(".", ":").Trim()),
                        DayOfWeekRus = DayOfWeekChangeSchedule,
                        IsChange = isChange,
                        DateEnd = ChangeScheduleDateInfo.DateTime
                    });
                }
            }


            return timesPairs;
        }

        internal void FillInformationNotes()
        {
            var allParagraphs = DocXData.Paragraphs;

            for (int i = 0; i < allParagraphs.Count; i++)
            {
                //Paragraph previousParagraph = null;
                Paragraph currentParagraph = allParagraphs[i];
                //Paragraph nextParagraph = null;


                var textCurrentParagraph = currentParagraph.Text.Trim();
                var textCurrentParagraphLower = textCurrentParagraph.ToLower();

                if (string.IsNullOrEmpty(textCurrentParagraph)) continue;

                if (LastWordsInTablesRow.Contains(textCurrentParagraphLower)) continue;

                if (Config.ExceptionWordsForChangeScheduleParagraphs.Contains(textCurrentParagraphLower)) continue;

                if (BackgroundWorker.CheckDateAndDayOfWeekAndTypeCell(textCurrentParagraphLower)) continue;

                TypeLocation = BackgroundWorker.GetValidChangesToCallScheduleForLocation(textCurrentParagraph);



                DeriLibrary.Console.Worker.NotifyMessageCall(textCurrentParagraph);

                //var informationNoteForSchedule = WorkerDB.GetInformationNoteForScheduleByText(textCurrentParagraph);


                //if (informationNoteForSchedule != null)
                //{
                    //WorkerDB.UpdateInformationNoteForSchedule(informationNoteForSchedule.Id, textCurrentParagraph);
                //}
                //else
                //{
                    //WorkerDB.AddInformationNoteForSchedule(textCurrentParagraph, DateTime.Now);
                //}

            }


        }

        public void Init()
        {
            if (File.Exists(FullPathDocumentTemp))
                File.Delete(FullPathDocumentTemp);

            File.Copy(FullPathDocument, FullPathDocumentTemp);

            using (FileStream fs = new FileStream(FullPathDocumentTemp, FileMode.Open))
            {
                DocXData = DocX.Load(fs);
            }
        }



        private Dictionary<DateTime, List<ModelDataTableChangeSchedule>> GetDataTablesChangeScheduleV1()
        {
            Dictionary<DateTime, List<ModelDataTableChangeSchedule>> tables = new Dictionary<DateTime, List<ModelDataTableChangeSchedule>>();

            try
            {
                var allParagraphs = DocXData.Paragraphs;



                List<DateTime> dateTimeFromTables = BackgroundWorker.GetDateTimeFromDocxChangeSchedule(allParagraphs);

                if (dateTimeFromTables.Count != DocXData.Tables.Count)
                {
                    DeriLibrary.Console.Worker.NotifyMessageCall("changeScedule => Количество дат и количество таблиц не совпадает, заполняем по порядку след дня");

                    BackgroundWorker.FillDateNextDay(dateTimeFromTables, DocXData.Tables.Count);
                }

                for (int indexTable = 0; indexTable < DocXData.Tables.Count; indexTable++)
                {
                    var table = DocXData.Tables[indexTable];

                    DateTime dateTimeFromTabel = DateTime.Now;

                    if (dateTimeFromTables.Count > 0)
                    {
                        dateTimeFromTabel = dateTimeFromTables[indexTable];
                    }
                    var changeScheduleCells = new List<ModelDataTableChangeSchedule>();


                    foreach (var row in table.Rows)
                    {
                        if (row.Cells.Count == 7)
                        {

                            try
                            {
                                List<string> nameGroups = new List<string>();
                                List<int> numberPairs = new List<int>();
                                List<string> oldNamePairs = new List<string>();
                                List<string> oldNameTeachers = new List<string>();
                                List<string> newNamePairs = new List<string>();
                                List<string> newNameTeachers = new List<string>();
                                List<string> audiences = new List<string>();

                                foreach (var paragraphNamesGroup in row.Cells[0].Paragraphs.ToArray())
                                {
                                    foreach (var nameGroup in paragraphNamesGroup.Text.ToLower().Split(','))
                                    {
                                        string tempNameGroup = nameGroup.Trim();

                                        if (tempNameGroup.Contains("-"))
                                        {
                                            nameGroups.Add(tempNameGroup);
                                        }
                                    }




                                }


                                foreach (var paragraphNumberPair in row.Cells[1].Paragraphs.ToArray())
                                {
                                    foreach (var numberPair in paragraphNumberPair.Text.ToLower().Split(','))
                                    {
                                        if (DeriLibrary.Check.IsNumber(numberPair, DeriLibrary.Check.TypeNumbers.Int32))
                                        {
                                            numberPairs.Add(Convert.ToInt32(numberPair));
                                        }
                                    }


                                }

                                foreach (var paragraphOldNamePair in row.Cells[2].Paragraphs.ToArray())
                                {
                                    foreach (var oldNamePair in paragraphOldNamePair.Text.ToLower().Split(','))
                                    {
                                        string tempNamePair = oldNamePair.Trim();

                                        oldNamePairs.Add(tempNamePair);
                                    }


                                }

                                foreach (var paragraphOldNameTeacher in row.Cells[3].Paragraphs.ToArray())
                                {
                                    foreach (var oldNameTeacher in paragraphOldNameTeacher.Text.ToLower().Split(','))
                                    {
                                        string tempNameTeacher = oldNameTeacher.Trim();

                                        oldNameTeachers.Add(tempNameTeacher);
                                    }


                                }

                                foreach (var paragraphNewNamePair in row.Cells[4].Paragraphs.ToArray())
                                {
                                    foreach (var newNamePair in paragraphNewNamePair.Text.ToLower().Split(','))
                                    {
                                        string tempNamePair = newNamePair.Trim();

                                        newNamePairs.Add(tempNamePair);
                                    }


                                }

                                foreach (var paragraphNewNameTeacher in row.Cells[5].Paragraphs.ToArray())
                                {
                                    foreach (var newNameTeacher in paragraphNewNameTeacher.Text.ToLower().Split(','))
                                    {
                                        string tempNameTeacher = newNameTeacher.Trim();

                                        newNameTeachers.Add(newNameTeacher);
                                    }


                                }

                                foreach (var paragraphAudiences in row.Cells[6].Paragraphs.ToArray())
                                {
                                    foreach (var auditory in paragraphAudiences.Text.ToLower().Split(','))
                                    {
                                        string tempauditory = auditory.Trim();

                                        audiences.Add(tempauditory);
                                    }


                                }


                                foreach (string nameGroup in nameGroups)
                                {

                                    foreach (int numberPair in numberPairs)
                                    {
                                        var changeScheduleCellsModel = new ModelDataTableChangeSchedule()
                                        {
                                            NameGroup = nameGroup,
                                            NumberPair = numberPair,
                                            OldNamePairs = oldNamePairs,
                                            OldNameTeachers = oldNameTeachers,
                                            NewNamePairs = newNamePairs,
                                            NewNameTeachers = newNameTeachers,
                                            Audiences = audiences
                                        };


                                        changeScheduleCells.Add(changeScheduleCellsModel);
                                    }
                                }
                            }
                            catch (Exception error)
                            {
                                DeriLibrary.Console.Worker.NotifyMessageCall($"парсинг -> {error.ToString()}");
                            }

                        }
                    }

                    tables.Add(dateTimeFromTabel, changeScheduleCells);

                }



                File.Delete(FullPathDocumentTemp);
                //DeriLibrary.Console.Worker.NotifyMessageCall(JsonConvert.SerializeObject(DateTimeAndCellsSchedule, Newtonsoft.Json.Formatting.Indented));
            }
            catch (Exception error)
            {
                DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);

            }



            return tables;
        }
        private List<List<ModelDataTableChangeSchedule>> GetDataTablesChangeScheduleV2()
        {
            List<List<ModelDataTableChangeSchedule>> tables = new List<List<ModelDataTableChangeSchedule>>();


            try
            {


                var allParagraphs = DocXData.Paragraphs;

                for (int indexTable = 0; indexTable < DocXData.Tables.Count; indexTable++)
                {
                    var table = DocXData.Tables[indexTable];


                    if (table.Rows.Count > 0)
                    {
                        if (table.Rows[0].Paragraphs[0].Text.Trim().ToLower().Contains("пара") && table.Rows[0].Paragraphs[1].Text.Trim().ToLower().Contains("группа"))
                        {
                            List<ModelDataTableChangeSchedule> tableChangeSchedules = new List<ModelDataTableChangeSchedule>();


                            List<string> groupNames = new List<string>();
                            List<string> newNamePairs = new List<string>();
                            List<string> newNameTeachers = new List<string>();
                            List<string> audiences = new List<string>();

                            for (int i = 1; i < table.Rows[0].Cells.Count; i++)
                            {
                                Cell cellSelect = table.Rows[0].Cells[i];
                                string tempNameGroup = cellSelect.Paragraphs[0].Text.Trim().ToLower();

                                groupNames.Add(BackgroundWorker.NormalizationNameGroup(tempNameGroup));
                            }



                            for (int j = 1; j < table.Rows[0].Cells.Count; j++)
                            {
                                for (int i = 1; i < table.Rows.Count; i++)
                                {
                                    newNamePairs = new List<string>();
                                    newNameTeachers = new List<string>();
                                    audiences = new List<string>();

                                    var rowSelect = table.Rows[i];
                                    var cellSelectNumPair = rowSelect.Cells[0];
                                    var cellTextNumPair = cellSelectNumPair.Paragraphs[0].Text;
                                    if (!string.IsNullOrEmpty(cellTextNumPair))
                                    {
                                        if (DeriLibrary.Check.IsNumber(cellTextNumPair, DeriLibrary.Check.TypeNumbers.Int32))
                                        {
                                            int tempNumPair = Convert.ToInt32(cellTextNumPair);

                                            var selectGroupName = groupNames[j - 1];


                                            try
                                            {
                                                foreach (var paragraphNewNamePair in table.Rows[i].Cells[j].Paragraphs.ToArray())
                                                {
                                                    foreach (var newNamePair in paragraphNewNamePair.Text.ToLower().Split(','))
                                                    {
                                                        string tempNamePair = newNamePair.Trim();

                                                        if (!string.IsNullOrEmpty(tempNamePair))
                                                        {
                                                            newNamePairs.Add(tempNamePair);
                                                        }
                                                    }
                                                }

                                                foreach (var paragraphNewNameTeacher in table.Rows[i + 1].Cells[j].Paragraphs.ToArray())
                                                {
                                                    var tempText = BackgroundWorker.TextSplits(paragraphNewNameTeacher.Text.ToLower(), new List<string>() { ",", "/" });

                                                    foreach (var newNameTeacher in tempText)
                                                    {
                                                        string tempNameTeacherOld = newNameTeacher.Trim();

                                                        if (!string.IsNullOrEmpty(tempNameTeacherOld))
                                                        {
                                                            var tempNameTeacher = BackgroundWorker.ClearAllIncomingWordsInText(tempNameTeacherOld, Program.MainConfigJson.WorkersSiteSmolApo.WorkerChangeSchedule.ExclusionWordsFromTextWithTeachersFullName);

                                                            if (!string.IsNullOrEmpty(tempNameTeacher))
                                                            {
                                                                tempNameTeacher = BackgroundWorker.GetAbbreviatedNameByFullName(tempNameTeacher);


                                                                if (!string.IsNullOrEmpty(tempNameTeacher))
                                                                {
                                                                    newNameTeachers.Add(tempNameTeacher);
                                                                }
                                                                else
                                                                {
                                                                    DeriLibrary.Console.Worker.NotifyMessageCall($"GetDataTablesChangeScheduleV2 -> парсинг учителя в ячейке, не добавлен \"{tempNameTeacherOld}\"");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                DeriLibrary.Console.Worker.NotifyMessageCall($"GetDataTablesChangeScheduleV2 -> парсинг учителя в ячейке, не добавлен \"{tempNameTeacherOld}\"");
                                                            }
                                                        }
                                                    }
                                                }

                                                foreach (var paragraphAudiences in table.Rows[i + 2].Cells[j].Paragraphs.ToArray())
                                                {
                                                    foreach (var auditory in paragraphAudiences.Text.ToLower().Split(','))
                                                    {
                                                        string tempAuditory = auditory.Trim();

                                                        if (!string.IsNullOrEmpty(tempAuditory))
                                                        {
                                                            audiences.Add(tempAuditory);
                                                        }
                                                    }


                                                }


                                                tableChangeSchedules.Add(new ModelDataTableChangeSchedule()
                                                {
                                                    NameGroup = selectGroupName,
                                                    NumberPair = tempNumPair,
                                                    NewNamePairs = newNamePairs,
                                                    NewNameTeachers = newNameTeachers,
                                                    Audiences = audiences
                                                });




                                            }
                                            catch (Exception error)
                                            {
                                                DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
                                            }
                                        }

                                    }


                                    i += 2;
                                }
                            }
                            tables.Add(tableChangeSchedules);
                        }
                    }

                }

            }
            catch (Exception error)
            {
                DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
            }

            return tables;
        }




        public bool DownloadDocument()
        {
            try
            {
                if (Program.MainConfigJson.WorkersSiteSmolApo.WorkerChangeSchedule.Settings.WhetherToUploadFile)
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

    }
}