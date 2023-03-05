using System.Collections.Generic;

namespace ScheduleWebApp.Types
{
    public class ModelDataTableChangeScheduleAdvanced
    {
        public long Id { get; set; }
        public LibrarySchedule.Models.Group Group { get; set; }
        public int NumberPair { get; set; }
        public bool IsChanged { get; set; }
        public List<string> OldNamePairs { get; set; }
        public List<Teacher> OldTeachersInfo { get; set; }
        public List<string> NewNamePairs { get; set; }
        public List<Teacher> NewTeachersInfo { get; set; }
        public List<string> Audiences { get; set; }
    }
}
