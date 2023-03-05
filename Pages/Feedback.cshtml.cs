using Microsoft.AspNetCore.Mvc.RazorPages;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;

namespace ScheduleWebApp.Pages
{
    public class FeedbackModel : PageModel
    {

        public LibrarySchedule.Models.User? UserInfo { get; set; }
        public string RedirectUrl { get; set; }
        public InfoDefaultAccount InfoAccount { get; set; }


        public void OnGet(string redirect_url)
        {
            InfoAccount = BackgroundWorker.UpdateDefaultDateAccountUser(HttpContext.Request.Cookies, HttpContext.Response.Cookies, ViewData);

            if (InfoAccount != null && InfoAccount.User != null)
            {
                UserInfo = InfoAccount.User;
            }

            if (!string.IsNullOrEmpty(redirect_url))
            {
                RedirectUrl = redirect_url;
            }
        }
    }
}
