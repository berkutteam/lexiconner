﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lexiconner.Api.DTOs;
using Lexiconner.Api.DTOs.StudyItems;
using Lexiconner.Api.Models;
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
    [Route("api/v{version:apiVersion}/[controller]")]
    public class StudyItemsController : ApiControllerBase
    {
        private readonly IMongoRepository _mongoRepository;
        private readonly IStudyItemsService _studyItemsService;
        private readonly IImageService _imageService;

        public StudyItemsController(
            IMongoRepository mongoRepository,
            IStudyItemsService studyItemsService,
            IImageService imageService
        )
        {
            _mongoRepository = mongoRepository;
            _studyItemsService = studyItemsService;
            _imageService = imageService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BaseApiResponseDto<PaginationResponseDto<StudyItemEntity>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] StudyItemsRequestDto dto)
        {
            var searchFilter = new StudyItemsSearchFilter(dto.Search, dto.IsFavourite);

            var result = await _studyItemsService.GetAllStudyItemsAsync(GetUserId(), dto.Offset, dto.Limit, searchFilter);
            return BaseResponse(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseApiResponseDto<StudyItemEntity>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get([FromRoute]string id)
        {
            var result = await _mongoRepository.GetOneAsync<StudyItemEntity>(x => x.Id == id && x.UserId == GetUserId());
            return BaseResponse(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseApiResponseDto<StudyItemEntity>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Post([FromBody] StudyItemEntity data)
        {
            data.UserId = GetUserId();

            // set image
            var imagesResult = await _imageService.FindImagesAsync(sourceLanguageCode: null, data.Title);

            if (imagesResult.Any())
            {
                var image = imagesResult.First();
                data.Image = new StudyItemImageEntity
                {
                    Url = image.Url,
                    Height = image.Height,
                    Width = image.Width,
                    Thumbnail = image.Thumbnail,
                    ThumbnailHeight = image.ThumbnailHeight,
                    ThumbnailWidth = image.ThumbnailWidth,
                    Base64Encoding = image.Base64Encoding,
                };
            }

            await _mongoRepository.AddAsync(data);
            return BaseResponse(data);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BaseApiResponseDto<StudyItemEntity>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Put([FromRoute]string id, [FromBody] StudyItemEntity data)
        {
            data.Id = id;
            data.UserId = GetUserId();

            // set image
            var imagesResult = await _imageService.FindImagesAsync(sourceLanguageCode: null, data.Title);

            if (imagesResult.Any())
            {
                var image = imagesResult.First();
                data.Image = new StudyItemImageEntity
                {
                    Url = image.Url,
                    Height = image.Height,
                    Width = image.Width,
                    Thumbnail = image.Thumbnail,
                    ThumbnailHeight = image.ThumbnailHeight,
                    ThumbnailWidth = image.ThumbnailWidth,
                    Base64Encoding = image.Base64Encoding,
                };
            }

            await _mongoRepository.UpdateAsync(data);
            return BaseResponse(data);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute]string id)
        {
            var existing = await _mongoRepository.GetOneAsync<StudyItemEntity>(x => x.Id == id && x.UserId == GetUserId());
            await _mongoRepository.DeleteAsync<StudyItemEntity>(x => x.Id == existing.Id);
            return StatusCodeBaseResponse();
        }
    }
}