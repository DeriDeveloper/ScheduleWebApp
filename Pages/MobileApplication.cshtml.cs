using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScheduleWebApp.Types;

namespace ScheduleWebApp.Pages
{
    public class MobileApplicationModel : PageModel
    {
		public InfoDefaultAccount InfoAccount { get; set; }


		public void OnGet()
        {
			InfoAccount = ScheduleWebApp.Services.BackgroundWorker.UpdateDefaultDateAccountUser(HttpContext.Request.Cookies, HttpContext.Response.Cookies, ViewData);

			//UrlScheduleUserForProfile = InfoAccount.UrlScheduleUserForProfile;

		}
	}
}
