using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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

    public class ScheduleCellsAdditionalInformationController : ControllerBase
    {

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<LibrarySchedule.Models.CellScheduleAdditionalInformation> cellScheduleAdditionalInformation = new List<LibrarySchedule.Models.CellScheduleAdditionalInformation>();

                cellScheduleAdditionalInformation.Add(new LibrarySchedule.Models.CellScheduleAdditionalInformation()
                {
                    AfterNumberOfPair = 1,
                    DayOfWeek = DayOfWeek.Monday,
                    Name = "Церемония поднятия флага",
                    TimesPair = new LibrarySchedule.Models.TimesPair()
                    {
                        DayOfWeek = DayOfWeek.Monday,
                        TimeStart = new DateTime(new TimeOnly(9, 20).Ticks),
                        TimeEnd = new DateTime(new TimeOnly(9, 30).Ticks)
                    }
                });



                cellScheduleAdditionalInformation.Add(new LibrarySchedule.Models.CellScheduleAdditionalInformation()
                {
                    AfterNumberOfPair = 2,
                    DayOfWeek = DayOfWeek.Monday,
                    Name = "Внеурочное занятие \"Разговоры о важном\"",
                    TimesPair = new LibrarySchedule.Models.TimesPair()
                    {
                        DayOfWeek = DayOfWeek.Monday,
                        TimeStart = new DateTime(new TimeOnly(10, 55).Ticks),
                        TimeEnd = new DateTime(new TimeOnly(11, 25).Ticks)
                    }
                });


                cellScheduleAdditionalInformation.Add(new LibrarySchedule.Models.CellScheduleAdditionalInformation()
                {
                    AfterNumberOfPair = 3,
                    DayOfWeek = DayOfWeek.Wednesday,
                    Name = "Кураторский час",
                    TimesPair = new LibrarySchedule.Models.TimesPair()
                    {
                        DayOfWeek = DayOfWeek.Wednesday,
                        TimeStart = new DateTime(new TimeOnly(12, 15).Ticks),
                        TimeEnd = new DateTime(new TimeOnly(12, 55).Ticks)
                    }
                });

                

                cellScheduleAdditionalInformation.Add(new LibrarySchedule.Models.CellScheduleAdditionalInformation()
                {
                    AfterNumberOfPair = 3,
                    DayOfWeek = DayOfWeek.Saturday,
                    Name = "Церемония спуска флага",
                    TimesPair = new LibrarySchedule.Models.TimesPair()
                    {
                        DayOfWeek = DayOfWeek.Saturday,
                        TimeStart = new DateTime(new TimeOnly(11, 50).Ticks),
                        TimeEnd = new DateTime(new TimeOnly(12, 00).Ticks)
                    }
                });

                var jsonCellsScheduleAdditionalInformation = new LibrarySchedule.Models.Json.JsonCellsScheduleAdditionalInformation()
                {
                    CellsScheduleAdditionalInformation = cellScheduleAdditionalInformation
                };

                return Ok(JsonConvert.SerializeObject(jsonCellsScheduleAdditionalInformation));


            }
            catch (Exception error)
            {
                DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
                return StatusCode(500);
            }
        }


    }
}
