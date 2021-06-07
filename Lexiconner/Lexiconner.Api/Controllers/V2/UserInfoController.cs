using Lexiconner.Application.Services;
using Lexiconner.Application.Services.Interfacse;
using Lexiconner.Domain.Dtos;
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
    public class UserInfoController : ApiControllerBase
    {
        private readonly IWordsService _wordsService;

        public UserInfoController(
            IWordsService wordsService
        )
        {
            _wordsService = wordsService;
        }
    }
}
