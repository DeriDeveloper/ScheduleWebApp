using System;
using System.Collections.Generic;
using LibrarySchedule.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;

namespace ScheduleWebApp.Pages
{
    public class SelectScheduleModel : PageModel
    {
        private readonly ILogger<SelectScheduleModel> _logger;

        public string UrlScheduleUserForProfile { get; set; }

        public LibrarySchedule.Models.Group[]? Groups { get; set; }
        public LibrarySchedule.Models.Teacher[]? Teachers { get; set; }

        public InfoDefaultAccount InfoAccount { get; set; }

        //public ScheduleWebApp.Types.User UserInfo { get; set; }

        public SelectScheduleModel(ILogger<SelectScheduleModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            try
            {
                InfoAccount = BackgroundWorker.UpdateDefaultDateAccountUser(HttpContext.Request.Cookies, HttpContext.Response.Cookies, ViewData);


                UrlScheduleUserForProfile = InfoAccount.UrlScheduleUserForProfile;


                //UserInfo = InfoAccount.User;




                Groups = LibrarySchedule.Services.DateBase.Worker.GetGroups();
                Teachers = LibrarySchedule.Services.DateBase.Worker.GetTeachers();


            }
            catch (Exception error)
            {
                DeriLibrary.Console.Worker.NotifyErrorMessageCall(error.ToString());
            }

            return Page();
        }

        
    }
}
