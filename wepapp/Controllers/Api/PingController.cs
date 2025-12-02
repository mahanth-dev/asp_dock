using Microsoft.AspNetCore.Mvc;

namespace wepapp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "API is working!",
                time = DateTime.UtcNow
            });
        }
    }
}
