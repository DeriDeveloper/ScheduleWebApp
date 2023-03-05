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

    public class ScheduleController : ControllerBase
    {

        [HttpGet]
        public string Get(int teacher_id, int group_id, bool json_indented, DateTime date)
        {
            try
            {
                if (teacher_id <= 0 && group_id <= 0)
                    return "Неверные параметры";

                DateOnly selectDate = new DateOnly(date.Year, date.Month, date.Day);

                if (selectDate == DateOnly.MinValue)
                    return "Неверный формат даты";


                
                
                
                
                var cellsSchedule = LibrarySchedule.Services.BackgroundWorker.GetCellsSchedule(date, group_id, teacher_id);


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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] JsonElement jsonElement)
        {
            try
            {
                if (!LibrarySchedule.Services.BackgroundWorker.CheckAccessTokenChangeSchedule(Request.Headers[HeaderNames.Authorization]))
                {
                    return StatusCode(403);
                }

                var cellSchedule = JsonConvert.DeserializeObject<LibrarySchedule.Models.CellSchedule>(jsonElement.ToString());

                if (cellSchedule != null)
                {
                    if (cellSchedule.Date != DateTime.MinValue)
                    {
                        var timesPair = LibrarySchedule.Services.DateBase.Worker.GetTimePair(cellSchedule.NumberPair, cellSchedule.Date);
                        if (timesPair != null)
                        {
                            cellSchedule.TimesPairId = timesPair.Id;
                        }
                        else
                        {
                            return Ok(new LibrarySchedule.Models.Json.StatusOperation()
                            {
                                Status = LibrarySchedule.Types.Enums.StatusOperation.InvalidFormat,
                                Message = "Не найдено время для выбранного дня недели или даты и номера пары"
                            });
                        }
                    }
                    else
                    {
                        var timesPair = LibrarySchedule.Services.DateBase.Worker.GetTimePair(cellSchedule.NumberPair, cellSchedule.DayOfWeek);

                        if (timesPair != null)
                        {
                            cellSchedule.TimesPairId = timesPair.Id;
                        }
                        else
                        {
                            return Ok(new LibrarySchedule.Models.Json.StatusOperation()
                            {
                                Status = LibrarySchedule.Types.Enums.StatusOperation.InvalidFormat,
                                Message = "Не найдено время для выбранного дня недели или даты и номера пары"
                            });
                        }
                    }


                    var statusOperation = await LibrarySchedule.Services.BackgroundWorker.AddCellScheduleAsync(cellSchedule);
                    
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

                var cellSchedule = JsonConvert.DeserializeObject<LibrarySchedule.Models.CellSchedule>(jsonElement.ToString());

                if (cellSchedule != null)
                {
                    if(cellSchedule.Group == null)
                    {
                        return Ok(new LibrarySchedule.Models.Json.StatusOperation()
                        {
                            Status = LibrarySchedule.Types.Enums.StatusOperation.InvalidFormat,
                            Message = "Не выбрана группа!"
                        });
                    }
                    var statusOperation = await LibrarySchedule.Services.BackgroundWorker.DeleteCellScheduleAsync(cellSchedule);
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
