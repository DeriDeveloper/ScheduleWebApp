using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;

namespace ScheduleWebApp.Pages
{
    public class IndexModel : PageModel
    {
        //public User UserInfo { get; set; }
        public string UrlScheduleUserForProfile { get; set; }
        public InfoDefaultAccount InfoAccount { get; set; }

        public IActionResult OnGet()
        {
            InfoAccount = BackgroundWorker.UpdateDefaultDateAccountUser(HttpContext.Request.Cookies, HttpContext.Response.Cookies, ViewData);

            UrlScheduleUserForProfile = InfoAccount.UrlScheduleUserForProfile;

            if (InfoAccount != null && InfoAccount.User != null) //&& UserInfo != null
            {
                return Redirect(BackgroundWorker.GetUrlRedirectSchedule(InfoAccount.User.Id));
            }
            else
            {
                return Redirect("/SelectSchedule");
            }
        }
    }
}
