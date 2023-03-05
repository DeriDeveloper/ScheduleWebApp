using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LibrarySchedule.Models.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml.ConditionalFormatting;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;
using static ScheduleWebApp.Types.Enums;

namespace ScheduleWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CellsScheduleExamsController : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> Get(int group_id, int teacher_id)
        {
            try
            {
                LibrarySchedule.Models.CellScheduleExam[] cellsScheduleExams = new LibrarySchedule.Models.CellScheduleExam[0];

                if (group_id > 0)
                {
                    cellsScheduleExams = await LibrarySchedule.Services.DateBase.Worker.GetCellScheduleExmasForGroupIdAsync(group_id);
                }
                else if (teacher_id > 0)
                {
                    cellsScheduleExams = await LibrarySchedule.Services.DateBase.Worker.GetCellScheduleExmasForTeacherIdAsync(teacher_id);
                }
                else
                {
                    cellsScheduleExams = await LibrarySchedule.Services.DateBase.Worker.GetCellScheduleExmasAsync();
                }

                var jsonCellsScheduleExams = new LibrarySchedule.Models.Json.JsonCellsScheduleExams()
                {
                    CellsScheduleExams = cellsScheduleExams
                };

                return Ok(JsonConvert.SerializeObject(jsonCellsScheduleExams));


            }
            catch (Exception error)
            {
                DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
                return StatusCode(500);
            }
        }


    }
}
