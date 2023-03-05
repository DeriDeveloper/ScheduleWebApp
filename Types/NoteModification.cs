using System;

namespace ScheduleWebApp.Types
{
    public class NoteModification
    {
        public Interfaces.IUser User { get; set; }
        public DateTime Date { get; set; }
    }
}
