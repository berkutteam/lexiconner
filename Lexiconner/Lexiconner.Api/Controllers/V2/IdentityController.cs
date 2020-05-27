using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Lexiconner.Api.DTOs;
using Lexiconner.Api.Models;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Lexiconner.Api.Controllers.V2
{
    [ApiController]
    [Authorize]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IdentityController : ApiControllerBase
    {
        [HttpGet]
        public BaseApiResponseDto<string> Get()
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
            return BaseJsonResponse(currentUserId);
        }
    }
}
