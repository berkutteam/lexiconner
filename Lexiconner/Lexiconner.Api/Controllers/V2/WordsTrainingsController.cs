using Lexiconner.Api.Dtos.WordsTrainings;
using Lexiconner.Api.DTOs.WordsTrainings;
using Lexiconner.Application.Services.Interfacse;
using Lexiconner.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Lexiconner.Api.Controllers.V2
{
    [ApiController]
    [Authorize]
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
        public async Task<IActionResult> GetTrainingStatistics()
        {
            var result = await _wordTrainingsService.GetTrainingStatisticsAsync(GetUserId());
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
        public async Task<IActionResult> FlashcardsTrainingStart([FromQuery]string collectionId, [FromQuery]int limit)
        {
            var result = await _wordTrainingsService.GetTrainingItemsForFlashCardsAsync(GetUserId(), collectionId, limit);
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
        public async Task<IActionResult> WordMeaningTrainingStart([FromQuery] string collectionId, [FromQuery] int limit)
        {
            var result = await _wordTrainingsService.GetTrainingItemsForWordMeaningAsync(GetUserId(), collectionId, limit);
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
        public async Task<IActionResult> MeaningWordTrainingStart([FromQuery] string collectionId, [FromQuery] int limit)
        {
            var result = await _wordTrainingsService.GetTrainingItemsForMeaningWordAsync(GetUserId(), collectionId, limit);
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

        #region Meaning-Word

        [HttpGet("matchwords")]
        [ProducesResponseType(typeof(BaseApiResponseDto<MatchWordsTrainingDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> MatchWordsTrainingStart([FromQuery] string collectionId)
        {
            var result = await _wordTrainingsService.GetTrainingItemsForMatchWordsAsync(GetUserId(), collectionId);
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
    }
}
