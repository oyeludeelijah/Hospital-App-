using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace HospitalApp.Web.HospitalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("ReactClient")]
    public class SimpleController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "Hello from the API!", timestamp = System.DateTime.Now });
        }
    }
}
