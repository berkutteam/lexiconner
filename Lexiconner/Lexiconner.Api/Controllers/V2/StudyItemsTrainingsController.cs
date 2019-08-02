using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> GetTrainingStatistics()
        {
            var result = await _studyItemsService.GetTrainingStatistics(GetUserId());
            return BaseResponse(result);
        }

        [HttpGet("flashcards/get")]
        public async Task<IActionResult> StartTraining()
        {
            var result = await _studyItemsService.GetTrainingItemsForFlashCards(GetUserId());
            return BaseResponse(result);
        }

        [HttpPost("flashcards/save")]
        public async Task<IActionResult> StartTraining([FromBody]FlashCardsTrainingResultDto dto)
        {
            await _studyItemsService.SaveTrainingResultsForFlashCards(GetUserId(), dto);
            return StatusCodeBaseResponse();
        }
    }
}
