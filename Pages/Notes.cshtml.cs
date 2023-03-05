using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;

namespace ScheduleWebApp.Pages
{
    public class NotesModel : PageModel
    {
        public InfoDefaultAccount InfoAccount { get; set; }
        public bool IsThereAccessToEditing { get; set; } = false;
        public long GroupId { get; set; }

        public List<NoteFromGroup> Notes { get; set; }


        public IActionResult OnGet(long group_id)
        {
            InfoAccount = BackgroundWorker.UpdateDefaultDateAccountUser(HttpContext.Request.Cookies, HttpContext.Response.Cookies, ViewData);


            if (group_id > 0)
            {
                GroupId = group_id;

                //Notes = WorkerDB.GetNotesForGroup(group_id);

                if (InfoAccount.User != null)
                {
                    if (InfoAccount.User!=null)//.GroupInfo != null)
                    {
                        if (GroupId == -1)//InfoAccount.User.GroupInfo.Id)
                        {
                            IsThereAccessToEditing = true;
                        }
                    }
                }


                return Page();
            }

            return Redirect("/");
        }
    }
}
