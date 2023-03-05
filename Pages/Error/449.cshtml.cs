using Microsoft.AspNetCore.Mvc.RazorPages;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;

namespace ScheduleWebApp.Pages.Error
{
    public class _449Model : PageModel
    {
        public InfoDefaultAccount InfoAccount { get; set; }

        public void OnGet()
        {
            //�� ���������� ����������
            InfoAccount = BackgroundWorker.UpdateDefaultDateAccountUser(HttpContext.Request.Cookies, HttpContext.Response.Cookies, ViewData);

        }
    }
}
