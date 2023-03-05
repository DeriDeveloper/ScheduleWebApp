using System.Collections.Generic;

namespace ScheduleWebApp.Types
{
    public class ModelDataTableChangeSchedule
    {
        public string NameGroup { get; set; }
        public int NumberPair { get; set; }
        public List<string> OldNamePairs { get; set; }
        public List<string> OldNameTeachers { get; set; }
        public List<string> NewNamePairs { get; set; }
        public List<string> NewNameTeachers { get; set; }
        public List<string> Audiences { get; set; }
    }
}
