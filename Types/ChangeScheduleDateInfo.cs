using System;

namespace ScheduleWebApp.Types
{
    public class ChangeScheduleDateInfo
    {
        public int NumberDay { get; set; }
        public string MonthRus { get; set; }
        public Types.Enums.TypeMonth TypeMonth { get; set; }
        public Types.Enums.CellScheduleType CellScheduleType { get; set; }
        public DateTime DateTime { get; set; }

    }
}
