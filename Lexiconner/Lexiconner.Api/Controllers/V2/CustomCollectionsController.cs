using Lexiconner.Api.DTOs.Misc;
using Lexiconner.Api.Services.Interfaces;
using Lexiconner.Domain.Config;
using Lexiconner.Domain.DTOs.CustomCollections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Lexiconner.Api.Controllers.V2
{
    // TODO: add tests
    [ApiController]
    [Authorize]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/customcollections")]
    public class CustomCollectionsController : ApiControllerBase
    {
        private readonly ICustomCollectionsService _customCollectionsService;

        public CustomCollectionsController(
            ICustomCollectionsService customCollectionsService
        )
        {
            _customCollectionsService = customCollectionsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _customCollectionsService.GetAllCustomCollectionsAsync(GetUserId());
            return BaseResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CustomCollectionCreateDto dto)
        {
            var result = await _customCollectionsService.CreateCustomCollectionAsync(GetUserId(), dto);
            return BaseResponse(result);
        }

        [HttpPut]
        [Route("{collectionId}")]
        public async Task<IActionResult> Update([FromRoute]string collectionId, [FromBody]CustomCollectionUpdateDto dto)
        {
            var result = await _customCollectionsService.UpdateCustomCollectionAsync(GetUserId(), collectionId, dto);
            return BaseResponse(result);
        }

        [HttpDelete]
        [Route("{collectionId}")]
        public async Task<IActionResult> Delete([FromRoute]string collectionId, [FromQuery]bool isDeleteItems)
        {
            var result = await _customCollectionsService.DeleteCustomCollectionAsync(GetUserId(), collectionId, isDeleteItems);
            return BaseResponse(result);
        }

        // TODO: delete nested items?
        [HttpPost]
        [Route("{collectionId}/duplicate")]
        public async Task<IActionResult> Duplicate([FromRoute]string collectionId)
        {
            var result = await _customCollectionsService.DuplicateCustomCollectionAsync(GetUserId(), collectionId);
            return BaseResponse(result);
        }
    }
}
