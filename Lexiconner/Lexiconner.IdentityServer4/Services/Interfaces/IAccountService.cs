using Lexiconner.Domain.Dtos.Identity.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.IdentityServer4.Services.Interfaces
{
    public interface IAccountService
    {
        Task<BrowserExtensionLoginResponseDto> BrowserExtensionLoginAsync(BrowserExtensionLoginRequestDto dto);
    }
}
