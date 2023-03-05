using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;

namespace ScheduleWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudiencesController : Controller
    {
        [HttpGet]
        public async Task<string> Get()
        {
            var audiences = await LibrarySchedule.Services.DateBase.Worker.GetAudiencesAsync();

            var jsonAudiences = new LibrarySchedule.Models.Json.JsonAudiences()
            {
                Audiences = audiences.ToList()
            };

            return JsonConvert.SerializeObject(jsonAudiences);
        }
    }
}
