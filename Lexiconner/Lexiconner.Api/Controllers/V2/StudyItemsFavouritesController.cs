using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lexiconner.Api.DTOs.StudyItemsTrainings;
using Lexiconner.Api.Models;
using Lexiconner.Api.Services;
using Lexiconner.Api.Services.Interfaces;
using Lexiconner.Application.Services;
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
    [Route("api/v{version:apiVersion}/studyitems")]
    public class StudyItemsFavouritesController : ApiControllerBase
    {
        private readonly IStudyItemsService _studyItemsService;

        public StudyItemsFavouritesController(
            IStudyItemsService studyItemsService
        )
        {
            _studyItemsService = studyItemsService;
        }

        [HttpPost("{id}/favourites")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddToFavourites([FromRoute]string id)
        {
            await _studyItemsService.AddToFavouritesAsync(GetUserId(), new List<string> { id });
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
            await _studyItemsService.DeleteFromFavouritesAsync(GetUserId(), new List<string> { id });
            return StatusCodeBaseResponse();
        }
    }
}
