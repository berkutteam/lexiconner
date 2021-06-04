﻿using Lexiconner.Api.Dtos.WordsTrainings;
using Lexiconner.Api.DTOs.WordsTrainings;
using Lexiconner.Application.Services.Interfacse;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.WordTrainings;
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
    [Route("api/v{version:apiVersion}/words/trainings")]
    public class WordsTrainingsController : ApiControllerBase
    {
        private readonly IWordsService _wordsService;
        private readonly IWordTrainingsService _wordTrainingsService;

        public WordsTrainingsController(
            IWordsService wordsService,
            IWordTrainingsService wordTrainingsService
        )
        {
            _wordsService = wordsService;
            _wordTrainingsService = wordTrainingsService;
        }

        [HttpGet("stats")]
        [ProducesResponseType(typeof(BaseApiResponseDto<TrainingsStatisticsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetTrainingStatistics([FromQuery] string userWordSetId)
        {
            var result = await _wordTrainingsService.GetTrainingStatisticsAsync(GetUserId(), userWordSetId);
            return BaseResponse(result);
        }

        [HttpPut("trained/{wordId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> MarkWordAsTrained([FromRoute] string wordId)
        {
            await _wordTrainingsService.MarkWordAsTrainedAsync(GetUserId(), wordId);
            return StatusCodeBaseResponse();
        }

        [HttpPut("not_trained/{wordId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> MarkWordAsNotTrained([FromRoute] string wordId)
        {
            await _wordTrainingsService.MarkWordAsNotTrainedAsync(GetUserId(), wordId);
            return StatusCodeBaseResponse();
        }


        #region Flashcards

        [HttpGet("flashcards")]
        [ProducesResponseType(typeof(BaseApiResponseDto<FlashCardsTrainingDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> FlashcardsTrainingStart([FromQuery]string collectionId, [FromQuery] string userWordSetId, [FromQuery]int limit)
        {
            var result = await _wordTrainingsService.GetTrainingItemsForFlashCardsAsync(GetUserId(), collectionId, userWordSetId, limit);
            return BaseResponse(result);
        }

        [HttpPost("flashcards/save")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> FlashcardsTrainingSave([FromBody]FlashCardsTrainingResultDto dto)
        {
            await _wordTrainingsService.SaveTrainingResultsForFlashCardsAsync(GetUserId(), dto);
            return StatusCodeBaseResponse();
        }

        #endregion

        #region Word-Meaning

        [HttpGet("wordmeaning")]
        [ProducesResponseType(typeof(BaseApiResponseDto<WordMeaningTrainingDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> WordMeaningTrainingStart([FromQuery] string collectionId, [FromQuery] string userWordSetId, [FromQuery] int limit)
        {
            var result = await _wordTrainingsService.GetTrainingItemsForWordMeaningAsync(GetUserId(), collectionId, userWordSetId, limit);
            return BaseResponse(result);
        }

        [HttpPost("wordmeaning/save")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> WordMeaningTrainingSave([FromBody] WordMeaningTrainingResultDto dto)
        {
            await _wordTrainingsService.SaveTrainingResultsForWordMeaningAsync(GetUserId(), dto);
            return StatusCodeBaseResponse();
        }

        #endregion

        #region Meaning-Word

        [HttpGet("meaningword")]
        [ProducesResponseType(typeof(BaseApiResponseDto<MeaningWordTrainingDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> MeaningWordTrainingStart([FromQuery] string collectionId, [FromQuery] string userWordSetId, [FromQuery] int limit)
        {
            var result = await _wordTrainingsService.GetTrainingItemsForMeaningWordAsync(GetUserId(), collectionId, userWordSetId, limit);
            return BaseResponse(result);
        }

        [HttpPost("meaningword/save")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> MeaningWordTrainingSave([FromBody] MeaningWordTrainingResultDto dto)
        {
            await _wordTrainingsService.SaveTrainingResultsForMeaningWordAsync(GetUserId(), dto);
            return StatusCodeBaseResponse();
        }

        #endregion

        #region Match words

        [HttpGet("matchwords")]
        [ProducesResponseType(typeof(BaseApiResponseDto<MatchWordsTrainingDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> MatchWordsTrainingStart([FromQuery] string collectionId, [FromQuery] string userWordSetId)
        {
            var result = await _wordTrainingsService.GetTrainingItemsForMatchWordsAsync(GetUserId(), collectionId, userWordSetId);
            return BaseResponse(result);
        }

        [HttpPost("matchwords/save")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> MatchWordsTrainingSave([FromBody] MatchWordsTrainingResultDto dto)
        {
            await _wordTrainingsService.SaveTrainingResultsForMatchWordsAsync(GetUserId(), dto);
            return StatusCodeBaseResponse();
        }

        #endregion

        #region Build words

        [HttpGet("buildwords")]
        [ProducesResponseType(typeof(BaseApiResponseDto<BuildWordsTrainingDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> BuildWordsTrainingStart([FromQuery] string collectionId, [FromQuery] string userWordSetId, [FromQuery] int limit)
        {
            var result = await _wordTrainingsService.GetTrainingItemsForBuildWordsAsync(GetUserId(), collectionId, userWordSetId, limit);
            return BaseResponse(result);
        }

        [HttpPost("buildwords/save")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> BuildWordsTrainingSave([FromBody] BuildWordsTrainingResultDto dto)
        {
            await _wordTrainingsService.SaveTrainingResultsForBuildWordsAsync(GetUserId(), dto);
            return StatusCodeBaseResponse();
        }

        #endregion

        #region Listen words

        [HttpGet("listenwords")]
        [ProducesResponseType(typeof(BaseApiResponseDto<ListenWordsTrainingDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ListenWordsTrainingStart([FromQuery] string collectionId, [FromQuery] string userWordSetId, [FromQuery] int limit)
        {
            var result = await _wordTrainingsService.GetTrainingItemsForListenWordsAsync(GetUserId(), collectionId, userWordSetId, limit);
            return BaseResponse(result);
        }

        [HttpPost("listenwords/save")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ListenWordsTrainingSave([FromBody] ListenWordsTrainingResultDto dto)
        {
            await _wordTrainingsService.SaveTrainingResultsForListenWordsAsync(GetUserId(), dto);
            return StatusCodeBaseResponse();
        }

        #endregion
    }
}
