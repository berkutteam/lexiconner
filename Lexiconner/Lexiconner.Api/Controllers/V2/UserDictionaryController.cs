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

        [HttpGet]
        [ProducesResponseType(typeof(BaseApiResponseDto<UserDictionaryDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUserDictionary([FromQuery] UserDictionaryRequestDto dto)
        {
            var result = await _userDictionaryService.GetUserDictionaryAsync(GetUserId(), dto.LanguageCode);
            return BaseResponse(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseApiResponseDto<UserDictionaryDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddWordSetToUserDictionary([FromBody] AddWordSetToUserDictionaryRequestDto data)
        {
            var result = await _userDictionaryService.AddWordSetToUserDictionaryAsync(GetUserId(), data.LanguageCode, data.WordSetId);
            return BaseResponse(result);
        }
    }
}
