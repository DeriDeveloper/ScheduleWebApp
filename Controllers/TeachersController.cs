using System.Data.Entity.Core.Mapping;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ScheduleWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : Controller
    {
        [HttpGet]
        public string Get()
        {
            var teachers = LibrarySchedule.Services.DateBase.Worker.GetTeachers().ToList();

            var jsonTeachers = new LibrarySchedule.Models.Json.JsonTeachers()
            {
                Teachers = teachers
            };

            return JsonConvert.SerializeObject(jsonTeachers);
        }
    }
}
