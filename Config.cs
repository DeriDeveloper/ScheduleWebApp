using System.Collections.Generic;

namespace ScheduleWebApp
{
    public class Config
    {
        public static List<string> ExceptionWordsForChangeScheduleParagraphs { get; } = new List<string>()
        {
            "начальник оооп\t\t\tс. в. шустина"
        };

        public static List<string> ExceptionWords { get; } = new List<string>()
        {
            "по расписанию"
        };
        public static List<string> SkippingWordsForChangeSchedule { get; } = new List<string>()
        {
            "по расписанию",
            "перейти в"
        };

        public static List<string> NamesPairPassWords { get; } = new List<string>()
        {
            "по расписанию",
            "будет по расписанию",
        };

        public static List<string> RemoveWords { get; } = new List<string>()
        {
            "(замещение)",
            "замещение",
            "(замещ.)",
            "замещ.",
            "(замещ)",
            "замещ",
            "(1)",
            "(2)"
        };


        public static bool ShowCellId { get; } = false; // false
    }
}
