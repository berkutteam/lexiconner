using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexiconner.Api.Models;
using Lexiconner.Api.Models.RequestModels;
using Lexiconner.Api.Models.ResponseModels;
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

        public StudyItemsController(IMongoRepository mongoRepository)
        {
            _mongoRepository = mongoRepository;
        }

        [HttpGet]
        public async Task<BaseApiResponseModel<GetAllResponseModel<StudyItemEntity>>> GetAll([FromQuery] GetAllRequestModel data)
        {
            var itemsTask = _mongoRepository.GetAllAsync<StudyItemEntity>(data.Offset.GetValueOrDefault(0), data.Limit.GetValueOrDefault(10), data.Search);
            var totalTask = _mongoRepository.CountAllAsync<StudyItemEntity>();
            await Task.WhenAll(itemsTask, totalTask);
            var result = new GetAllResponseModel<StudyItemEntity>
            {
                Items = await itemsTask,
                TotalCount = await totalTask,
            };
            return BaseJsonResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<BaseApiResponseModel<StudyItemEntity>> Get(string id)
        {
            var result = await _mongoRepository.GetOneAsync<StudyItemEntity>(x => x.Id == id);
            return BaseJsonResponse(result);
        }

        [HttpPost]
        public async Task<BaseApiResponseModel<StudyItemEntity>> Post([FromBody] StudyItemEntity data)
        {
            await _mongoRepository.AddAsync(data);
            return BaseJsonResponse(data);
        }

        [HttpPut("{id}")]
        public async Task<BaseApiResponseModel<StudyItemEntity>> Put(string id, [FromBody] StudyItemEntity data)
        {
            await _mongoRepository.UpdateAsync(data);
            return BaseJsonResponse(data);
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var existing = await _mongoRepository.GetOneAsync<StudyItemEntity>(x => x.Id == id);
            await _mongoRepository.DeleteAsync<StudyItemEntity>(x => x.Id == existing.Id);
        }
    }
}