using Lexiconner.Application.Services;
using Lexiconner.Application.Services.Interfacse;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.General;
using Lexiconner.Domain.Dtos.UserDictionaries;
using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Dtos.WordSets;
using Lexiconner.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Lexiconner.Api.Controllers.V2
{
    [ApiController]
    [Authorize]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserDictionaryController : ApiControllerBase
    {
        private readonly IUserDictionaryService _userDictionaryService;

        public UserDictionaryController(
            IUserDictionaryService userDictionaryService
        )
        {
            _userDictionaryService = userDictionaryService;
        }

        [HttpGet("{languageCode}")]
        [ProducesResponseType(typeof(BaseApiResponseDto<UserDictionaryDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUserDictionary([FromRoute] string languageCode, [FromQuery] UserDictionaryRequestDto dto)
        {
            var result = await _userDictionaryService.GetUserDictionaryAsync(GetUserId(), languageCode);
            return BaseResponse(result);
        }

        [HttpPost("{languageCode}/wordsets")]
        [ProducesResponseType(typeof(BaseApiResponseDto<UserDictionaryDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddWordSetToUserDictionary([FromRoute] string languageCode, [FromBody] AddWordSetToUserDictionaryRequestDto data)
        {
            var result = await _userDictionaryService.AddWordSetToUserDictionaryAsync(GetUserId(), languageCode, data.WordSetId);
            return BaseResponse(result);
        }

        [HttpDelete("{languageCode}/wordsets/{wordSetId}")]
        [ProducesResponseType(typeof(BaseApiResponseDto<UserDictionaryDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteWordSetFromUserDictionary([FromRoute] string languageCode, [FromRoute] string wordSetId)
        {
            var result = await _userDictionaryService.DeleteWordSetFromUserDictionaryAsync(GetUserId(), languageCode, wordSetId);
            return BaseResponse(result);
        }
    }
}
