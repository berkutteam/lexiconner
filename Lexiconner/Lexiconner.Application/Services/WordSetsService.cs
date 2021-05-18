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
using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Dtos.WordSets;
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
    public class WordSetsService : IWordSetsService
    {
        private readonly IMapper _mapper;
        private readonly IDataRepository _dataRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IImageService _imageService;
        private readonly ITwinwordWordDictionaryApiClient _twinwordWordDictionaryApiClient;
        private readonly IReversoContextScraper _reversoContextScraper;
        private readonly IOxfordLearnersDictionariesScrapper _oxfordLearnersDictionariesScrapper;

        public WordSetsService(
            IMapper mapper,
            IDataRepository MongoDataRepository,
            IHttpClientFactory httpClientFactory,
            IImageService imageService,
            ITwinwordWordDictionaryApiClient twinwordWordDictionaryApiClient,
            IReversoContextScraper reversoContextScraper,
            IOxfordLearnersDictionariesScrapper oxfordLearnersDictionariesScrapper
        )
        {
            _mapper = mapper;
            _dataRepository = MongoDataRepository;
            _httpClientFactory = httpClientFactory;
            _imageService = imageService;
            _twinwordWordDictionaryApiClient = twinwordWordDictionaryApiClient;
            _reversoContextScraper = reversoContextScraper;
            _oxfordLearnersDictionariesScrapper = oxfordLearnersDictionariesScrapper;
        }

        public async Task<PaginationResponseDto<WordSetDto>> GetAllWordSetsAsync(
           string languageCode,
           int offset,
           int limit,
           string search = null
        )
        {
            var predicate = PredicateBuilder.New<WordSetEntity>(x => x.WordsLanguageCode == languageCode);

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLowerInvariant();
                predicate.And(x => x.Name.Contains(search));
            }

            var totalTask = _dataRepository.CountAllAsync<WordSetEntity>(predicate);
            var total = await totalTask;

            var items = await _dataRepository.GetManyAsync<WordSetEntity>(predicate, offset, limit);

            var result = new PaginationResponseDto<WordSetDto>
            {

                Items = _mapper.Map<IEnumerable<WordSetDto>>(items),
                Pagination = new PaginationInfoDto()
                {
                    TotalCount = total,
                    ReturnedCount = items.Count(),
                    Offset = offset,
                    Limit = limit,
                }
            };

            return result;
        }

        public async Task<WordSetDto> CreateWordSetAsync(string userId, WordSetCreateDto dto)
        {
            var existingCount = await _dataRepository.CountAllAsync<WordSetEntity>(x => x.Name == dto.Name && x.WordsLanguageCode == dto.WordsLanguageCode);
            if (existingCount != 0)
            {
                throw new BadRequestException($"Word set '{dto.Name}' already exists.");
            }

            var entity = _mapper.Map<WordSetEntity>(dto);
            entity.CreatedByUserId = userId;

            CustomValidationHelper.Validate(entity);
            await _dataRepository.AddAsync(entity);

            return _mapper.Map<WordSetDto>(entity); ;
        }
    }
}
