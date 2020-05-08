using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexiconner.Api.DTOs;
using Lexiconner.Api.Models;
using Lexiconner.Domain.Dtos;
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
        public StudyItemsController()
        {
        }

        [HttpGet]
        public async Task<BaseApiResponseDto<IEnumerable<StudyItemEntity>>> GetAll()
        {
            throw new InvalidOperationException("Derecated! Use v2");
        }

        [HttpGet("{id}")]
        public async Task<BaseApiResponseDto<StudyItemEntity>> Get(string id)
        {
            throw new InvalidOperationException("Derecated! Use v2");
        }

        [HttpPost]
        public async Task<BaseApiResponseDto<StudyItemEntity>> Post([FromBody] StudyItemEntity data)
        {
            throw new InvalidOperationException("Derecated! Use v2");
        }

        [HttpPut("{id}")]
        public async Task<BaseApiResponseDto<StudyItemEntity>> Put(string id, [FromBody] StudyItemEntity data)
        {
            throw new InvalidOperationException("Derecated! Use v2");
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            throw new InvalidOperationException("Derecated! Use v2");
        }
    }
}