using Lexiconner.Api.Mappers;
using Lexiconner.Api.Services.Interfaces;
using Lexiconner.Application.Exceptions;
using Lexiconner.Domain.Dto.CustomCollections;
using Lexiconner.Domain.DTOs.CustomCollections;
using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.Services.Interfaces
{
    public interface ICustomCollectionsService
    {
        Task<CustomCollectionsAllResponseDto> GetAllCustomCollectionsAsync(string userId);
        Task<CustomCollectionsAllResponseDto> CreateCustomCollectionAsync(string userId, CustomCollectionCreateDto createDto);
        Task<CustomCollectionsAllResponseDto> UpdateCustomCollectionAsync(string userId, string collectionId, CustomCollectionUpdateDto updateDto);
        Task<CustomCollectionsAllResponseDto> DeleteCustomCollectionAsync(string userId, string customCollectionId);
        Task<CustomCollectionsAllResponseDto> DuplicateCustomCollectionAsync(string userId, string collectionId);
    }
}
