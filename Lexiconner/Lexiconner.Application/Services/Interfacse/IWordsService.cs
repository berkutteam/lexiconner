using Lexiconner.Api.Dtos.WordsTrainings;
using Lexiconner.Api.DTOs.WordsTrainings;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.General;
using Lexiconner.Domain.Dtos.Words;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Application.Services.Interfacse
{
    public interface IWordsService
    {
        Task<PaginationResponseDto<WordDto>> GetAllWordsAsync(
            string userId,
            string languageCode,
            int offset, 
            int limit, 
            string collectionId = null,
            string search = null,
            bool? isFavourite = null,
            bool isShuffle = false,
            bool? isTrained = null,
            string userWordSetId = null
        );
        Task<IEnumerable<WordDto>> BrowserExtensionGetLastAddedWordsAsync(string userId, string wordLanguageCode, int limit);
        Task<WordDto> GetWordAsync(string userId, string wordId);
        Task<WordDto> CreateWordAsync(string userId, WordCreateDto createDto);
        Task<WordDto> BrowserExtensionCreateWordAsync(string userId, BrowserExtensionWordCreateDto createDto);
        Task<WordDto> UpdateWordAsync(string userId, string wordId, WordUpdateDto updateDto);
        Task DeleteWord(string userId, string sutyItemId);
        Task<PaginationResponseDto<GeneralImageDto>> FindWordImagesAsync(string userId, string wordId);
        Task<WordDto> UpdateWordImagesAsync(string userId, string wordId, UpdateWordImagesDto dto);
        Task<WordMeaningsDto> GetWordMeaningsAsync(string word, string wordLanguageCode, string meaningLanguageCode);
        Task<WordExamplesDto> GetWordExamplesAsync(string languageCode, string word);
        Task<WordPronunciationAudioDto> GetWordPronunciationAudioAsync(string languageCode, string word);

        Task AddToFavouritesAsync(string userId, IEnumerable<string> itemIds);
        Task DeleteFromFavouritesAsync(string userId, IEnumerable<string> itemIds);
    }
}
