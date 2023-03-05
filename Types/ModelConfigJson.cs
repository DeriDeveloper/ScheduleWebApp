using System.Collections.Generic;

namespace ScheduleWebApp.Types
{
    public class ModelConfigJson
    {
        public class CellSchedule
        {
            public bool ShowCellId { get; set; }
            public bool ShowCellType { get; set; }
        }

        public class DeveloperSettings
        {
            public CellSchedule CellSchedule { get; set; }
            public Schedule Schedule { get; set; }
        }

        public class NotificationMessages
        {
            public NotificationMessageUnstableData NotificationMessageUnstableData { get; set; }
        }

        public class NotificationMessageUnstableData
        {
            public bool ShowStatus { get; set; }
            public string Title { get; set; }
            public string Descrition { get; set; }
        }

        public class Root
        {
            public WorkersSiteSmolApo WorkersSiteSmolApo { get; set; }
            public Startup Startup { get; set; }
            public DeveloperSettings DeveloperSettings { get; set; }
        }

        public class Schedule
        {
            public NotificationMessages NotificationMessages { get; set; }
        }

        public class Settings
        {
            public string Url { get; set; }
            public int Version { get; set; }
            public bool StatusWork { get; set; }
            public bool WhetherToUploadFile { get; set; }
            public bool OutputTablesToConsole { get; set; }
            public bool ClearAllCellsScheduleAndChangeSchedule { get; set; }
            public bool ClearAllCellsScheduleExam { get; set; }
            public bool ClearAllInformationNotesForSchedule { get; set; }
        }

        public class Startup
        {
            public Settings Settings { get; set; }
        }

        public class WorkerChangeSchedule
        {
            public Settings Settings { get; set; }
            public List<string> ExclusionWordsFromTextWithTeachersFullName { get; set; }
        }

        public class WorkerExams
        {
            public Settings Settings { get; set; }
        }

        public class WorkerSchedule
        {
            public Settings Settings { get; set; }
        }

        public class WorkersSiteSmolApo
        {
            public WorkerSchedule WorkerSchedule { get; set; }
            public WorkerChangeSchedule WorkerChangeSchedule { get; set; }
            public WorkerExams WorkerExams { get; set; }
        }
    }
}
