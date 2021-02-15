using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lexiconner.Api.DTOs;
using Lexiconner.Api.Mappers;
using Lexiconner.Api.Models;
using Lexiconner.Api.Services;
using Lexiconner.Api.Services.Interfaces;
using Lexiconner.Application.Exceptions;
using Lexiconner.Application.Services;
using Lexiconner.Application.Services.Interfacse;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.StudyItems;
using Lexiconner.Domain.Dtos.UserFilms;
using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence.Repositories.MongoDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lexiconner.Api.Controllers.V2
{
    [ApiController]
    [Authorize]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WordsController : ApiControllerBase
    {
        private readonly IWordsService _wordsService;

        public WordsController(
            IDataRepository dataRepository,
            IWordsService wordsService
        )
        {
            _wordsService = wordsService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BaseApiResponseDto<WordExamplesDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetWordExamples([FromQuery] string languageCode, [FromQuery] string word)
        {
            var result = await _wordsService.GetWordExamplesAsync(languageCode, word);
            return BaseResponse(result);
        }
    }
}
