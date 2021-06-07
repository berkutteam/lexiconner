using Lexiconner.Application.Services;
using Lexiconner.Application.Services.Interfacse;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.General;
using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Lexiconner.Api.Controllers.V2.BrowserExtension
{
    [ApiController]
    [Authorize(policy: "BrowserExtensionWebApiAuth")]
    [EnableCors("BrowserExtensionApi")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/browser-extension/words")]
    public class BrowserExtensionWordsControler : ApiControllerBase
    {
        private readonly IWordsService _wordsService;

        public BrowserExtensionWordsControler(
            IWordsService wordsService
        )
        {
            _wordsService = wordsService;
        }

        [HttpGet("meanings")]
        [ProducesResponseType(typeof(BaseApiResponseDto<WordMeaningsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetWordMeanings([FromQuery] WordMeaningsRequestDto dto)
        {
            var result = await _wordsService.GetWordMeaningsAsync(dto.Word, dto.WordLanguageCode, dto.MeaningLanguageCode);
            return BaseResponse(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseApiResponseDto<WordDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BrowserExtensionWordCreateDto data)
        {
            var result = await _wordsService.BrowserExtensionCreateWordAsync(GetUserId(), data);
            return BaseResponse(result);
        }
    }
}
