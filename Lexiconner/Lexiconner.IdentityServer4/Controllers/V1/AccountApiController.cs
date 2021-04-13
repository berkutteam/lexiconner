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
        //private readonly IAccountService _accountService;

        //public WordsController(
        //    IAccountService imageService
        //)
        //{
        //    _accountService = accountService;
        //}

        //[HttpGet("{id}")]
        //[ProducesResponseType(typeof(BaseApiResponseDto<WordDto>), (int)HttpStatusCode.OK)]
        //[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        //[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        //[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        //[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        //public async Task<IActionResult> Get([FromRoute] string id)
        //{
        //    var result = await _accountService.GetWordAsync(GetUserId(), id);
        //    return BaseResponse(result);
        //}
    }
}
