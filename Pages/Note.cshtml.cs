using Microsoft.AspNetCore.Mvc.RazorPages;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;

namespace ScheduleWebApp.Pages
{
    public class NoteModel : PageModel
    {
        public NoteFromGroup NoteInfo { get; set; }
        public InfoDefaultAccount InfoAccount { get; set; }
        public bool IsThereAccessToEditing { get; set; } = false;

        public long NoteId { get; set; }


        public bool HasRecordBeenViewed { get; set; }

        public void OnGet(long id)
        {
            if (id > 0)
            {
                NoteId = id;

                //NoteInfo = WorkerDB.GetNoteForGroup(NoteId);


                InfoAccount = BackgroundWorker.UpdateDefaultDateAccountUser(HttpContext.Request.Cookies, HttpContext.Response.Cookies, ViewData);

                if (InfoAccount != null)
                {
                    if (InfoAccount.User != null)
                    {
                        if (InfoAccount.User!=null)//.GroupInfo != null)
                        {

                            if (NoteInfo != null)
                            {
                                if (NoteInfo.GroupId == -1)//InfoAccount.User.GroupInfo.Id)
                                {

                                    /*if (WorkerDB.GetNoteView(InfoAccount.User.Id, NoteInfo.Id) == null)
                                    {
                                        WorkerDB.AddNoteView(InfoAccount.User.Id, NoteInfo.Id);
                                    }

                                    HasRecordBeenViewed = WorkerDB.GetStatusOfViewingARecordByUser(InfoAccount.User.Id, NoteInfo.Id);
                                    */
                                    //WorkerDB.SetNoteView(InfoAccount.User.Id, NoteInfo.Id, true);


                                    IsThereAccessToEditing = true;


                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
