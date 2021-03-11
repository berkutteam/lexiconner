using Lexiconner.Domain.Dto.CustomCollections;
using Lexiconner.Domain.Dtos.CustomCollections;
using System.Threading.Tasks;

namespace Lexiconner.Application.Services.Interfacse
{
    public interface ICustomCollectionsService
    {
        Task<CustomCollectionsAllResponseDto> GetAllCustomCollectionsAsync(string userId);
        Task<CustomCollectionsAllResponseDto> CreateCustomCollectionAsync(string userId, CustomCollectionCreateDto createDto);
        Task<CustomCollectionsAllResponseDto> UpdateCustomCollectionAsync(string userId, string collectionId, CustomCollectionUpdateDto updateDto);
        Task<CustomCollectionsAllResponseDto> MarkCustomCollectionAsSelectedAsync(string userId, string collectionId);
        Task<CustomCollectionsAllResponseDto> DeleteCustomCollectionAsync(string userId, string customCollectionId, bool isDeleteItems);
        Task<CustomCollectionsAllResponseDto> DuplicateCustomCollectionAsync(string userId, string collectionId);
    }
}
