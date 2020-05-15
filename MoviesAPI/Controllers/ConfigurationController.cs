using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfiguration configuration;
        public ConfigurationController(IConfiguration configuration)
        {
            this.configuration = configuration;

        }

        [HttpGet]
        public IActionResult GetAction()
        {
            return Ok();
        }
    }
}