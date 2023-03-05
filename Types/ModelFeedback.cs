

using System;

namespace ScheduleWebApp.Types
{
    public class ModelFeedback
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string UrlFeedback { get; set; }
        public DateTime DateTimeReceipt { get; set; }
        public DateTime DateTimeAnswered { get; set; }
        public bool ExecutionStatus { get; set; }
    }
}
