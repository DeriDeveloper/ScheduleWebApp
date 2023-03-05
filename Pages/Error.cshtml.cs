using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;

namespace ScheduleWebApp.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        public string RequestId { get; set; }
        public int PageStatusCode { get; set; }
        public LibrarySchedule.Models.User? UserInfo { get; set; }


        public InfoDefaultAccount InfoAccount { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            UserInfo = LibrarySchedule.Services.BackgroundWorker.CheckUserForAuthorization(HttpContext.Request.Cookies["user_id"]);

            InfoAccount = BackgroundWorker.UpdateDefaultDateAccountUser(HttpContext.Request.Cookies, HttpContext.Response.Cookies, ViewData);


            //RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            PageStatusCode = HttpContext.Response.StatusCode;
            //_logger.Log(LogLevel.Error, "Code error: {0}", PageStatusCode);
            //_logger.Log(LogLevel.Error, "ошибка в error");


            switch (PageStatusCode)
            {
                case 404:
                    {
                        return RedirectToPage("Error/404");
                    }
                default:
                    {
                        return RedirectToPage($"Error/Default", new { status_code = PageStatusCode });
                    }
            }
        }

    }
}
