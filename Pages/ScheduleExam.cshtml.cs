using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;

namespace ScheduleWebApp.Pages
{
    public class ScheduleExamModel : PageModel
    {
        public bool IsScheduleDataUpdateProcessUnderway { get; set; } = false;



        public LibrarySchedule.Models.Group Group { get; private set; }
        public LibrarySchedule.Models.Teacher Teacher { get; private set; }


        public InfoDefaultAccount InfoAccount { get; set; }

        public LibrarySchedule.Models.CellScheduleExam[]? CellsScheduleExams { get; private set; }

        public async Task<IActionResult> OnGet(int group_id, int teacher_id)
        {
            if (Program.WorkerExamsSiteSmolApo.IsDataUpdateProcessUnderway)
            {
                IsScheduleDataUpdateProcessUnderway = true;
            }

            if (group_id > 0)
            {
                int groupId = group_id;

                Group = LibrarySchedule.Services.DateBase.Worker.GetGroupById(groupId);

                CellsScheduleExams = await LibrarySchedule.Services.DateBase.Worker.GetCellScheduleExmasForGroupIdAsync(groupId);

            }
            else if (teacher_id > 0)
            {
                int teacherId = teacher_id;

                Teacher = LibrarySchedule.Services.DateBase.Worker.GetTeacherById(teacherId);

				CellsScheduleExams = await LibrarySchedule.Services.DateBase.Worker.GetCellScheduleExmasForTeacherIdAsync(teacherId);
			}
            else
            {
                return RedirectToPage("Error/449");
            }

            InfoAccount = BackgroundWorker.UpdateDefaultDateAccountUser(HttpContext.Request.Cookies, HttpContext.Response.Cookies, ViewData);

            return Page();
        }
    }
}
