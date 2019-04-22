using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexiconner.Api.Models;
using Lexiconner.Api.Models.RequestModels;
using Lexiconner.Api.Models.ResponseModels;
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
    [Route("api/v{version:apiVersion}/[controller]")]
    public class StudyItemsController : ApiControllerBase
    {
        private readonly IStudyItemRepository _studyItemRepository;

        public StudyItemsController(IStudyItemRepository studyItemRepository)
        {
            _studyItemRepository = studyItemRepository;
        }

        [HttpGet]
        public async Task<BaseApiResponseModel<GetAllResponseModel<StudyItem>>> GetAll([FromQuery] GetAllRequestModel data)
        {
            var itemsTask = _studyItemRepository.GetAll(data.Offset.GetValueOrDefault(0), data.Limit.GetValueOrDefault(10), data.Search);
            var totalTask = _studyItemRepository.CountAll();
            await Task.WhenAll(itemsTask, totalTask);
            var result = new GetAllResponseModel<StudyItem>
            {
                Items = await itemsTask,
                TotalCount = await totalTask,
            };
            return BaseJsonResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<BaseApiResponseModel<StudyItem>> Get(string id)
        {
            var result = await _studyItemRepository.GetById(id);
            return BaseJsonResponse(result);
        }

        [HttpPost]
        public async Task<BaseApiResponseModel<StudyItem>> Post([FromBody] StudyItem data)
        {
            await _studyItemRepository.Add(data);
            return BaseJsonResponse(data);
        }

        [HttpPut("{id}")]
        public async Task<BaseApiResponseModel<StudyItem>> Put(string id, [FromBody] StudyItem data)
        {
            await _studyItemRepository.Update(data);
            return BaseJsonResponse(data);
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var existing = await _studyItemRepository.GetById(id);
            await _studyItemRepository.Delete(existing);
        }
    }
}