using System.Collections.Generic;

namespace ScheduleWebApp.Types
{
    public class CellSchedule
    {
        public long Id { get; set; }
        public LibrarySchedule.Models.Group Group { get; set; }
        public bool IsChange { get; set; }
        public Enums.DayOfWeekRusShort DayOfWeek { get; set; }
        public Enums.CellScheduleType Type { get; set; }
        public int NumberPair { get; set; }
        public Types.TimesPair TimesPair { get; set; } = new TimesPair();
        public List<string> NamesPair { get; set; } = new List<string>();
        public List<LibrarySchedule.Models.Teacher> TeachersPair { get; set; }
        public List<string> AudiencesPair { get; set; } = new List<string>();
        public List<GroupInfo> NamesGroupPair { get; set; }
        public CellSchedule ChangeCellSchedule { get; set; }


    }
}
