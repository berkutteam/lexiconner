using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Application.ApiClients
{
    public interface IGoogleTranslateApiClient
    {

    }

    /// <summary>
    /// The free tier is only available for Translation API v3.
    /// </summary>
    public class GoogleTranslateApiClient : IGoogleTranslateApiClient
    {
        private const int FreeCharactersLimitV3 = 500_000;

        public async Task<string> Translate(string text, string language)
        {
            return "";
        }
    }
}
