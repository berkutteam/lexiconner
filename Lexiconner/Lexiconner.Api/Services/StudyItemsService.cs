using AutoMapper;
using Lexiconner.Api.Dtos.StudyItemsTrainings;
using Lexiconner.Api.DTOs;
using Lexiconner.Api.DTOs.StudyItemsTrainings;
using Lexiconner.Api.Mappers;
using Lexiconner.Api.Models;
using Lexiconner.Api.Services.Interfaces;
using Lexiconner.Application.ApiClients;
using Lexiconner.Application.Exceptions;
using Lexiconner.Application.Extensions;
using Lexiconner.Application.Helpers;
using Lexiconner.Application.Services;
using Lexiconner.Application.Validation;
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
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Lexiconner.Api.Services
{
    public class StudyItemsService : IStudyItemsService
    {
        private readonly IMapper _mapper;
        private readonly IDataRepository _dataRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IImageService _imageService;
        private readonly ITwinwordWordDictionaryApiClient _twinwordWordDictionaryApiClient;

        public StudyItemsService(
            IMapper mapper,
            IDataRepository MongoDataRepository,
            IHttpClientFactory httpClientFactory,
            IImageService imageService,
            ITwinwordWordDictionaryApiClient twinwordWordDictionaryApiClient
        )
        {
            _mapper = mapper;
            _dataRepository = MongoDataRepository;
            _httpClientFactory = httpClientFactory;
            _imageService = imageService;
            _twinwordWordDictionaryApiClient = twinwordWordDictionaryApiClient;
        }

        #region Study items

        public async Task<PaginationResponseDto<StudyItemDto>> GetAllStudyItemsAsync(
            string userId, 
            int offset, 
            int limit, 
            StudyItemsSearchFilterModel searchFilter = null, 
            string collectionId = null
        )
        {
            var predicate = PredicateBuilder.New<StudyItemEntity>(x => x.UserId == userId);

            if(searchFilter != null)
            {
                if (!String.IsNullOrEmpty(searchFilter.Search))
                {
                    string search = searchFilter.Search.Trim().ToLower();
                    predicate.And(x => x.Title.ToLower().Contains(search) || x.Description.ToLower().Contains(search));
                }
                if (searchFilter.IsFavourite.GetValueOrDefault(false))
                {
                    predicate.And(x => x.IsFavourite);
                }
                if (searchFilter.IsTrained != null)
                {
                    predicate.And(x => x.TrainingInfo != null && x.TrainingInfo.IsTrained == searchFilter.IsTrained);
                }
            }

            if(collectionId != null)
            {
                predicate.And(x => x.CustomCollectionIds.Contains(collectionId));
            }

            var totalTask = _dataRepository.CountAllAsync<StudyItemEntity>(predicate);
            var total = await totalTask;

            if (searchFilter != null && searchFilter.IsShuffle)
            {
                // get random N items
                var random = new Random();

                // TODO: avoid explicit long to int conversion
                offset = random.Next(0, (int)total - limit);
            }
            
            var itemsTask = _dataRepository.GetManyAsync<StudyItemEntity>(predicate, offset, limit);
            var items = await itemsTask;

            var result = new PaginationResponseDto<StudyItemDto>
            {
              
                Items = _mapper.Map<IEnumerable<StudyItemDto>>(items),
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

        public async Task<StudyItemDto> GetStudyItemAsync(string userId, string studyItemId)
        {
            var entity = await _dataRepository.GetOneAsync<StudyItemEntity>(x => x.Id == studyItemId && x.UserId == userId);
            return _mapper.Map<StudyItemDto>(entity);
        }

        public async Task<StudyItemDto> CreateStudyItemAsync(string userId, StudyItemCreateDto createDto)
        {
            var entity = CustomMapper.MapToEntity(userId, createDto);
            CustomValidationHelper.Validate(entity);

            // set image
            if (entity.Title.Length > 3)
            {
                var imagesResult = await _imageService.FindImagesAsync(sourceLanguageCode: entity.LanguageCode, entity.Title);

                if (imagesResult.Any())
                {
                    // try to find suitable image
                    var image = _imageService.GetSuitableImages(imagesResult).FirstOrDefault();
                    if (image != null)
                    {
                        entity.Image = new StudyItemImageEntity
                        {
                            Url = image.Url,
                            Height = image.Height,
                            Width = image.Width,
                            Thumbnail = image.Thumbnail,
                            ThumbnailHeight = image.ThumbnailHeight,
                            ThumbnailWidth = image.ThumbnailWidth,
                            Base64Encoding = image.Base64Encoding,
                        };
                    }
                }
            }


            await _dataRepository.AddAsync(entity);
            return _mapper.Map<StudyItemDto>(entity);
        }

        public async Task<StudyItemDto> UpdateStudyItemAsync(string userId, string studyItemId, StudyItemUpdateDto updateDto)
        {
            var entity = await _dataRepository.GetOneAsync<StudyItemEntity>(x => x.Id == studyItemId);
            if (entity.UserId != userId)
            {
                throw new AccessDeniedException("Can't edit the item that you don't own!");
            }

            // update
            entity.UpdateSelf(updateDto);

            // set image
            if (entity.Image == null)
            {
                if (entity.Title.Length > 3)
                {
                    var imagesResult = await _imageService.FindImagesAsync(sourceLanguageCode: entity.LanguageCode, entity.Title);

                    if (imagesResult.Any())
                    {
                        // try to find suitable image
                        var image = _imageService.GetSuitableImages(imagesResult).FirstOrDefault();
                        if (image != null)
                        {
                            entity.Image = new StudyItemImageEntity
                            {
                                Url = image.Url,
                                Height = image.Height,
                                Width = image.Width,
                                Thumbnail = image.Thumbnail,
                                ThumbnailHeight = image.ThumbnailHeight,
                                ThumbnailWidth = image.ThumbnailWidth,
                                Base64Encoding = image.Base64Encoding,
                            };
                        }
                    }
                }
            }

            await _dataRepository.UpdateAsync(entity);
            return _mapper.Map<StudyItemDto>(entity);
        }

        public async Task DeleteStudyItem(string userId, string sutyItemId)
        {
            var existing = await _dataRepository.GetOneAsync<StudyItemEntity>(x => x.Id == sutyItemId && x.UserId == userId);
            if (existing == null)
            {
                throw new NotFoundException();
            }
            await _dataRepository.DeleteAsync<StudyItemEntity>(x => x.Id == existing.Id);
        }

        public async Task<PaginationResponseDto<StudyItemImageDto>> FindWordImagesAsync(string userId, string wordId)
        {
            var entity = await _dataRepository.GetOneAsync<StudyItemEntity>(x => x.Id == wordId && x.UserId == userId);
            if (entity == null)
            {
                throw new NotFoundException();
            }

            var imagesResult = await _imageService.FindImagesAsync(entity.LanguageCode, entity.Title, limit: 100);
            imagesResult = _imageService.GetSuitableImages(imagesResult).Take(10).ToList();

            var result = new PaginationResponseDto<StudyItemImageDto>()
            {
                Items = imagesResult.Select(x => _mapper.Map<StudyItemImageDto>(x)),
                Pagination = new PaginationInfoDto()
                {
                    Offset = 0,
                    Limit = 10,
                    ReturnedCount = imagesResult.Count,
                    TotalCount = imagesResult.Count,
                },
            };

            return result;
        }

        public async Task<StudyItemDto> UpdateWordImagesAsync(string userId, string wordId, UpdateWordImagesDto dto)
        {
            var entity = await _dataRepository.GetOneAsync<StudyItemEntity>(x => x.Id == wordId && x.UserId == userId);
            if (entity == null)
            {
                throw new NotFoundException();
            }

            if (dto.Images == null || !dto.Images.Any())
            {
                return _mapper.Map<StudyItemDto>(entity);
            }

            // set width/height for images added by URL
            foreach (var image in dto.Images.Where(x => x.IsAddedByUrl))
            {
                var httpClent = _httpClientFactory.CreateClient();
                using (var stream = await httpClent.GetStreamAsync(image.Url))
                {
                    var bitmap = new Bitmap(stream);
                    image.Width = bitmap.Width.ToString();
                    image.Height = bitmap.Height.ToString();
                }
            }

            entity.Image = _mapper.Map<StudyItemImageEntity>(dto.Images.First());
            entity.Images = _mapper.Map<List<StudyItemImageEntity>>(dto.Images);

            await _dataRepository.UpdateAsync(entity);
            return _mapper.Map<StudyItemDto>(entity);
        }

        #endregion


        #region Trainings

        public async Task<TrainingsStatisticsDto> GetTrainingStatisticsAsync(string userId)
        {
            long totalItemCount = await _dataRepository.CountAllAsync<StudyItemEntity>(x => x.UserId == userId);
            long onTrainingItemCount = await _dataRepository.CountAllAsync<StudyItemEntity>(x => 
                x.UserId == userId && 
                x.TrainingInfo != null && 
                !x.TrainingInfo.IsTrained
            );
            long trainedItemCount = await _dataRepository.CountAllAsync<StudyItemEntity>(x => 
                x.UserId == userId && 
                x.TrainingInfo != null &&
                x.TrainingInfo.IsTrained
            );

            Func<string, TrainingType, Task<TrainingsStatisticsDto.TrainingStatisticsItemDto>> getTrainingStatisticsItem = async (_userId, trainingType) =>
            {
                return new TrainingsStatisticsDto.TrainingStatisticsItemDto
                {
                    TrainingType = trainingType,
                    TrainingTypeFormatted = EnumHelper<TrainingType>.GetDisplayValue(trainingType),
                    OnTrainingItemCount = await _dataRepository.CountAllAsync<StudyItemEntity>(
                            x => x.UserId == _userId && x.TrainingInfo != null && x.TrainingInfo.Trainings.Any(y => y.TrainingType == trainingType && y.Progress != 1)
                        ),
                    TrainedItemCount = await _dataRepository.CountAllAsync<StudyItemEntity>(
                            x => x.UserId == _userId && x.TrainingInfo != null && x.TrainingInfo.Trainings.Any(y => y.TrainingType == trainingType && y.Progress == 1)
                        ),
                };
            };

            var result = new TrainingsStatisticsDto()
            {
                TotalItemCount = totalItemCount,
                OnTrainingItemCount = onTrainingItemCount,
                TrainedItemCount = trainedItemCount,

                TrainingStats = new List<TrainingsStatisticsDto.TrainingStatisticsItemDto>()
                {
                    await getTrainingStatisticsItem(userId, TrainingType.FlashCards),
                    await getTrainingStatisticsItem(userId, TrainingType.WordMeaning),
                    await getTrainingStatisticsItem(userId, TrainingType.MeaningWord),
                    await getTrainingStatisticsItem(userId, TrainingType.MatchWords),
                }
            };

            return result;
        }

        public async Task MarkStudyItemAsTrainedAsync(string userId, string studyItemId)
        {
            var entity = await _dataRepository.GetOneAsync<StudyItemEntity>(x => x.UserId == userId && x.Id == studyItemId);
            entity.MarkAsTrained();
            entity.RecalculateTotalTrainingProgress();
            await _dataRepository.UpdateAsync(entity);
        }

        public async Task MarkStudyItemAsNotTrainedAsync(string userId, string studyItemId)
        {
            var entity = await _dataRepository.GetOneAsync<StudyItemEntity>(x => x.UserId == userId && x.Id == studyItemId);
            entity.MarkAsNotTrained();
            entity.ResetTotalTrainingProgress();
            await _dataRepository.UpdateAsync(entity);
        }

        public async Task<FlashCardsTrainingDto> GetTrainingItemsForFlashCardsAsync(string userId, string collectionId, int limit)
        {
            var predicate = PredicateBuilder.New<StudyItemEntity>(x =>
                x.UserId == userId &&
                (
                    x.TrainingInfo == null ||
                    !x.TrainingInfo.Trainings.Any() ||
                    (
                        x.TrainingInfo != null && 
                        !x.TrainingInfo.IsTrained &&
                        x.TrainingInfo.Trainings.Any(y => y.TrainingType == TrainingType.FlashCards && y.Progress < 1 && y.NextTrainingdAt <= DateTime.UtcNow)
                    )
                )
            );

            if (collectionId != null)
            {
                predicate.And(x => x.CustomCollectionIds.Contains(collectionId));
            }

            var entities = await _dataRepository.GetManyAsync<StudyItemEntity>(predicate, 0, limit);

            return new FlashCardsTrainingDto
            {
                Items = entities.ToList(),
            };
        }

        public async Task SaveTrainingResultsForFlashCardsAsync(string userId, FlashCardsTrainingResultDto results)
        {
            var ids = results.ItemsResults.Select(x => x.ItemId);
            var entities = (await _dataRepository.GetManyAsync<StudyItemEntity>(
                x => x.UserId == userId && ids.Contains(x.Id)
            )).ToList();

            var infoAttribute = TrainingTypeHelper.GetAttribute(TrainingType.FlashCards);

            entities = entities.Select(x =>
            {
                bool isCorrect = results.ItemsResults.Any(y => y.ItemId == x.Id && y.IsCorrect);

                x.TrainingInfo = x.TrainingInfo ?? new StudyItemTrainingInfoEntity();
                x.TrainingInfo.Trainings = x.TrainingInfo.Trainings ?? new List<StudyItemTrainingInfoEntity.StudyItemTrainingProgressItemEntity>();

                var training = x.TrainingInfo.Trainings.FirstOrDefault(y => y.TrainingType == TrainingType.FlashCards);
                if(training == null)
                {
                    training = new StudyItemTrainingInfoEntity.StudyItemTrainingProgressItemEntity() {
                        TrainingType = TrainingType.FlashCards,
                    };
                    x.TrainingInfo.Trainings.Add(training);
                }

                if (isCorrect)
                {
                    training.Progress = training.Progress + infoAttribute.CorrectAnswerProgressRate;
                    training.Progress = Math.Min(training.Progress, 1);
                }
                else
                {
                    training.Progress = training.Progress + infoAttribute.WrongAnswerProgressRate;
                    training.Progress = Math.Max(training.Progress, 0);
                }
                training.Progress = Math.Round(training.Progress, 2);

                training.LastTrainingdAt = DateTimeOffset.UtcNow;

                if(training.Progress < 1)
                {
                    training.NextTrainingdAt = DateTimeOffset.UtcNow.Add(infoAttribute.TrainIntervalTimespan);
                } 
                else
                {
                    training.NextTrainingdAt = DateTimeOffset.UtcNow.Add(infoAttribute.TrainIntervalForRepeatTimespan);
                }

                x.RecalculateTotalTrainingProgress();

                return x;
            }).ToList();

            await _dataRepository.UpdateManyAsync(entities);
        }

        public async Task<WordMeaningTrainingDto> GetTrainingItemsForWordMeaningAsync(string userId, string collectionId, int limit)
        {
            const int meaningsPerWord = 5;
            var random = new Random();

            var predicate = PredicateBuilder.New<StudyItemEntity>(x =>
                x.UserId == userId &&
                (
                    x.TrainingInfo == null ||
                    !x.TrainingInfo.Trainings.Any() ||
                    (
                        x.TrainingInfo != null &&
                        !x.TrainingInfo.IsTrained &&
                        x.TrainingInfo.Trainings.Any(y => y.TrainingType == TrainingType.WordMeaning && y.Progress < 1 && y.NextTrainingdAt <= DateTime.UtcNow)
                    )
                )
            );

            if (collectionId != null)
            {
                predicate.And(x => x.CustomCollectionIds.Contains(collectionId));
            }

            var entities = await _dataRepository.GetManyAsync<StudyItemEntity>(predicate, 0, limit);

            // find meanings list for each entity
            var trainingItems = new List<WordMeaningTrainingItemDto>();
            foreach (var entity in entities)
            {
                var possibleOptions= new List<WordMeaningTrainingOptionDto>()
                {
                    // correct meaning
                    new WordMeaningTrainingOptionDto()
                    {
                        Value = entity.Description,
                        IsCorrect = true,
                    }
                };

                // other similar meanings
                // v1: search from other study items with the same language
                long otherStudyItemsCount = await _dataRepository.CountAllAsync<StudyItemEntity>(x => x.LanguageCode == entity.LanguageCode && x.Id != entity.Id);
                int otherStudyItemsCountInt = otherStudyItemsCount > int.MaxValue ? int.MaxValue : (int)otherStudyItemsCount;
                int otherStudyItemsLimit = meaningsPerWord - 1;
                int otherStudyItemsOffset = random.Next(0, otherStudyItemsCountInt - otherStudyItemsLimit);
                var otherStudyItems = await _dataRepository.GetManyAsync<StudyItemEntity>(
                    x => x.LanguageCode == entity.LanguageCode && x.Id != entity.Id,
                    otherStudyItemsOffset,
                    otherStudyItemsLimit
                );
                possibleOptions.AddRange(otherStudyItems.Select(x => new WordMeaningTrainingOptionDto()
                {
                    Value = x.Description,
                    IsCorrect = false,
                }));

                // find accross 
                trainingItems.Add(new WordMeaningTrainingItemDto()
                {
                    StudyItem = _mapper.Map<StudyItemDto>(entity),
                    PossibleOptions = possibleOptions,
                });
            }

            // shuffle
            foreach (var item in trainingItems)
            {
                item.PossibleOptions = item.PossibleOptions.Shuffle();
            }

            var result = new WordMeaningTrainingDto
            {
               Items = trainingItems,
            };
            return result;
        }

        public async Task SaveTrainingResultsForWordMeaningAsync(string userId, WordMeaningTrainingResultDto results)
        {
            var ids = results.ItemsResults.Select(x => x.ItemId);
            var entities = (await _dataRepository.GetManyAsync<StudyItemEntity>(
                x => x.UserId == userId && ids.Contains(x.Id)
            )).ToList();

            var infoAttribute = TrainingTypeHelper.GetAttribute(TrainingType.WordMeaning);

            entities = entities.Select(x =>
            {
                bool isCorrect = results.ItemsResults.Any(y => y.ItemId == x.Id && y.IsCorrect);

                x.TrainingInfo = x.TrainingInfo ?? new StudyItemTrainingInfoEntity();
                x.TrainingInfo.Trainings = x.TrainingInfo.Trainings ?? new List<StudyItemTrainingInfoEntity.StudyItemTrainingProgressItemEntity>();

                var training = x.TrainingInfo.Trainings.FirstOrDefault(y => y.TrainingType == TrainingType.WordMeaning);
                if (training == null)
                {
                    training = new StudyItemTrainingInfoEntity.StudyItemTrainingProgressItemEntity()
                    {
                        TrainingType = TrainingType.WordMeaning,
                    };
                    x.TrainingInfo.Trainings.Add(training);
                }

                if (isCorrect)
                {
                    training.Progress = training.Progress + infoAttribute.CorrectAnswerProgressRate;
                    training.Progress = Math.Min(training.Progress, 1);
                }
                else
                {
                    training.Progress = training.Progress + infoAttribute.WrongAnswerProgressRate;
                    training.Progress = Math.Max(training.Progress, 0);
                }
                training.Progress = Math.Round(training.Progress, 2);

                training.LastTrainingdAt = DateTimeOffset.UtcNow;

                if (training.Progress < 1)
                {
                    training.NextTrainingdAt = DateTimeOffset.UtcNow.Add(infoAttribute.TrainIntervalTimespan);
                }
                else
                {
                    training.NextTrainingdAt = DateTimeOffset.UtcNow.Add(infoAttribute.TrainIntervalForRepeatTimespan);
                }

                x.RecalculateTotalTrainingProgress();

                return x;
            }).ToList();

            await _dataRepository.UpdateManyAsync(entities);
        }

        public async Task<MeaningWordTrainingDto> GetTrainingItemsForMeaningWordAsync(string userId, string collectionId, int limit)
        {
            const int meaningsPerWord = 5;
            var random = new Random();

            var predicate = PredicateBuilder.New<StudyItemEntity>(x =>
                x.UserId == userId &&
                (
                    x.TrainingInfo == null ||
                    !x.TrainingInfo.Trainings.Any() ||
                    (
                        x.TrainingInfo != null &&
                        !x.TrainingInfo.IsTrained &&
                        x.TrainingInfo.Trainings.Any(y => y.TrainingType == TrainingType.MeaningWord && y.Progress < 1 && y.NextTrainingdAt <= DateTime.UtcNow)
                    )
                )
            );

            if (collectionId != null)
            {
                predicate.And(x => x.CustomCollectionIds.Contains(collectionId));
            }

            var entities = await _dataRepository.GetManyAsync<StudyItemEntity>(predicate, 0, limit);

            // find words list for each entity
            var trainingItems = new List<MeaningWordTrainingItemDto>();
            foreach (var entity in entities)
            {
                var possibleOptions = new List<MeaningWordTrainingOptionDto>()
                {
                    // correct meaning
                    new MeaningWordTrainingOptionDto()
                    {
                        Value = entity.Title,
                        IsCorrect = true,
                    }
                };

                // other similar words
                // v1: search from other study items with the same language
                long otherStudyItemsCount = await _dataRepository.CountAllAsync<StudyItemEntity>(x => x.LanguageCode == entity.LanguageCode && x.Id != entity.Id);
                int otherStudyItemsCountInt = otherStudyItemsCount > int.MaxValue ? int.MaxValue : (int)otherStudyItemsCount;
                int otherStudyItemsLimit = meaningsPerWord - 1;
                int otherStudyItemsOffset = random.Next(0, otherStudyItemsCountInt - otherStudyItemsLimit);
                var otherStudyItems = await _dataRepository.GetManyAsync<StudyItemEntity>(
                    x => x.LanguageCode == entity.LanguageCode && x.Id != entity.Id,
                    otherStudyItemsOffset,
                    otherStudyItemsLimit
                );
                possibleOptions.AddRange(otherStudyItems.Select(x => new MeaningWordTrainingOptionDto()
                {
                    Value = x.Title,
                    IsCorrect = false,
                }));

                // find accross 
                trainingItems.Add(new MeaningWordTrainingItemDto()
                {
                    StudyItem = _mapper.Map<StudyItemDto>(entity),
                    PossibleOptions = possibleOptions,
                });
            }

            // shuffle
            foreach (var item in trainingItems)
            {
                item.PossibleOptions = item.PossibleOptions.Shuffle();
            }

            var result = new MeaningWordTrainingDto
            {
                Items = trainingItems,
            };
            return result;
        }

        public async Task SaveTrainingResultsForMeaningWordAsync(string userId, MeaningWordTrainingResultDto results)
        {
            var ids = results.ItemsResults.Select(x => x.ItemId);
            var entities = (await _dataRepository.GetManyAsync<StudyItemEntity>(
                x => x.UserId == userId && ids.Contains(x.Id)
            )).ToList();

            var infoAttribute = TrainingTypeHelper.GetAttribute(TrainingType.MeaningWord);

            entities = entities.Select(x =>
            {
                bool isCorrect = results.ItemsResults.Any(y => y.ItemId == x.Id && y.IsCorrect);

                x.TrainingInfo = x.TrainingInfo ?? new StudyItemTrainingInfoEntity();
                x.TrainingInfo.Trainings = x.TrainingInfo.Trainings ?? new List<StudyItemTrainingInfoEntity.StudyItemTrainingProgressItemEntity>();

                var training = x.TrainingInfo.Trainings.FirstOrDefault(y => y.TrainingType == TrainingType.MeaningWord);
                if (training == null)
                {
                    training = new StudyItemTrainingInfoEntity.StudyItemTrainingProgressItemEntity()
                    {
                        TrainingType = TrainingType.MeaningWord,
                    };
                    x.TrainingInfo.Trainings.Add(training);
                }

                if (isCorrect)
                {
                    training.Progress = training.Progress + infoAttribute.CorrectAnswerProgressRate;
                    training.Progress = Math.Min(training.Progress, 1);
                }
                else
                {
                    training.Progress = training.Progress + infoAttribute.WrongAnswerProgressRate;
                    training.Progress = Math.Max(training.Progress, 0);
                }
                training.Progress = Math.Round(training.Progress, 2);

                training.LastTrainingdAt = DateTimeOffset.UtcNow;

                if (training.Progress < 1)
                {
                    training.NextTrainingdAt = DateTimeOffset.UtcNow.Add(infoAttribute.TrainIntervalTimespan);
                }
                else
                {
                    training.NextTrainingdAt = DateTimeOffset.UtcNow.Add(infoAttribute.TrainIntervalForRepeatTimespan);
                }

                x.RecalculateTotalTrainingProgress();

                return x;
            }).ToList();

            await _dataRepository.UpdateManyAsync(entities);
        }

        public async Task<MatchWordsTrainingDto> GetTrainingItemsForMatchWordsAsync(string userId, string collectionId)
        {
            const int matchWordsCount = 5;
            const int additionalOptionsCount = 3;
            var random = new Random();

            var predicate = PredicateBuilder.New<StudyItemEntity>(x =>
                x.UserId == userId &&
                (
                    x.TrainingInfo == null ||
                    !x.TrainingInfo.Trainings.Any() ||
                    (
                        x.TrainingInfo != null &&
                        !x.TrainingInfo.IsTrained &&
                        x.TrainingInfo.Trainings.Any(y => y.TrainingType == TrainingType.MatchWords && y.Progress < 1 && y.NextTrainingdAt <= DateTime.UtcNow)
                    )
                )
            );

            if (collectionId != null)
            {
                predicate.And(x => x.CustomCollectionIds.Contains(collectionId));
            }

            var entities = await _dataRepository.GetManyAsync<StudyItemEntity>(predicate, 0, matchWordsCount);

            if(!entities.Any())
            {
                return new MatchWordsTrainingDto();
            }

            var entitiesIds = entities.Select(x => x.Id).ToList();
            string languageCode = entities.First().LanguageCode;

            // other similar words
            // v1: search from other study items with the same language
            long otherStudyItemsCount = await _dataRepository.CountAllAsync<StudyItemEntity>(x => x.LanguageCode == languageCode && !entitiesIds.Contains(x.Id));
            int otherStudyItemsCountInt = otherStudyItemsCount > int.MaxValue ? int.MaxValue : (int)otherStudyItemsCount;
            int otherStudyItemsLimit = additionalOptionsCount;
            int otherStudyItemsOffset = random.Next(0, otherStudyItemsCountInt - otherStudyItemsLimit);
            var otherStudyItems = await _dataRepository.GetManyAsync<StudyItemEntity>(
                x => x.LanguageCode == languageCode && !entitiesIds.Contains(x.Id),
                otherStudyItemsOffset,
                otherStudyItemsLimit
            );

            // build options
            var possibleOptions = entities.Select(x => new MatchWordsTrainingPossibleOptionDto()
            {
                Value = x.Description,
                CorrectForStudyItemId = x.Id,
            });

            // add other words options to complicate training
            possibleOptions = possibleOptions.Concat(
                 otherStudyItems.Select(x => new MatchWordsTrainingPossibleOptionDto()
                 {
                     Value = x.Description,
                     CorrectForStudyItemId = null,
                 })
            );

            // shuffle
            entities = entities.Shuffle();
            possibleOptions = possibleOptions.Shuffle();

            var result = new MatchWordsTrainingDto
            {
                Items = entities.Select(x => new MatchWordsTrainingItemDto()
                {
                    StudyItem = _mapper.Map<StudyItemDto>(x),
                    CorrectOptionId = possibleOptions.First(y => y.CorrectForStudyItemId == x.Id).RandomId,
                }),
                PossibleOptions = possibleOptions,
            };
            return result;
        }

        public async Task SaveTrainingResultsForMatchWordsAsync(string userId, MatchWordsTrainingResultDto results)
        {
            var ids = results.ItemsResults.Select(x => x.ItemId);
            var entities = (await _dataRepository.GetManyAsync<StudyItemEntity>(
                x => x.UserId == userId && ids.Contains(x.Id)
            )).ToList();

            var infoAttribute = TrainingTypeHelper.GetAttribute(TrainingType.MatchWords);

            entities = entities.Select(x =>
            {
                bool isCorrect = results.ItemsResults.Any(y => y.ItemId == x.Id && y.IsCorrect);

                x.TrainingInfo = x.TrainingInfo ?? new StudyItemTrainingInfoEntity();
                x.TrainingInfo.Trainings = x.TrainingInfo.Trainings ?? new List<StudyItemTrainingInfoEntity.StudyItemTrainingProgressItemEntity>();

                var training = x.TrainingInfo.Trainings.FirstOrDefault(y => y.TrainingType == TrainingType.MatchWords);
                if (training == null)
                {
                    training = new StudyItemTrainingInfoEntity.StudyItemTrainingProgressItemEntity()
                    {
                        TrainingType = TrainingType.MatchWords,
                    };
                    x.TrainingInfo.Trainings.Add(training);
                }

                if (isCorrect)
                {
                    training.Progress = training.Progress + infoAttribute.CorrectAnswerProgressRate;
                    training.Progress = Math.Min(training.Progress, 1);
                }
                else
                {
                    training.Progress = training.Progress + infoAttribute.WrongAnswerProgressRate;
                    training.Progress = Math.Max(training.Progress, 0);
                }
                training.Progress = Math.Round(training.Progress, 2);

                training.LastTrainingdAt = DateTimeOffset.UtcNow;

                if (training.Progress < 1)
                {
                    training.NextTrainingdAt = DateTimeOffset.UtcNow.Add(infoAttribute.TrainIntervalTimespan);
                }
                else
                {
                    training.NextTrainingdAt = DateTimeOffset.UtcNow.Add(infoAttribute.TrainIntervalForRepeatTimespan);
                }

                x.RecalculateTotalTrainingProgress();

                return x;
            }).ToList();

            await _dataRepository.UpdateManyAsync(entities);
        }

        #endregion


        #region Favourites

        public async Task AddToFavouritesAsync(string userId, IEnumerable<string> itemIds)
        {
            var entities = (await _dataRepository.GetManyAsync<StudyItemEntity>(x => x.UserId == userId && itemIds.Contains(x.Id))).ToList();

            entities = entities.Select(x =>
            {
                x.IsFavourite = true;
                return x;
            }).ToList();

            await _dataRepository.UpdateManyAsync<StudyItemEntity>(entities);
        }

        public async Task DeleteFromFavouritesAsync(string userId, IEnumerable<string> itemIds)
        {
            var entities = (await _dataRepository.GetManyAsync<StudyItemEntity>(x => x.UserId == userId && itemIds.Contains(x.Id))).ToList();

            entities = entities.Select(x =>
            {
                x.IsFavourite = false;
                return x;
            }).ToList();

            await _dataRepository.UpdateManyAsync<StudyItemEntity>(entities);
        }

        #endregion
    }
}
