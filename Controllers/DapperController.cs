using Microsoft.AspNetCore.Mvc;

namespace SQL.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DapperController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
