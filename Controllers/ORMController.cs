using Microsoft.AspNetCore.Mvc;

namespace SQL.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ORMController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
