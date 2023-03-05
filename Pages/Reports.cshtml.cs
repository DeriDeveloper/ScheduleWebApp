using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;

namespace ScheduleWebApp.Pages
{
    public class ReportsModel : PageModel
    {
        public InfoDefaultAccount InfoAccount { get; set; }



        public List<Types.ModelGroupInfoForReport> ModelGroupsInfoForReportsToday { get; set; }
        public List<Types.ModelGroupInfoForReport> ModelGroupsInfoForReportsInAWeek { get; set; }


        public List<Types.ModelTeacherForReport> ModelTeahersInfoForReportsToday { get; set; }
        public List<Types.ModelTeacherForReport> ModelTeahersInfoForReportsInAWeek { get; set; }

        public IActionResult OnGet()
        {
            if (BackgroundWorker.CheckAccess(HttpContext.Request.Cookies, Types.Enums.TypePrivilege.Developer))
            {
                InfoAccount = BackgroundWorker.UpdateDefaultDateAccountUser(HttpContext.Request.Cookies, HttpContext.Response.Cookies, ViewData);


                var dateTimeNow = DateTime.Now;
                var dateTimeStartWeek = DeriLibrary.DateTime.GetStartDateCurrentWeek();
                var dateTimeEndWeek = DeriLibrary.DateTime.GetEndDateCurrentWeek();

                //ModelGroupsInfoForReportsToday = WorkerDB.GetCountRecordGroupForPeriod(dateTimeNow, dateTimeNow);
                //ModelGroupsInfoForReportsInAWeek = WorkerDB.GetCountRecordGroupForPeriod(dateTimeStartWeek, dateTimeEndWeek);

                //ModelTeahersInfoForReportsToday = WorkerDB.GetCountRecordTeacherForPeriod(dateTimeNow, dateTimeNow);
                //ModelTeahersInfoForReportsInAWeek = WorkerDB.GetCountRecordTeacherForPeriod(dateTimeStartWeek, dateTimeEndWeek);






                return Page();
            }
            else
            {
                return RedirectToPage("NoAccess");
            }
        }
    }
}
