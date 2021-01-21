using Lexiconner.Api.DTOs.Misc;
using Lexiconner.Domain.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.Controllers.V2
{
    [ApiController]
    [AllowAnonymous]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class LanguagesController : ApiControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            return BaseResponse(LanguageConfig.SupportedLanguages.Select(x => new LanguageDto()
            {
                LanguageFamily = x.LanguageFamily,
                IsoLanguageName = x.IsoLanguageName,
                NativeName = x.NativeName,
                Iso639_1_Code = x.Iso639_1_Code,
            }).ToList());
        }
    }
}
