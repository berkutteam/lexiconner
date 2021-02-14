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
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.StudyItems;
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
    public class StudyItemsController : ApiControllerBase
    {
        private readonly IDataRepository _dataRepository;
        private readonly IStudyItemsService _studyItemsService;
        private readonly IImageService _imageService;

        public StudyItemsController(
            IDataRepository dataRepository,
            IStudyItemsService studyItemsService,
            IImageService imageService
        )
        {
            _dataRepository = dataRepository;
            _studyItemsService = studyItemsService;
            _imageService = imageService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BaseApiResponseDto<PaginationResponseDto<StudyItemDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] StudyItemsRequestDto dto)
        {
            var searchFilter = new StudyItemsSearchFilterModel(dto.Search, dto.IsFavourite, dto.IsShuffle);
            var result = await _studyItemsService.GetAllStudyItemsAsync(GetUserId(), dto.Offset, dto.Limit, searchFilter, dto.CollectionId);
            return BaseResponse(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseApiResponseDto<StudyItemDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get([FromRoute]string id)
        {
            var result = await _studyItemsService.GetStudyItemAsync(GetUserId(), id);
            return BaseResponse(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseApiResponseDto<StudyItemDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromBody] StudyItemCreateDto data)
        {
            var result = await _studyItemsService.CreateStudyItemAsync(GetUserId(), data);
            return BaseResponse(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BaseApiResponseDto<StudyItemDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Put([FromRoute]string id, [FromBody] StudyItemUpdateDto data)
        {
            var result = await _studyItemsService.UpdateStudyItemAsync(GetUserId(), id, data);
            return BaseResponse(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute]string id)
        {
            await _studyItemsService.DeleteStudyItem(GetUserId(), id);
            return StatusCodeBaseResponse();
        }
    }
}