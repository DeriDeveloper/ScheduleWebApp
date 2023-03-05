using System.Data.Entity.Core.Mapping;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ScheduleWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileAppsController : Controller
    {
        [HttpGet]
        public string Get()
        {
            var mobileApps = LibrarySchedule.Services.DateBase.Worker.GetMobileApps();

            var jsonGroups = new LibrarySchedule.Models.Json.JsonMobileApps()
            {
                MobileApps = mobileApps
            };

            return JsonConvert.SerializeObject(jsonGroups);
        }
    }
}
