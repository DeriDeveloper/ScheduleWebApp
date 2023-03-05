using System;

namespace ScheduleWebApp.Types
{
    public class TypeWeek
    {
        public long Id { get; set; }
        public Enums.CellScheduleType Type { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }
}
