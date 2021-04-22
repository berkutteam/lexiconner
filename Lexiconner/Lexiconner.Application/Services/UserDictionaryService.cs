using AutoMapper;
using Lexiconner.Api.Dtos.WordsTrainings;
using Lexiconner.Api.DTOs.WordsTrainings;
using Lexiconner.Application.ApiClients;
using Lexiconner.Application.ApiClients.Scrapers;
using Lexiconner.Application.Exceptions;
using Lexiconner.Application.Extensions;
using Lexiconner.Application.Helpers;
using Lexiconner.Application.Mappers;
using Lexiconner.Application.Services.Interfacse;
using Lexiconner.Application.Validation;
using Lexiconner.Domain.Config;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.General;
using Lexiconner.Domain.Dtos.UserDictionaries;
using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Entitites.General;
using Lexiconner.Domain.Enums;
using Lexiconner.Persistence.Repositories;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lexiconner.Application.Services
{
    public class UserDictionaryService : IUserDictionaryService
    {
        private readonly IMapper _mapper;
        private readonly IDataRepository _dataRepository;

        public UserDictionaryService(
            IMapper mapper,
            IDataRepository MongoDataRepository
        )
        {
            _mapper = mapper;
            _dataRepository = MongoDataRepository;
        }

        public async Task<UserDictionaryDto> GetUserDictionaryAsync(string userId, string languageCode)
        {
            var dictionary = await GetOrCreateUserDictionaryAsync(userId, languageCode);
            return _mapper.Map<UserDictionaryDto>(dictionary);
        }

        public async Task<UserDictionaryDto> AddWordSetToUserDictionaryAsync(string userId, string languageCode, string wordSetId)
        {
            var wordSet = await _dataRepository.GetOneAsync<WordSetEntity>(x => x.Id == wordSetId);
            if(wordSet == null)
            {
                throw new NotFoundException("Word set not found.");
            }
            if (wordSet.WordsLanguageCode != languageCode)
            {
                throw new NotFoundException($"Word set language is {wordSet.WordsLanguageCode} but you currently learning {languageCode}.");
            }

            // add word set to dictionary
            var dictionary = await GetOrCreateUserDictionaryAsync(userId, wordSet.WordsLanguageCode);
            var userWordSet = dictionary.AddWordSet(wordSet);
            CustomValidationHelper.Validate(dictionary);
            await _dataRepository.UpdateAsync(dictionary);

            // copy words from word set to user
            var newWords = wordSet.Words.Select(x =>
            {
                var result = _mapper.Map<WordEntity>(x);
                result.UserId = userId;
                result.UserDictionaryId = dictionary.Id;
                result.UserWordSetId = userWordSet.Id;
                result.SourceWordSetWordId = x.Id;
                return result;
            }).ToList();
            foreach (var item in newWords)
            {
                CustomValidationHelper.Validate(item);
            }
            await _dataRepository.AddManyAsync(newWords);

            return await GetUserDictionaryAsync(userId, wordSet.WordsLanguageCode);
        }

        #region Private

        private async Task<UserDictionaryEntity> GetOrCreateUserDictionaryAsync(string userId, string languageCode)
        {
            var dictionary = await _dataRepository.GetOneAsync<UserDictionaryEntity>(x => x.UserId == userId && x.WordsLanguageCode == languageCode);
            if (dictionary == null)
            {
                dictionary = new UserDictionaryEntity()
                {
                    UserId = userId,
                    WordsLanguageCode = languageCode,
                };
                CustomValidationHelper.Validate(dictionary);
                await _dataRepository.AddAsync(dictionary);
            }
            return dictionary;
        }

        #endregion
    }
}
