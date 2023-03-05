namespace ScheduleWebApp.Types
{
    public class ModelCellExam
    {
        public long Id { get; set; }
        public string TypeString { get; set; }
        public LibrarySchedule.Models.Group Group { get; set; }
        public string NameExam { get; set; }
        public LibrarySchedule.Models.Teacher Teacher { get; set; }
        public string Auditory { get; set; }
        public System.DateTime Time { get; set; }
        public System.DateTime Date { get; set; }
    }
}
