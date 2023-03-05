using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LibrarySchedule.Models.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;
using static LibrarySchedule.Types.Enums;
using static ScheduleWebApp.Types.Enums;

namespace ScheduleWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ScheduleExamController : ControllerBase
    {

        /*[HttpGet]
        public string Get(int teacher_id, int group_id, bool json_indented, DateTime date)
        {
            try
            {
                if (teacher_id <= 0 && group_id <= 0)
                    return "Неверные параметры";

                DateOnly selectDate = new DateOnly(date.Year, date.Month, date.Day);

                if (selectDate == DateOnly.MinValue)
                    return "Неверный формат даты";


                
                
                
                
                var cellsSchedule = LibrarySchedule.Services.BackgroundWorker.GetCellsSchedule(group_id, teacher_id, date);


                var jsonCellsSchedule = new LibrarySchedule.Models.Json.JsonCellsSchedule()
                {
                    CellsSchedule = cellsSchedule.ToList()
                };

                if (json_indented)
                {
                    return JsonConvert.SerializeObject(jsonCellsSchedule, Formatting.Indented,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                        });
                }
                else
                {
                    return JsonConvert.SerializeObject(jsonCellsSchedule,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                        });
                }
            }
            catch (Exception error)
            {
                DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
            }

            return "Неизвестная ошибка";
        }
        */
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] JsonElement jsonElement)
        {
            try
            {
                if (!LibrarySchedule.Services.BackgroundWorker.CheckAccessTokenChangeSchedule(Request.Headers[HeaderNames.Authorization]))
                {
                    return StatusCode(403);
                }

                var cellScheduleExam = JsonConvert.DeserializeObject<LibrarySchedule.Models.CellScheduleExam>(jsonElement.ToString());

                if (cellScheduleExam != null)
                {
                    var statusOperation = await LibrarySchedule.Services.BackgroundWorker.AddCellScheduleExamAsync(cellScheduleExam);
                    
                    return Ok(statusOperation);
                }
                else
                {
                    return Ok(new LibrarySchedule.Models.Json.StatusOperation()
                    {
                        Status = LibrarySchedule.Types.Enums.StatusOperation.InvalidFormat,
                         Message = "Неверный формат"
                    });
                }

            }
            catch (Exception error)
            {
                DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
                return Ok(new LibrarySchedule.Models.Json.StatusOperation()
                {
                    Status = LibrarySchedule.Types.Enums.StatusOperation.InvalidFormat,
                    Message = error.ToString()
                });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] JsonElement jsonElement)
        {
            try
            {
                if (!LibrarySchedule.Services.BackgroundWorker.CheckAccessTokenChangeSchedule(Request.Headers[HeaderNames.Authorization]))
                {
                    return StatusCode(403);
                }

                var cellScheduleExam = JsonConvert.DeserializeObject<LibrarySchedule.Models.CellScheduleExam>(jsonElement.ToString());

                if (cellScheduleExam != null)
                {
                    var statusOperation = await LibrarySchedule.Services.BackgroundWorker.DeleteCellScheduleExamAsync(cellScheduleExam);
                    return Ok(statusOperation);
                }
                else
                {
                    return Ok(new LibrarySchedule.Models.Json.StatusOperation()
                    {
                        Status = LibrarySchedule.Types.Enums.StatusOperation.InvalidFormat,
                        Message = "Неверный формат"
                    });
                }
            }
            catch (Exception error)
            {
                DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
                return Ok(new LibrarySchedule.Models.Json.StatusOperation()
                {
                    Status = LibrarySchedule.Types.Enums.StatusOperation.InvalidFormat,
                    Message = error.ToString()
                });
            }
        }
    }
}
