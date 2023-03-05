using System;

namespace ScheduleWebApp.Types
{
    public class InformationNoteForSchedule
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTime DateTimeStart { get; set; }
        public DateTime DateTimeEnd { get; set; }
    }
}