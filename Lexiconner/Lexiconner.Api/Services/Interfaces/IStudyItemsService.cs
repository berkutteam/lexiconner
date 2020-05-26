using Lexiconner.Api.DTOs;
using Lexiconner.Api.DTOs.StudyItemsTrainings;
using Lexiconner.Api.Mappers;
using Lexiconner.Api.Models;
using Lexiconner.Application.Helpers;
using Lexiconner.Application.Services;
using Lexiconner.Domain.Attributes;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.StudyItems;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Enums;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence.Repositories.MongoDb;
using LinqKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace Lexiconner.Api.Services.Interfaces
{
    public interface IStudyItemsService
    {
        Task<PaginationResponseDto<StudyItemDto>> GetAllStudyItemsAsync(string userId, int offset, int limit, StudyItemsSearchFilter searchFilter = null, string collectionId = null);
        Task<StudyItemDto> CreateStudyItemAsync(string userId, StudyItemCreateDto createDto);
        Task<StudyItemDto> UpdateStudyItemAsync(string userId, string studyItemId, StudyItemUpdateDto updateDto);
        Task DeleteStudyItem(string userId, string sutyItemId);

        Task<TrainingsStatisticsDto> GetTrainingStatisticsAsync(string userId);
        Task<FlashCardsTrainingDto> GetTrainingItemsForFlashCardsAsync(string userId, string collectionId, int limit);
        Task SaveTrainingResultsForFlashCardsAsync(string userId, FlashCardsTrainingResultDto results);

        Task AddToFavouritesAsync(string userId, IEnumerable<string> itemIds);
        Task DeleteFromFavouritesAsync(string userId, IEnumerable<string> itemIds);
    }
}
