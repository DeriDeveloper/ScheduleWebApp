
using System;
using LibrarySchedule.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScheduleWebApp.Services;

namespace ScheduleWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CellScheduleTypeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(DateTime date)
        {
            try
            {
                var typeCellSchedule = LibrarySchedule.Services.BackgroundWorker.GetDateCellScheduleType(date);

                var jsonDateByNumeratorAndDenominator = new LibrarySchedule.Models.Json.JsonDateByNumeratorAndDenominator()
                {
                    DateByNumeratorAndDenominator = typeCellSchedule
                };

                return Ok(JsonConvert.SerializeObject(jsonDateByNumeratorAndDenominator));
            }
            catch (Exception error)
            {
                DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
                return StatusCode(500);
            }
        }
    }
}
