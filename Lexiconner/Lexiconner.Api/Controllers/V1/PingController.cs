using Microsoft.AspNetCore.Mvc;

namespace Lexiconner.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PingController : ApiControllerBase
    {
        public PingController()
        {
        }

        [HttpGet]
        public string Ping()
        {
            return "pong";
        }
    }
}
