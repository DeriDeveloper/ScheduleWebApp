using Microsoft.AspNetCore.Mvc.RazorPages;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;

namespace ScheduleWebApp.Pages.Error
{
    public class DefaultModel : PageModel
    {
        public int StatusCode { get; private set; }
        public InfoDefaultAccount InfoAccount { get; set; }

        public void OnGet(int status_code)
        {
            StatusCode = status_code;

            InfoAccount = BackgroundWorker.UpdateDefaultDateAccountUser(HttpContext.Request.Cookies, HttpContext.Response.Cookies, ViewData);

        }
    }
}
