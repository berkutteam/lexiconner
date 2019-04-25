using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Lexiconner.Api.Models;
using Lexiconner.Api.Models.RequestModels;
using Lexiconner.Api.Models.ResponseModels;
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
        public async Task<BaseApiResponseModel<object>> Get()
        {
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var sss1 = currentUser.FindFirst("name").Value;
            //var sss2 = currentUser.FindFirst("email").Value;

            object result = JsonConvert.SerializeObject(currentUserId);
            return BaseJsonResponse(result);
        }
    }
}
