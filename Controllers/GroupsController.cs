using System.Data.Entity.Core.Mapping;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ScheduleWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : Controller
    {
        [HttpGet]
        public string Get()
        {
            var groups = LibrarySchedule.Services.DateBase.Worker.GetGroups().ToList();

            var jsonGroups = new LibrarySchedule.Models.Json.JsonGroups()
            {
                Groups = groups
            };

            return JsonConvert.SerializeObject(jsonGroups);
        }
    }
}
