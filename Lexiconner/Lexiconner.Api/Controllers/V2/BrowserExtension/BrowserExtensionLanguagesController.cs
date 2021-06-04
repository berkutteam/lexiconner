using Lexiconner.Api.DTOs.Misc;
using Lexiconner.Domain.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Lexiconner.Api.Controllers.V2.BrowserExtension
{
    [ApiController]
    [AllowAnonymous]
    [EnableCors("BrowserExtensionApi")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/browser-extension/languages")]
    public class BrowserExtensionLanguagesController : ApiControllerBase
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
                CountryIsoAlpha2Code = x.CountryIsoAlpha2Code,
            }).ToList());
        }
    }
}
