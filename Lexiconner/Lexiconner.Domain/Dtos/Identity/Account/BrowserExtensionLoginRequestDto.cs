using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.Identity.Account
{
    public class BrowserExtensionLoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ExtensionVersion { get; set; }
    }
}
