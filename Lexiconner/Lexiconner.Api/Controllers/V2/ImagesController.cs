using Lexiconner.Application.Services;
using Lexiconner.Application.Services.Interfacse;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.General;
using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Lexiconner.Api.Controllers.V2
{
    [ApiController]
    [Authorize(policy: "DefaultWebApiAuth")]
    [EnableCors("DefaultApi")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ImagesController : ApiControllerBase
    {
        private readonly IImageService _imageService;

        public ImagesController(
            IImageService imageService
        )
        {
            _imageService = imageService;
        }

        [HttpPost("find/by-language")]
        [ProducesResponseType(typeof(BaseApiResponseDto<PaginationResponseDto<GeneralImageDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> FindNextWordImages([FromQuery] string languageCode, [FromQuery] string search, [FromQuery] int limit)
        {
            var result = await _imageService.FindImagesByLanguageCodeAsync(languageCode, search, limit);
            return BaseResponse(result);
        }
    }
}
