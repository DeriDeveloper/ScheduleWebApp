using System;

namespace ScheduleWebApp.Interfaces
{
    public interface IWorkerDocumentSiteSmolAPO
    {
        public string UrlDocument { get; set; }
        public string PathSaveFile { get; set; }
        public string FullPathDocument { get; set; }
        public string FullPathDocumentTemp { get; set; }
        public Types.Enums.TypeDocument TypeDocument { get; set; }
        public string NameFile { get; set; }
        public bool IsReady { get; set; }
        public bool IsRun { get; set; }
        public int Version { get; set; }
        public bool StatusWork { get; set; }
        public bool IsDataUpdateProcessUnderway { get; set; }
        public DateTime? DateUpdateData { get; set; }


        public void Init();
        public void Start();
        public bool DownloadDocument();

    }
}
