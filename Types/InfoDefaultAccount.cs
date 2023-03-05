namespace ScheduleWebApp.Types
{
    public class InfoDefaultAccount
    {
        public LibrarySchedule.Models.User  User{ get; set; }
        public string UrlScheduleUserForProfile { get; set; }
        public UserSettings UserSettings { get; set; }
    }
}
