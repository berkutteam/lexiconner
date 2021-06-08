using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lexiconner.IdentityServer4.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/account")]
    public class AccountApiController : ApiControllerBase
    {
    }
}
