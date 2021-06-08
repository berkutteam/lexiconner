using Lexiconner.Domain.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Application.Services.Interfacse
{
    public interface IUsersService
    {
        Task<UserDto> GetUserAsync(string userId);
        Task<UserDto> UpdateProfileAsync(string userId, ProfileUpdateDto dto);
        Task<UserDto> SelectLearningLanguageAsync(string userId, string languageCode);
        Task<UserDto> BrowserExtensionSelectLearningLanguageAsync(string userId, string languageCode);
    }
}
