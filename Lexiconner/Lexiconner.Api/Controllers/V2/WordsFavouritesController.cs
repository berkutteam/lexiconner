using Lexiconner.Application.Services.Interfacse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Lexiconner.Api.Controllers.V2
{
    [ApiController]
    [Authorize(policy: "DefaultWebApiAuth")]
    [EnableCors("DefaultApi")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/words")]
    public class WordsFavouritesController : ApiControllerBase
    {
        private readonly IWordsService _wordsService;

        public WordsFavouritesController(
            IWordsService wordsService
        )
        {
            _wordsService = wordsService;
        }

        [HttpPost("{id}/favourites")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddToFavourites([FromRoute]string id)
        {
            await _wordsService.AddToFavouritesAsync(GetUserId(), new List<string> { id }).ConfigureAwait(false);
            return StatusCodeBaseResponse();
        }

        [HttpDelete("{id}/favourites")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteFromFavourites([FromRoute]string id)
        {
            await _wordsService.DeleteFromFavouritesAsync(GetUserId(), new List<string> { id }).ConfigureAwait(false);
            return StatusCodeBaseResponse();
        }
    }
}
