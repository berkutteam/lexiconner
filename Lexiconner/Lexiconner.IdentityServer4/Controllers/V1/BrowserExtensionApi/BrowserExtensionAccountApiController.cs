using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.Identity.Account;
using Lexiconner.Domain.Entitites;
using Lexiconner.IdentityServer4.Services.Interfaces;
using Lexiconner.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lexiconner.IdentityServer4.Controllers.V1.BrowserExtensionApi
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/browser-extension/account")]
    public class BrowserExtensionAccountApiController : ApiControllerBase
    {
        private readonly IAccountService _accountService;

        public BrowserExtensionAccountApiController(
            IAccountService accountService
        )
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        [ProducesResponseType(typeof(BaseApiResponseDto<BrowserExtensionLoginResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromBody] BrowserExtensionLoginRequestDto dto)
        {
            var result = await _accountService.BrowserExtensionLoginAsync(dto);
            return BaseResponse(result);
        }

        // Anonymous because you might have expired accessToken but valid refreshToken
        [AllowAnonymous]
        [Route("refresh-tokens")]
        [HttpPost]
        [ProducesResponseType(typeof(BaseApiResponseDto<BrowserExtensionLoginResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RefreshTokens([FromBody] BrowserExtensionRefreshTokensRequestDto dto)
        {
            var result = await _accountService.BrowserExtensionLoginAsync(dto);
            return BaseResponse(result);
        }
    }
}
