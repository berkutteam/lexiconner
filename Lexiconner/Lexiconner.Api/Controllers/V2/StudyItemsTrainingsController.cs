using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lexiconner.Api.DTOs.StudyItemsTrainings;
using Lexiconner.Api.Models;
using Lexiconner.Api.Models.RequestModels;
using Lexiconner.Api.Models.ResponseModels;
using Lexiconner.Api.Services;
using Lexiconner.Application.Services;
using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence.Repositories.Base;
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
        [ProducesResponseType(typeof(BaseApiResponseModel<TrainingsStatisticsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetTrainingStatistics()
        {
            var result = await _studyItemsService.GetTrainingStatistics(GetUserId());
            return BaseResponse(result);
        }


        #region Flashcards

        [HttpGet("flashcards")]
        [ProducesResponseType(typeof(BaseApiResponseModel<FlashCardsTrainingDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> FlashcardsStartTraining([FromQuery]int limit)
        {
            var result = await _studyItemsService.GetTrainingItemsForFlashCards(GetUserId(), limit);
            return BaseResponse(result);
        }

        [HttpPost("flashcards/save")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> FlashcardsSaveTrainingResults([FromBody]FlashCardsTrainingResultDto dto)
        {
            await _studyItemsService.SaveTrainingResultsForFlashCards(GetUserId(), dto);
            return StatusCodeBaseResponse();
        }

        #endregion
    }
}
