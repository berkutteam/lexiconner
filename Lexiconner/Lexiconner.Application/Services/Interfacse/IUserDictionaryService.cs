using Lexiconner.Domain.Dtos.UserDictionaries;
using System.Threading.Tasks;


namespace Lexiconner.Application.Services.Interfacse
{
    public interface IUserDictionaryService
    {
        Task<UserDictionaryDto> GetUserDictionaryAsync(string userId, string languageCode);
        Task<UserDictionaryDto> AddWordSetToUserDictionaryAsync(string userId, string languageCode, string wordSetId);
    }
}
