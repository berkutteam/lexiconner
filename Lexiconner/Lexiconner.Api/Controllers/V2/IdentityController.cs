using IdentityModel;
using Lexiconner.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace Lexiconner.Api.Controllers.V2
{
    [ApiController]
    [Authorize(policy: "DefaultWebApiAuth")]
    [EnableCors("DefaultApi")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IdentityController : ApiControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            ClaimsPrincipal currentUser = this.User;
            string currentUserId = null;

            if (currentUser.HasClaim(x => x.Type == ClaimTypes.NameIdentifier))
            {
                currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            else if (currentUser.HasClaim(x => x.Type == JwtClaimTypes.Subject))
            {
                currentUserId = currentUser.FindFirst(JwtClaimTypes.Subject).Value;
            }
            return BaseResponse(new { Id = currentUserId, Claims = currentUser.Claims.Select(x => new { Type = x.Type, Value = x.Value }) });
        }
    }
}
