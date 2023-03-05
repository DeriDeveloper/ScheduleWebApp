
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;

namespace ScheduleWebApp.Pages
{
    public class ProfileModel : PageModel
    {
        public LibrarySchedule.Models.User? UserInfo { get; set; }
        public LibrarySchedule.Models.StudentAccount Student { get; set; }
        public LibrarySchedule.Models.Teacher Teacher { get; set; }


        public LibrarySchedule.Models.Group[] Groups { get; set; }
        public LibrarySchedule.Models.Teacher[] Teachers { get; set; }

        public InfoDefaultAccount InfoAccount { get; set; }


        public string UrlScheduleUserForProfile { get; set; }


        public IActionResult OnGet()
        {
            InfoAccount = BackgroundWorker.UpdateDefaultDateAccountUser(HttpContext.Request.Cookies, HttpContext.Response.Cookies, ViewData);

            UrlScheduleUserForProfile = InfoAccount.UrlScheduleUserForProfile;


            UserInfo = InfoAccount.User;
            if (UserInfo == null)
            {
                return RedirectToPage("Login");

            }

            switch (UserInfo.TypePerson)
            {
                case  LibrarySchedule.Types.Enums.TypePerson.Student:
                    {
                        Student = LibrarySchedule.Services.DateBase.Worker.GetStudentById(UserInfo.Id);
                        
                        if(Student == null)
                        {
                            LibrarySchedule.Services.BackgroundWorker.UpdateCookieUserId(HttpContext.Response.Cookies, null);
                        }
                        
                        Groups = LibrarySchedule.Services.DateBase.Worker.GetGroups();

                        break;
                    }
                case  LibrarySchedule.Types.Enums.TypePerson.Teacher:
                    {
                        //Teacher = LibrarySchedule.Services.DateBase.Worker.GetTeacherById(UserInfo.Id);
                        Teachers = LibrarySchedule.Services.DateBase.Worker.GetTeachers();

                        break;
                    }
            }







            return Page();
        }



        public IActionResult OnPostLogOut()
        {
            HttpContext.Response.Cookies.Append("user_id", "", new Microsoft.AspNetCore.Http.CookieOptions() { Expires = DateTime.Now.AddDays(-1) });
            return RedirectToPage("Login");
        }


    }
}
