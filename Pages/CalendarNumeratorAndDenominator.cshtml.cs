using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;

namespace ScheduleWebApp.Pages
{
    public class CalendarNumeratorAndDenominatorModel : PageModel
    {
        public InfoDefaultAccount InfoAccount { get; set; }


        public LibrarySchedule.Models.DateByNumeratorAndDenominator[]? DatesNumeratorAndDenominator { get; set; }
        public List<long> TypeWeeksForProgress { get; set; }


        public void OnGet()
        {
            InfoAccount = BackgroundWorker.UpdateDefaultDateAccountUser(HttpContext.Request.Cookies, HttpContext.Response.Cookies, ViewData);

            DatesNumeratorAndDenominator = LibrarySchedule.Services.DateBase.Worker.GetDatesNumeratorAndDenominator();

            TypeWeeksForProgress = new List<long>();

            var dateNowRound = DateTime.Parse(DateTime.Now.ToString("dd.MM.yyyy") + " 00:00:00");

            for (int i = 0; i < DatesNumeratorAndDenominator.Length; i++)
            {
                var typeWeek = DatesNumeratorAndDenominator[i];

                double result = 0;




                if(typeWeek.DateEnd <= dateNowRound)
                {
                    result = 100;
                }
                else if (typeWeek.DateStart >= dateNowRound)
                {
                    result = 0;
                }
                else
                {
                    double totalDays = (typeWeek.DateEnd - typeWeek.DateStart).Days;
                    double currentDay = (dateNowRound - typeWeek.DateStart).Days;

                    result = (currentDay / totalDays) * 100;
                }

                TypeWeeksForProgress.Add(Convert.ToInt32(result));
            }
        }
    }
}
