
using System;
using System.Linq;
using LibrarySchedule.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScheduleWebApp.Services;

namespace ScheduleWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatesNumeratorAndDenominatorController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var datesNumeratorAndDenominator = LibrarySchedule.Services.DateBase.Worker.GetDatesNumeratorAndDenominator();

                var jsonDatesNumeratorAndDenominator = new LibrarySchedule.Models.Json.JsonDatesNumeratorAndDenominator();

                if(datesNumeratorAndDenominator!= null)
                {
                    jsonDatesNumeratorAndDenominator.DatesNumeratorAndDenominator = datesNumeratorAndDenominator.ToList();
                }

                return Ok(JsonConvert.SerializeObject(jsonDatesNumeratorAndDenominator));
            }
            catch (Exception error)
            {
                DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
                return Ok(error.ToString());
            }
        }
    }
}
