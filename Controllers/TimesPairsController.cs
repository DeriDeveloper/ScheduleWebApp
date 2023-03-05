
using LibrarySchedule.Models.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using ScheduleWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xceed.Document.NET;

namespace ScheduleWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimesPairsController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(DayOfWeek dayOfWeek, bool json_indented)
        {
            var timesPairs = LibrarySchedule.Services.BackgroundWorker.GetTimesPairs(dayOfWeek);

            var jsonTimesPairs = new LibrarySchedule.Models.Json.JsonTimesPairs();

            if (timesPairs != null)
            {
                jsonTimesPairs.TimesPairs = timesPairs.ToList();

            }

            if (json_indented)
            {
                return Ok(JsonConvert.SerializeObject(jsonTimesPairs, Newtonsoft.Json.Formatting.Indented,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                            }));
            }
            else
            {
                return Ok(JsonConvert.SerializeObject(jsonTimesPairs,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                            }));
            }


        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] JsonElement jsonElement)
        {
            try
            {
                
                if(!LibrarySchedule.Services.BackgroundWorker.CheckAccessTokenChangeSchedule(Request.Headers[HeaderNames.Authorization]))
                {
                    return StatusCode(403);
                }


                var timesPair = JsonConvert.DeserializeObject<LibrarySchedule.Models.TimesPair>(jsonElement.ToString());

                if (timesPair != null)
                {
                    var statusOperation = await LibrarySchedule.Services.BackgroundWorker.AddTimesPairAsync(timesPair);

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
                var timesPair = JsonConvert.DeserializeObject<LibrarySchedule.Models.TimesPair>(jsonElement.ToString());

                if (timesPair != null)
                {
                    var statusOperation = await LibrarySchedule.Services.BackgroundWorker.DeleteTimesPairAsync(timesPair);

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
