using System;

namespace ScheduleWebApp.Types
{
    public class TimesPair
    {
        public long Id { get; set; }
        public Types.Enums.DayOfWeekRusShort DayOfWeekRus { get; set; }
        public DateTime DateEnd { get; set; }
        public int NumberPair { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public bool IsChange { get; set; } = false;
        public Types.Enums.TypeChangesToCallScheduleForLocation TypeLocation { get; set; }
    }
}