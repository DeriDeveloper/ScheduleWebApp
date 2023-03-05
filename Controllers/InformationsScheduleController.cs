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

    public class InformationsScheduleController : ControllerBase
    {

        [HttpGet]
        public string Get(DateTime date, bool json_indented)
        {
            try
            {
                var informationsSchedule = LibrarySchedule.Services.DateBase.Worker.GetInformationsSchedule(date);


                var jsonInformationsSchedule = new LibrarySchedule.Models.Json.JsonInformationsSchedule()
                {
                    InformationsSchedule = informationsSchedule.ToList()
                };

                if (json_indented)
                {
                    return JsonConvert.SerializeObject(jsonInformationsSchedule, Formatting.Indented,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                        });
                }
                else
                {
                    return JsonConvert.SerializeObject(jsonInformationsSchedule,
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

                var informationSchedule = JsonConvert.DeserializeObject<LibrarySchedule.Models.InformationSchedule>(jsonElement.ToString());

                if (informationSchedule != null)
                {
                    if (informationSchedule.Date >= DateTime.Parse(DateTime.Now.ToString("dd.MM.yyyy")))
                    {
                        var statusOperation = await LibrarySchedule.Services.DateBase.Worker.AddInformationScheduleAsync(informationSchedule);

                        return Ok(statusOperation);
                    }
                    else
                    {
                        return Ok(new LibrarySchedule.Models.Json.StatusOperation()
                        {
                            Status = LibrarySchedule.Types.Enums.StatusOperation.InvalidFormat,
                            Message = "Дата не должна быть меньше чем сегодня"
                        });
                    }
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

                var informationSchedule = JsonConvert.DeserializeObject<LibrarySchedule.Models.InformationSchedule>(jsonElement.ToString());

                if (informationSchedule != null)
                {

                    LibrarySchedule.Models.Json.StatusOperation statusOperation = await LibrarySchedule.Services.BackgroundWorker.DeleteInformationScheduleAsync(informationSchedule);

                    if (statusOperation.Status != LibrarySchedule.Types.Enums.StatusOperation.Ok)
                    {
                        return Ok(statusOperation);
                    }


                    return Ok(new LibrarySchedule.Models.Json.StatusOperation()
                    {
                        Status = LibrarySchedule.Types.Enums.StatusOperation.Ok,
                        Message = "Успешно удалено!"
                    });
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