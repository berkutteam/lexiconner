using Lexiconner.Application.Services;
using Lexiconner.Application.Services.Interfacse;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.Users;
using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Lexiconner.Api.Controllers.V2.BrowserExtension
{
    [ApiController]
    [Authorize]
    [EnableCors("BrowserExtensionApi")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/browser-extension/profile")]
    public class BrowserExtensionProfileController : ApiControllerBase
    {
        private readonly IUsersService _usersService;

        public BrowserExtensionProfileController(
            IUsersService usersService
        )
        {
            _usersService = usersService;
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(BaseApiResponseDto<UserDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _usersService.GetUserAsync(GetUserId());
            return BaseResponse(result);
        }

        [HttpPost("learning-languages/{languageCode}")]
        [ProducesResponseType(typeof(BaseApiResponseDto<UserDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SelectLearningLanguage([FromRoute] string languageCode)
        {
            var result = await _usersService.BrowserExtensionSelectLearningLanguageAsync(GetUserId(), languageCode);
            return BaseResponse(result);
        }
    }
}
