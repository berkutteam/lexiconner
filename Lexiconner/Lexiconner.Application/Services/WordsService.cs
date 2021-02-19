using AutoMapper;
using Lexiconner.Application.ApiClients;
using Lexiconner.Application.ApiClients.Scrapers;
using Lexiconner.Application.Exceptions;
using Lexiconner.Application.Extensions;
using Lexiconner.Application.Helpers;
using Lexiconner.Application.Services;
using Lexiconner.Application.Services.Interfacse;
using Lexiconner.Application.Validation;
using Lexiconner.Domain.Attributes;
using Lexiconner.Domain.Config;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.StudyItems;
using Lexiconner.Domain.Dtos.Words;
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

namespace Lexiconner.Application.Services
{
    public class WordsService: IWordsService
    {
        private readonly IMapper _mapper;
        private readonly IDataRepository _dataRepository;
        private readonly IReversoContextScraper _reversoContextScraper;

        public WordsService(
            IMapper mapper,
            IDataRepository MongoDataRepository,
            IReversoContextScraper reversoContextScraper
        )
        {
            _mapper = mapper;
            _dataRepository = MongoDataRepository;
            _reversoContextScraper = reversoContextScraper;
        }

        public async Task<WordExamplesDto> GetWordExamplesAsync(string languageCode, string word)
        {
            var translationResult = await _reversoContextScraper.GetWordTranslationsAsync(
                sourceLanguageCode: languageCode,
                targetLanguageCode: LanguageConfig.GetLanguageByCode("ru").Iso639_1_Code, // any language as we need examples and don't translations actually
                word: word
            );

            var result = new WordExamplesDto()
            {
                LanguageCode = languageCode,
                Word = word,
                Examples = translationResult.Results.Select(x => x.SourceLanguageSentence),
            };
            return result;
        }
    }
}
