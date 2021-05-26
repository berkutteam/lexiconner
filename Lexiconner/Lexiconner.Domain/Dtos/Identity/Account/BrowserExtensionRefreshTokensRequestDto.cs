using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.Identity.Account
{
    public class BrowserExtensionRefreshTokensRequestDto
    {
        public string IdentityToken { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ClientId { get; set; }
    }
}
