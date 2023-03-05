namespace ScheduleWebApp.Types
{
    public class Note
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Types.NoteModification Edited { get; set; }
        public Types.NoteModification Created { get; set; }
        public Types.NoteModification? Deleted { get; set; }
        public bool IsDeleted { get; set; }
    }
}
