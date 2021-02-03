using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lexiconner.Api.Dtos.StudyItemsTrainings;
using Lexiconner.Api.DTOs;
using Lexiconner.Api.DTOs.StudyItemsTrainings;
using Lexiconner.Api.Models;
using Lexiconner.Api.Services;
using Lexiconner.Api.Services.Interfaces;
using Lexiconner.Application.Services;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lexiconner.Api.Controllers.V2
{
    [ApiController]
    [Authorize]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/studyitems/trainings")]
    public class StudyItemsTrainingsController : ApiControllerBase
    {
        private readonly IStudyItemsService _studyItemsService;

        public StudyItemsTrainingsController(
            IStudyItemsService studyItemsService
        )
        {
            _studyItemsService = studyItemsService;
        }

        [HttpGet("stats")]
        [ProducesResponseType(typeof(BaseApiResponseDto<TrainingsStatisticsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetTrainingStatistics()
        {
            var result = await _studyItemsService.GetTrainingStatisticsAsync(GetUserId());
            return BaseResponse(result);
        }

        [HttpPut("trained/{studyItemId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> MarkStudyItemAsTrained([FromRoute] string studyItemId)
        {
            await _studyItemsService.MarkStudyItemAsTrainedAsync(GetUserId(), studyItemId);
            return StatusCodeBaseResponse();
        }

        [HttpPut("not_trained/{studyItemId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> MarkStudyItemAsNotTrained([FromRoute] string studyItemId)
        {
            await _studyItemsService.MarkStudyItemAsNotTrainedAsync(GetUserId(), studyItemId);
            return StatusCodeBaseResponse();
        }


        #region Flashcards

        [HttpGet("flashcards")]
        [ProducesResponseType(typeof(BaseApiResponseDto<FlashCardsTrainingDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> FlashcardsTrainingStart([FromQuery]string collectionId, [FromQuery]int limit)
        {
            var result = await _studyItemsService.GetTrainingItemsForFlashCardsAsync(GetUserId(), collectionId, limit);
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
            await _studyItemsService.SaveTrainingResultsForFlashCardsAsync(GetUserId(), dto);
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
        public async Task<IActionResult> WordMeaningTrainingStart([FromQuery] string collectionId, [FromQuery] int limit)
        {
            var result = await _studyItemsService.GetTrainingItemsForWordMeaningAsync(GetUserId(), collectionId, limit);
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
            await _studyItemsService.SaveTrainingResultsForWordMeaningAsync(GetUserId(), dto);
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
        public async Task<IActionResult> MeaningWordTrainingStart([FromQuery] string collectionId, [FromQuery] int limit)
        {
            var result = await _studyItemsService.GetTrainingItemsForMeaningWordAsync(GetUserId(), collectionId, limit);
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
            await _studyItemsService.SaveTrainingResultsForMeaningWordAsync(GetUserId(), dto);
            return StatusCodeBaseResponse();
        }

        #endregion
    }
}
