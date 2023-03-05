using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LibrarySchedule.Models.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;
using static LibrarySchedule.Types.Enums;
using static ScheduleWebApp.Types.Enums;

namespace ScheduleWebApp.Controllers.Schedule
{
    [Route("api/schedule/[controller]")]
    [ApiController]

    public class CellController : ControllerBase
    {

        [HttpGet]
        public string Get(int teacher_id, int group_id, bool json_indented, DayOfWeek day_of_week, DateTime date, bool is_change, int number_pair, LibrarySchedule.Types.Enums.CellScheduleType cell_type)
        {
            try
            {
                if (teacher_id <= 0 && group_id <= 0)
                    return "Неверные параметры";

                
                var cellSchedule = LibrarySchedule.Services.DateBase.Worker.GetCellSchedule( group_id, teacher_id, day_of_week, number_pair, date,is_change, cell_type);


                var jsonCellsSchedule = new LibrarySchedule.Models.Json.JsonCellSchedule();

                if(cellSchedule!= null)
                {
                    jsonCellsSchedule.CellSchedule = cellSchedule;
                }


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
    }
}
