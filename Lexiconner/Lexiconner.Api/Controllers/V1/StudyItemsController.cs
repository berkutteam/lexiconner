using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexiconner.Api.Models;
using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence.Repositories.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lexiconner.Api.Controllers.V1
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class StudyItemsController : ApiControllerBase
    {
        private readonly IStudyItemJsonRepository _studyItemJsonRepository;

        public StudyItemsController(IStudyItemJsonRepository studyItemJsonRepository)
        {
            _studyItemJsonRepository = studyItemJsonRepository;
        }

        [HttpGet]
        public async Task<BaseApiResponseModel<IEnumerable<StudyItemEntity>>> GetAll()
        {
            var result = await _studyItemJsonRepository.GetAllAsync<StudyItemEntity>();
            return BaseJsonResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<BaseApiResponseModel<StudyItemEntity>> Get(string id)
        {
            var result = await _studyItemJsonRepository.GetOneAsync<StudyItemEntity>(x => x.Id == id);
            return BaseJsonResponse(result);
        }

        [HttpPost]
        public async Task<BaseApiResponseModel<StudyItemEntity>> Post([FromBody] StudyItemEntity data)
        {
            await _studyItemJsonRepository.AddAsync(data);
            return BaseJsonResponse(data);
        }

        [HttpPut("{id}")]
        public async Task<BaseApiResponseModel<StudyItemEntity>> Put(string id, [FromBody] StudyItemEntity data)
        {
            await _studyItemJsonRepository.UpdateAsync(data);
            return BaseJsonResponse(data);
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var existing = await _studyItemJsonRepository.GetOneAsync<StudyItemEntity>(x => x.Id == id);
            await _studyItemJsonRepository.DeleteAsync<StudyItemEntity>(x => x.Id == existing.Id);
        }
    }
}