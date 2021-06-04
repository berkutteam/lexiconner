using Lexiconner.Application.Services;
using Lexiconner.Application.Services.Interfacse;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.General;
using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Dtos.WordSets;
using Lexiconner.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Lexiconner.Api.Controllers.V2
{
    [ApiController]
    [Authorize]
    [EnableCors("DefaultApi")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WordSetsController : ApiControllerBase
    {
        private readonly IWordSetsService _wordSetsService;

        public WordSetsController(
            IWordSetsService wordSetsService
        )
        {
            _wordSetsService = wordSetsService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BaseApiResponseDto<PaginationResponseDto<WordSetDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] WordSetsRequestDto dto)
        {
            var result = await _wordSetsService.GetAllWordSetsAsync(dto.LanguageCode, dto.Offset, dto.Limit, dto.Search);
            return BaseResponse(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseApiResponseDto<WordSetDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromBody] WordSetCreateDto dto)
        {
            var result = await _wordSetsService.CreateWordSetAsync(GetUserId(), dto);
            return BaseResponse(result);
        }
    }
}
