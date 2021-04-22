using Lexiconner.Domain.Dtos.UserDictionaries;
using System.Threading.Tasks;


namespace Lexiconner.Application.Services.Interfacse
{
    public interface IUserDictionaryService
    {
        Task<UserDictionaryDto> GetUserDictionaryAsync(string userId, string languageCode);
        Task<UserDictionaryDto> AddWordSetToUserDictionaryAsync(string userId, string languageCode, string wordSetId);
        Task<UserDictionaryDto> CreateUserDictionaryWordSetAsync(string userId, string languageCode, UserWordSetCreateDto dto);
        Task<UserDictionaryDto> UpdateUserDictionaryWordSetAsync(string userId, string languageCode, string userWordSetId, UserWordSetUpdateDto dto);
        Task<UserDictionaryDto> DeleteWordSetFromUserDictionaryAsync(string userId, string languageCode, string wordSetId);
    }
}
