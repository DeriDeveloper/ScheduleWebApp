using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;

namespace ScheduleWebApp.Pages
{
    public class ControllPanelModel : PageModel
    {
        public InfoDefaultAccount InfoAccount { get; set; }

        public Dictionary<string, string> Links { get; set; } = new Dictionary<string, string>()
        {
            { "Обратная связь", "/ControllPanel/FeedbackPanel" }
        };

        public IActionResult OnGet()
        {
            if (BackgroundWorker.CheckAccess(HttpContext.Request.Cookies, Types.Enums.TypePrivilege.Developer))
            {
                InfoAccount = BackgroundWorker.UpdateDefaultDateAccountUser(HttpContext.Request.Cookies, HttpContext.Response.Cookies, ViewData);


                return Page();
            }
            else
            {
                return RedirectToPage("NoAccess");
            }
        }
    }
}
