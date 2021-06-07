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
    public class WordsService : IWordsService
    {
        private readonly IMapper _mapper;
        private readonly IDataRepository _dataRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IImageService _imageService;
        private readonly ITwinwordWordDictionaryApiClient _twinwordWordDictionaryApiClient;
        private readonly IMicrosoftTranslatorApiClient _microsoftTranslatorApiClient;
        private readonly IReversoContextScraper _reversoContextScraper;
        private readonly IOxfordLearnersDictionariesScrapper _oxfordLearnersDictionariesScrapper;
        
        public WordsService(
            IMapper mapper,
            IDataRepository MongoDataRepository,
            IHttpClientFactory httpClientFactory,
            IImageService imageService,
            ITwinwordWordDictionaryApiClient twinwordWordDictionaryApiClient,
            IMicrosoftTranslatorApiClient microsoftTranslatorApiClient,
            IReversoContextScraper reversoContextScraper,
            IOxfordLearnersDictionariesScrapper oxfordLearnersDictionariesScrapper
        )
        {
            _mapper = mapper;
            _dataRepository = MongoDataRepository;
            _httpClientFactory = httpClientFactory;
            _imageService = imageService;
            _twinwordWordDictionaryApiClient = twinwordWordDictionaryApiClient;
            _microsoftTranslatorApiClient = microsoftTranslatorApiClient;
            _reversoContextScraper = reversoContextScraper;
            _oxfordLearnersDictionariesScrapper = oxfordLearnersDictionariesScrapper;
        }

        #region Words

        public async Task<PaginationResponseDto<WordDto>> GetAllWordsAsync(
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
        )
        {
            var predicate = PredicateBuilder.New<WordEntity>(x => x.UserId == userId);

            predicate.And(x => x.WordLanguageCode == languageCode);
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                predicate.And(x => x.Word.ToLower().Contains(search) || x.Meaning.ToLower().Contains(search));
            }
            if (isFavourite.GetValueOrDefault(false))
            {
                predicate.And(x => x.IsFavourite);
            }
            if (isTrained != null)
            {
                predicate.And(x => x.TrainingInfo != null && x.TrainingInfo.IsTrained == isTrained);
            }

            if(collectionId != null)
            {
                predicate.And(x => x.CustomCollectionIds.Contains(collectionId));
            }
            if (userWordSetId != null)
            {
                predicate.And(x => x.UserWordSetId == userWordSetId);
            }

            var totalTask = _dataRepository.CountAllAsync<WordEntity>(predicate);
            var total = await totalTask;

            if (isShuffle)
            {
                // get random N items
                var random = new Random();

                // TODO: avoid explicit long to int conversion
                offset = random.Next(0, (int)total - limit);
            }
            
            var itemsTask = _dataRepository.GetManyAsync<WordEntity>(predicate, offset, limit);
            var items = await itemsTask;

            var result = new PaginationResponseDto<WordDto>
            {
              
                Items = _mapper.Map<IEnumerable<WordDto>>(items),
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

        public async Task<WordDto> GetWordAsync(string userId, string wordId)
        {
            var entity = await _dataRepository.GetOneAsync<WordEntity>(x => x.Id == wordId && x.UserId == userId);
            return _mapper.Map<WordDto>(entity);
        }

        public async Task<WordDto> CreateWordAsync(string userId, WordCreateDto createDto)
        {
            var dictionary = await _dataRepository.GetOneAsync<UserDictionaryEntity>(x => x.UserId == userId && x.WordsLanguageCode == createDto.WordLanguageCode);
            if(dictionary == null)
            {
                throw new NotFoundException($"Dictionary for {createDto.WordLanguageCode} not found.");
            }

            var existingCount = await _dataRepository.CountAllAsync<WordEntity>(x => x.Word == createDto.Word && x.Meaning == createDto.Meaning);
            if (existingCount != 0)
            {
                throw new BadRequestException($"You already have word \"{createDto.Word}\" with meaning \"{createDto.Meaning}\" in you dictionary.");
            }

            var entity = _mapper.Map<WordEntity>(createDto);
            entity.UserId = userId;
            entity.UserDictionaryId = dictionary.Id;
            CustomValidationHelper.Validate(entity);

            // set image
            if (entity.Word.Length > 3)
            {
                var imagesResults = await _imageService.FindImagesAsync(sourceLanguageCode: entity.WordLanguageCode, entity.Word);
                imagesResults = _imageService.GetSuitableImages(imagesResults);
                var image = imagesResults.FirstOrDefault();
                if (image != null)
                {
                    entity.Images.Add(new GeneralImageEntity
                    {
                        Url = image.Url,
                        Height = int.Parse(image.Height),
                        Width = int.Parse(image.Width),
                        Thumbnail = image.Thumbnail,
                        ThumbnailHeight = int.Parse(image.ThumbnailHeight),
                        ThumbnailWidth = int.Parse(image.ThumbnailWidth),
                        Base64Encoding = image.Base64Encoding,
                    });
                }
            }


            await _dataRepository.AddAsync(entity);
            return _mapper.Map<WordDto>(entity);
        }

        public async Task<WordDto> BrowserExtensionCreateWordAsync(string userId, BrowserExtensionWordCreateDto createDto)
        {
            var dictionary = await _dataRepository.GetOneAsync<UserDictionaryEntity>(x => x.UserId == userId && x.WordsLanguageCode == createDto.WordLanguageCode);
            if (dictionary == null)
            {
                throw new NotFoundException($"Dictionary for {createDto.WordLanguageCode} not found.");
            }

            var existingCount = await _dataRepository.CountAllAsync<WordEntity>(x => x.Word == createDto.Word && x.Meaning == createDto.Meaning);
            if(existingCount != 0)
            {
                throw new BadRequestException($"You already have word \"{createDto.Word}\" with meaning \"{createDto.Meaning}\" in you dictionary.");
            }

            var entity = _mapper.Map<WordEntity>(createDto);
            entity.UserId = userId;
            entity.UserDictionaryId = dictionary.Id;
            CustomValidationHelper.Validate(entity);

            // set image
            if (entity.Word.Length > 3)
            {
                var imagesResults = await _imageService.FindImagesAsync(sourceLanguageCode: entity.WordLanguageCode, entity.Word);
                imagesResults = _imageService.GetSuitableImages(imagesResults);
                var image = imagesResults.FirstOrDefault();
                if (image != null)
                {
                    entity.Images.Add(new GeneralImageEntity
                    {
                        Url = image.Url,
                        Height = int.Parse(image.Height),
                        Width = int.Parse(image.Width),
                        Thumbnail = image.Thumbnail,
                        ThumbnailHeight = int.Parse(image.ThumbnailHeight),
                        ThumbnailWidth = int.Parse(image.ThumbnailWidth),
                        Base64Encoding = image.Base64Encoding,
                    });
                }
            }


            await _dataRepository.AddAsync(entity);
            return _mapper.Map<WordDto>(entity);
        }

        public async Task<WordDto> UpdateWordAsync(string userId, string wordId, WordUpdateDto updateDto)
        {
            var dictionary = await _dataRepository.GetOneAsync<UserDictionaryEntity>(x => x.UserId == userId && x.WordsLanguageCode == updateDto.WordLanguageCode);
            if (dictionary == null)
            {
                throw new NotFoundException($"Dictionary for {updateDto.WordLanguageCode} not found.");
            }

            var entity = await _dataRepository.GetOneAsync<WordEntity>(x => x.Id == wordId);
            if (entity.UserId != userId)
            {
                throw new AccessDeniedException("Can't edit the item that you don't own!");
            }
            entity.UserDictionaryId = dictionary.Id;

            // update
            entity.UpdateSelf(updateDto);

            // set image
            if (entity.Images == null || !entity.Images.Any())
            {
                if (entity.Word.Length > 3)
                {
                    var imagesResults = await _imageService.FindImagesAsync(sourceLanguageCode: entity.WordLanguageCode, entity.Word);
                    imagesResults = _imageService.GetSuitableImages(imagesResults);
                    var image = imagesResults.FirstOrDefault();
                    if (image != null)
                    {
                        entity.Images.Add(new GeneralImageEntity
                        {
                            Url = image.Url,
                            Height = int.Parse(image.Height),
                            Width = int.Parse(image.Width),
                            Thumbnail = image.Thumbnail,
                            ThumbnailHeight = int.Parse(image.ThumbnailHeight),
                            ThumbnailWidth = int.Parse(image.ThumbnailWidth),
                            Base64Encoding = image.Base64Encoding,
                        });
                    }
                }
            }

            await _dataRepository.UpdateAsync(entity);
            return _mapper.Map<WordDto>(entity);
        }

        public async Task DeleteWord(string userId, string sutyItemId)
        {
            var existing = await _dataRepository.GetOneAsync<WordEntity>(x => x.Id == sutyItemId && x.UserId == userId);
            if (existing == null)
            {
                throw new NotFoundException();
            }
            await _dataRepository.DeleteAsync<WordEntity>(x => x.Id == existing.Id);
        }

        public async Task<PaginationResponseDto<GeneralImageDto>> FindWordImagesAsync(string userId, string wordId)
        {
            var entity = await _dataRepository.GetOneAsync<WordEntity>(x => x.Id == wordId && x.UserId == userId);
            if (entity == null)
            {
                throw new NotFoundException();
            }

            var imagesResults = await _imageService.FindImagesAsync(entity.WordLanguageCode, entity.Word, limit: 100);
            imagesResults = _imageService.GetSuitableImages(imagesResults);
            imagesResults = imagesResults.Take(10).ToList();

            var result = new PaginationResponseDto<GeneralImageDto>()
            {
                Items = imagesResults.Select(x => _mapper.Map<GeneralImageDto>(x)),
                Pagination = new PaginationInfoDto()
                {
                    Offset = 0,
                    Limit = 10,
                    ReturnedCount = imagesResults.Count(),
                    TotalCount = imagesResults.Count(),
                },
            };

            return result;
        }

        public async Task<WordDto> UpdateWordImagesAsync(string userId, string wordId, UpdateWordImagesDto dto)
        {
            var entity = await _dataRepository.GetOneAsync<WordEntity>(x => x.Id == wordId && x.UserId == userId);
            if (entity == null)
            {
                throw new NotFoundException();
            }

            if (dto.Images == null || !dto.Images.Any())
            {
                return _mapper.Map<WordDto>(entity);
            }

            dto.Images = dto.Images.Where(x => x != null).ToList();

            // set width/height for images added by URL
            dto.Images = (await Task.WhenAll(dto.Images.Select(async image =>
            {
                if(!image.IsAddedByUrl)
                {
                    return image;
                }
                var imageInfo = await _imageService.GetImageInfoByUrlAsync(image.Url);
                if (imageInfo == null)
                {
                    return null;
                }

                image.Width = imageInfo.Width;
                image.Height = imageInfo.Height;
                return image;
            }))).Where(x => x != null).ToList();

            entity.Images = _mapper.Map<List<GeneralImageEntity>>(dto.Images);

            await _dataRepository.UpdateAsync(entity);
            return _mapper.Map<WordDto>(entity);
        }

        public async Task<WordMeaningsDto> GetWordMeaningsAsync(string word, string wordLanguageCode, string meaningLanguageCode)
        {
            // when word lang == meaning lang then we need to search meanings in dictionary and don't do translation
            if(wordLanguageCode == meaningLanguageCode)
            {
                throw new NotImplementedException($"{wordLanguageCode} -> {meaningLanguageCode} not supported yet");
            }

            var lookupResult = await _microsoftTranslatorApiClient.DictionaryLookupAsync(
                word,
                wordLanguageCode,
                meaningLanguageCode
            );

            var result = new WordMeaningsDto()
            {
                Word = word,
                WordLanguageCode = wordLanguageCode,
                MeaningLanguageCode = meaningLanguageCode,
                Meanings = lookupResult.Translations.Select(x => new WordMeaningsItemDto()
                {
                    Meaning = x.DisplayTarget,
                    PartOfSpeechTag = x.PosTag,
                }),
            };
            return result;
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
                Word = word,
                LanguageCode = languageCode,
                Examples = translationResult.Results.Select(x => new WordExampleItemDto()
                {
                    Example = x.SourceLanguageSentence,
                }),
            };
            return result;
        }

        public async Task<WordPronunciationAudioDto> GetWordPronunciationAudioAsync(string languageCode, string word)
        {
            var pronunciationResult = await _oxfordLearnersDictionariesScrapper.GetWordPronunciationAudioAsync(
                sourceLanguageCode: languageCode,
                targetLanguageCode: LanguageConfig.GetLanguageByCode("ru").Iso639_1_Code, // any language as we need examples and don't translations actually
                word: word
            );

            var result = new WordPronunciationAudioDto()
            {
                LanguageCode = languageCode,
                Word = word,
                AudioMp3Url = pronunciationResult.Results.FirstOrDefault()?.USAudioMp3Url ?? pronunciationResult.Results.FirstOrDefault()?.UKAudioMp3Url,
                AudioOggUrl = pronunciationResult.Results.FirstOrDefault()?.USAudioOggUrl ?? pronunciationResult.Results.FirstOrDefault()?.UKAudioOggUrl,
            };
            return result;
        }

        #endregion


        #region Favourites

        public async Task AddToFavouritesAsync(string userId, IEnumerable<string> itemIds)
        {
            var entities = (await _dataRepository.GetManyAsync<WordEntity>(x => x.UserId == userId && itemIds.Contains(x.Id))).ToList();

            entities = entities.Select(x =>
            {
                x.IsFavourite = true;
                return x;
            }).ToList();

            await _dataRepository.UpdateManyAsync<WordEntity>(entities);
        }

        public async Task DeleteFromFavouritesAsync(string userId, IEnumerable<string> itemIds)
        {
            var entities = (await _dataRepository.GetManyAsync<WordEntity>(x => x.UserId == userId && itemIds.Contains(x.Id))).ToList();

            entities = entities.Select(x =>
            {
                x.IsFavourite = false;
                return x;
            }).ToList();

            await _dataRepository.UpdateManyAsync<WordEntity>(entities);
        }

        #endregion
    }
}
