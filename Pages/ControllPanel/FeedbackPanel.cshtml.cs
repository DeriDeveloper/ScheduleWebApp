using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;

namespace ScheduleWebApp.Pages
{
    public class FeedbackPanelModel : PageModel
    {
        public InfoDefaultAccount InfoAccount { get; set; }

        public List<Types.ModelFeedback> FeedbacksNotAnswered { get; set; }
        public IActionResult OnGet()
        {
            if (BackgroundWorker.CheckAccess(HttpContext.Request.Cookies, Types.Enums.TypePrivilege.Developer))
            {
                InfoAccount = BackgroundWorker.UpdateDefaultDateAccountUser(HttpContext.Request.Cookies, HttpContext.Response.Cookies, ViewData);

                //FeedbacksNotAnswered = WorkerDB.FeedbacksNotAnswered();

                return Page();
            }
            else
            {
                return RedirectToPage("NoAccess");
            }
        }
    }
}
