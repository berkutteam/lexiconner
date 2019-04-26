using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexiconner.Api.Models;
using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.Repositories;
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
        public async Task<BaseApiResponseModel<IEnumerable<StudyItem>>> GetAll()
        {
            var result = await _studyItemJsonRepository.GetAll();
            return BaseJsonResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<BaseApiResponseModel<StudyItem>> Get(string id)
        {
            var result = await _studyItemJsonRepository.GetById(id);
            return BaseJsonResponse(result);
        }

        [HttpPost]
        public async Task<BaseApiResponseModel<StudyItem>> Post([FromBody] StudyItem data)
        {
            await _studyItemJsonRepository.Add(data);
            return BaseJsonResponse(data);
        }

        [HttpPut("{id}")]
        public async Task<BaseApiResponseModel<StudyItem>> Put(string id, [FromBody] StudyItem data)
        {
            await _studyItemJsonRepository.Update(data);
            return BaseJsonResponse(data);
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var existing = await _studyItemJsonRepository.GetById(id);
            await _studyItemJsonRepository.Delete(existing);
        }
    }
}