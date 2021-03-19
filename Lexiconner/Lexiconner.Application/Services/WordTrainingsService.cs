using AutoMapper;
using Lexiconner.Api.Dtos.WordsTrainings;
using Lexiconner.Api.DTOs.WordsTrainings;
using Lexiconner.Application.ApiClients;
using Lexiconner.Application.Exceptions;
using Lexiconner.Application.Extensions;
using Lexiconner.Application.Helpers;
using Lexiconner.Application.Services.Interfacse;
using Lexiconner.Application.Validation;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Dtos.WordTrainings;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Enums;
using Lexiconner.Domain.Helpers;
using Lexiconner.Persistence.Repositories;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lexiconner.Application.Services
{
    public class WordTrainingsService: IWordTrainingsService
    {
        private readonly IMapper _mapper;
        private readonly IDataRepository _dataRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IImageService _imageService;
        private readonly ITwinwordWordDictionaryApiClient _twinwordWordDictionaryApiClient;

        public WordTrainingsService(
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

        #region Trainings

        public async Task<TrainingsStatisticsDto> GetTrainingStatisticsAsync(string userId)
        {
            long totalItemCount = await _dataRepository.CountAllAsync<WordEntity>(x => x.UserId == userId);
            long onTrainingItemCount = await _dataRepository.CountAllAsync<WordEntity>(x =>
                x.UserId == userId &&
                x.TrainingInfo != null &&
                !x.TrainingInfo.IsTrained
            );
            long trainedItemCount = await _dataRepository.CountAllAsync<WordEntity>(x =>
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
                    OnTrainingItemCount = await _dataRepository.CountAllAsync<WordEntity>(
                            x => x.UserId == _userId && x.TrainingInfo != null && x.TrainingInfo.Trainings.Any(y => y.TrainingType == trainingType && y.Progress != 1)
                        ),
                    TrainedItemCount = await _dataRepository.CountAllAsync<WordEntity>(
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

        public async Task MarkWordAsTrainedAsync(string userId, string wordId)
        {
            var entity = await _dataRepository.GetOneAsync<WordEntity>(x => x.UserId == userId && x.Id == wordId);
            entity.MarkAsTrained();
            entity.RecalculateTotalTrainingProgress();
            await _dataRepository.UpdateAsync(entity);
        }

        public async Task MarkWordAsNotTrainedAsync(string userId, string wordId)
        {
            var entity = await _dataRepository.GetOneAsync<WordEntity>(x => x.UserId == userId && x.Id == wordId);
            entity.MarkAsNotTrained();
            entity.ResetTotalTrainingProgress();
            await _dataRepository.UpdateAsync(entity);
        }

        public async Task<FlashCardsTrainingDto> GetTrainingItemsForFlashCardsAsync(string userId, string collectionId, int limit)
        {
            var predicate = PredicateBuilder.New<WordEntity>(x =>
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

            var entities = await _dataRepository.GetManyAsync<WordEntity>(predicate, 0, limit);

            return new FlashCardsTrainingDto
            {
                Items = entities.ToList(),
            };
        }

        public async Task SaveTrainingResultsForFlashCardsAsync(string userId, FlashCardsTrainingResultDto results)
        {
            if (results.TrainingType != TrainingType.FlashCards)
            {
                throw new BadRequestException("Incorrect training type passed!");
            }

            var ids = results.ItemsResults.Select(x => x.ItemId);
            var entities = (await _dataRepository.GetManyAsync<WordEntity>(
                x => x.UserId == userId && ids.Contains(x.Id)
            )).ToList();

            var infoAttribute = TrainingTypeHelper.GetAttribute(TrainingType.FlashCards);

            entities = entities.Select(x =>
            {
                bool isCorrect = results.ItemsResults.Any(y => y.ItemId == x.Id && y.IsCorrect);

                x.TrainingInfo = x.TrainingInfo ?? new WordTrainingInfoEntity();
                x.TrainingInfo.Trainings = x.TrainingInfo.Trainings ?? new List<WordTrainingInfoEntity.WordTrainingProgressItemEntity>();

                var training = x.TrainingInfo.Trainings.FirstOrDefault(y => y.TrainingType == TrainingType.FlashCards);
                if (training == null)
                {
                    training = new WordTrainingInfoEntity.WordTrainingProgressItemEntity()
                    {
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

            CustomValidationHelper.Validate(entities);
            await _dataRepository.UpdateManyAsync(entities);
        }

        public async Task<WordMeaningTrainingDto> GetTrainingItemsForWordMeaningAsync(string userId, string collectionId, int limit)
        {
            const int meaningsPerWord = 5;
            var random = new Random();

            var predicate = PredicateBuilder.New<WordEntity>(x =>
                x.UserId == userId &&
                x.Word != null &&
                x.Meaning != null &&
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

            var entities = await _dataRepository.GetManyAsync<WordEntity>(predicate, 0, limit);

            // find meanings list for each entity
            var trainingItems = new List<WordMeaningTrainingItemDto>();
            foreach (var entity in entities)
            {
                var possibleOptions = new List<WordMeaningTrainingOptionDto>()
                {
                    // correct meaning
                    new WordMeaningTrainingOptionDto()
                    {
                        Value = entity.Meaning,
                        IsCorrect = true,
                    }
                };

                // other similar meanings
                // v1: search from other words with the same language
                long otherWordsCount = await _dataRepository.CountAllAsync<WordEntity>(x => x.WordLanguageCode == entity.WordLanguageCode && x.Id != entity.Id && x.Meaning != null);
                int otherWordsCountInt = otherWordsCount > int.MaxValue ? int.MaxValue : (int)otherWordsCount;
                int otherWordsLimit = meaningsPerWord - 1;
                int otherWordsOffset = random.Next(0, otherWordsCountInt - otherWordsLimit);
                var otherWords = await _dataRepository.GetManyAsync<WordEntity>(
                    x => x.WordLanguageCode == entity.WordLanguageCode && x.Id != entity.Id && x.Meaning != null,
                    otherWordsOffset,
                    otherWordsLimit
                );
                possibleOptions.AddRange(otherWords.Select(x => new WordMeaningTrainingOptionDto()
                {
                    Value = x.Meaning,
                    IsCorrect = false,
                }));

                // find accross 
                trainingItems.Add(new WordMeaningTrainingItemDto()
                {
                    Word = _mapper.Map<WordDto>(entity),
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
            if (results.TrainingType != TrainingType.WordMeaning)
            {
                throw new BadRequestException("Incorrect training type passed!");
            }

            var ids = results.ItemsResults.Select(x => x.ItemId);
            var entities = (await _dataRepository.GetManyAsync<WordEntity>(
                x => x.UserId == userId && ids.Contains(x.Id)
            )).ToList();

            var infoAttribute = TrainingTypeHelper.GetAttribute(TrainingType.WordMeaning);

            entities = entities.Select(x =>
            {
                bool isCorrect = results.ItemsResults.Any(y => y.ItemId == x.Id && y.IsCorrect);

                x.TrainingInfo = x.TrainingInfo ?? new WordTrainingInfoEntity();
                x.TrainingInfo.Trainings = x.TrainingInfo.Trainings ?? new List<WordTrainingInfoEntity.WordTrainingProgressItemEntity>();

                var training = x.TrainingInfo.Trainings.FirstOrDefault(y => y.TrainingType == TrainingType.WordMeaning);
                if (training == null)
                {
                    training = new WordTrainingInfoEntity.WordTrainingProgressItemEntity()
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

            CustomValidationHelper.Validate(entities);
            await _dataRepository.UpdateManyAsync(entities);
        }

        public async Task<MeaningWordTrainingDto> GetTrainingItemsForMeaningWordAsync(string userId, string collectionId, int limit)
        {
            const int meaningsPerWord = 5;
            var random = new Random();

            var predicate = PredicateBuilder.New<WordEntity>(x =>
                x.UserId == userId &&
                x.Word != null &&
                x.Meaning != null &&
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

            var entities = await _dataRepository.GetManyAsync<WordEntity>(predicate, 0, limit);

            // find words list for each entity
            var trainingItems = new List<MeaningWordTrainingItemDto>();
            foreach (var entity in entities)
            {
                var possibleOptions = new List<MeaningWordTrainingOptionDto>()
                {
                    // correct meaning
                    new MeaningWordTrainingOptionDto()
                    {
                        Value = entity.Word,
                        IsCorrect = true,
                    }
                };

                // other similar words
                // v1: search from other words with the same language
                long otherWordsCount = await _dataRepository.CountAllAsync<WordEntity>(x => x.WordLanguageCode == entity.WordLanguageCode && x.Id != entity.Id && x.Word != null);
                int otherWordsCountInt = otherWordsCount > int.MaxValue ? int.MaxValue : (int)otherWordsCount;
                int otherWordsLimit = meaningsPerWord - 1;
                int otherWordsOffset = random.Next(0, otherWordsCountInt - otherWordsLimit);
                var otherWords = await _dataRepository.GetManyAsync<WordEntity>(
                    x => x.WordLanguageCode == entity.WordLanguageCode && x.Id != entity.Id && x.Word != null,
                    otherWordsOffset,
                    otherWordsLimit
                );
                possibleOptions.AddRange(otherWords.Select(x => new MeaningWordTrainingOptionDto()
                {
                    Value = x.Word,
                    IsCorrect = false,
                }));

                // find accross 
                trainingItems.Add(new MeaningWordTrainingItemDto()
                {
                    Word = _mapper.Map<WordDto>(entity),
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
            if (results.TrainingType != TrainingType.MeaningWord)
            {
                throw new BadRequestException("Incorrect training type passed!");
            }

            var ids = results.ItemsResults.Select(x => x.ItemId);
            var entities = (await _dataRepository.GetManyAsync<WordEntity>(
                x => x.UserId == userId && ids.Contains(x.Id)
            )).ToList();

            var infoAttribute = TrainingTypeHelper.GetAttribute(TrainingType.MeaningWord);

            entities = entities.Select(x =>
            {
                bool isCorrect = results.ItemsResults.Any(y => y.ItemId == x.Id && y.IsCorrect);

                x.TrainingInfo = x.TrainingInfo ?? new WordTrainingInfoEntity();
                x.TrainingInfo.Trainings = x.TrainingInfo.Trainings ?? new List<WordTrainingInfoEntity.WordTrainingProgressItemEntity>();

                var training = x.TrainingInfo.Trainings.FirstOrDefault(y => y.TrainingType == TrainingType.MeaningWord);
                if (training == null)
                {
                    training = new WordTrainingInfoEntity.WordTrainingProgressItemEntity()
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

            CustomValidationHelper.Validate(entities);
            await _dataRepository.UpdateManyAsync(entities);
        }

        public async Task<MatchWordsTrainingDto> GetTrainingItemsForMatchWordsAsync(string userId, string collectionId)
        {
            const int matchWordsCount = 5;
            const int additionalOptionsCount = 3;
            var random = new Random();

            var predicate = PredicateBuilder.New<WordEntity>(x =>
                x.UserId == userId &&
                x.Word != null &&
                x.Meaning != null &&
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

            var entities = await _dataRepository.GetManyAsync<WordEntity>(predicate, 0, matchWordsCount);

            if (!entities.Any())
            {
                return new MatchWordsTrainingDto();
            }

            var entitiesIds = entities.Select(x => x.Id).ToList();
            string languageCode = entities.First().WordLanguageCode;

            // other similar words
            // v1: search from other words with the same language
            long otherWordsCount = await _dataRepository.CountAllAsync<WordEntity>(x => x.WordLanguageCode == languageCode && !entitiesIds.Contains(x.Id) && x.Word != null && x.Meaning != null);
            int otherWordsCountInt = otherWordsCount > int.MaxValue ? int.MaxValue : (int)otherWordsCount;
            int otherWordsLimit = additionalOptionsCount;
            int otherWordsOffset = random.Next(0, otherWordsCountInt - otherWordsLimit);
            var otherWords = await _dataRepository.GetManyAsync<WordEntity>(
                x => x.WordLanguageCode == languageCode && !entitiesIds.Contains(x.Id) && x.Word != null && x.Meaning != null,
                otherWordsOffset,
                otherWordsLimit
            );

            // build options
            var possibleOptions = entities.Select(x => new MatchWordsTrainingPossibleOptionDto()
            {
                Value = x.Meaning,
                CorrectForWordId = x.Id,
            });

            // add other words options to complicate training
            possibleOptions = possibleOptions.Concat(
                 otherWords.Select(x => new MatchWordsTrainingPossibleOptionDto()
                 {
                     Value = x.Meaning,
                     CorrectForWordId = null,
                 })
            );

            // shuffle
            entities = entities.Shuffle();
            possibleOptions = possibleOptions.Shuffle();

            var result = new MatchWordsTrainingDto
            {
                Items = entities.Select(x => new MatchWordsTrainingItemDto()
                {
                    Word = _mapper.Map<WordDto>(x),
                    CorrectOptionId = possibleOptions.First(y => y.CorrectForWordId == x.Id).RandomId,
                }),
                PossibleOptions = possibleOptions,
            };
            return result;
        }

        public async Task SaveTrainingResultsForMatchWordsAsync(string userId, MatchWordsTrainingResultDto results)
        {
            if (results.TrainingType != TrainingType.MatchWords)
            {
                throw new BadRequestException("Incorrect training type passed!");
            }

            var ids = results.ItemsResults.Select(x => x.ItemId);
            var entities = (await _dataRepository.GetManyAsync<WordEntity>(
                x => x.UserId == userId && ids.Contains(x.Id)
            )).ToList();

            var infoAttribute = TrainingTypeHelper.GetAttribute(TrainingType.MatchWords);

            entities = entities.Select(x =>
            {
                bool isCorrect = results.ItemsResults.Any(y => y.ItemId == x.Id && y.IsCorrect);

                x.TrainingInfo = x.TrainingInfo ?? new WordTrainingInfoEntity();
                x.TrainingInfo.Trainings = x.TrainingInfo.Trainings ?? new List<WordTrainingInfoEntity.WordTrainingProgressItemEntity>();

                var training = x.TrainingInfo.Trainings.FirstOrDefault(y => y.TrainingType == TrainingType.MatchWords);
                if (training == null)
                {
                    training = new WordTrainingInfoEntity.WordTrainingProgressItemEntity()
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

            CustomValidationHelper.Validate(entities);
            await _dataRepository.UpdateManyAsync(entities);
        }

        public async Task<BuildWordsTrainingDto> GetTrainingItemsForBuildWordsAsync(string userId, string collectionId, int limit)
        {
            var predicate = PredicateBuilder.New<WordEntity>(x =>
                x.UserId == userId &&
                x.Word != null &&
                x.Meaning != null &&
                (
                    x.TrainingInfo == null ||
                    !x.TrainingInfo.Trainings.Any() ||
                    (
                        x.TrainingInfo != null &&
                        !x.TrainingInfo.IsTrained &&
                        x.TrainingInfo.Trainings.Any(y => y.TrainingType == TrainingType.BuildWords && y.Progress < 1 && y.NextTrainingdAt <= DateTime.UtcNow)
                    )
                )
            );

            if (collectionId != null)
            {
                predicate.And(x => x.CustomCollectionIds.Contains(collectionId));
            }

            var entities = await _dataRepository.GetManyAsync<WordEntity>(predicate, 0, limit);


            var result = new BuildWordsTrainingDto
            {
               Items = entities.Select(x => new BuildWordTrainingItemDto()
               {
                   Word = _mapper.Map<WordDto>(x),
                   WordParts = x.Word.ToLowerInvariant().ToCharArray().Shuffle(),
                   CorrectWordParts = x.Word.ToLowerInvariant().ToCharArray(),
                   CorrectAnswer = x.Word.ToLowerInvariant(),
               }),
            };
            return result;
        }

        public async Task SaveTrainingResultsForBuildWordsAsync(string userId, BuildWordsTrainingResultDto results)
        {
            if(results.TrainingType != TrainingType.BuildWords)
            {
                throw new BadRequestException("Incorrect training type passed!");
            }

            var ids = results.ItemsResults.Select(x => x.WordId);
            var entities = (await _dataRepository.GetManyAsync<WordEntity>(
                x => x.UserId == userId && ids.Contains(x.Id)
            )).ToList();

            var infoAttribute = TrainingTypeHelper.GetAttribute(TrainingType.BuildWords);

            entities = entities.Select(x =>
            {
                bool isCorrect = results.ItemsResults.Any(y => y.WordId == x.Id && y.Answer?.ToLowerInvariant() == x.Word.ToLowerInvariant());

                x.TrainingInfo = x.TrainingInfo ?? new WordTrainingInfoEntity();
                x.TrainingInfo.Trainings = x.TrainingInfo.Trainings ?? new List<WordTrainingInfoEntity.WordTrainingProgressItemEntity>();

                var training = x.TrainingInfo.Trainings.FirstOrDefault(y => y.TrainingType == TrainingType.BuildWords);
                if (training == null)
                {
                    training = new WordTrainingInfoEntity.WordTrainingProgressItemEntity()
                    {
                        TrainingType = TrainingType.BuildWords,
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

            CustomValidationHelper.Validate(entities);
            await _dataRepository.UpdateManyAsync(entities);
        }

        public async Task<ListenWordsTrainingDto> GetTrainingItemsForListenWordsAsync(string userId, string collectionId, int limit)
        {
            var predicate = PredicateBuilder.New<WordEntity>(x =>
                x.UserId == userId &&
                x.Word != null &&
                x.Meaning != null &&
                (
                    x.TrainingInfo == null ||
                    !x.TrainingInfo.Trainings.Any() ||
                    (
                        x.TrainingInfo != null &&
                        !x.TrainingInfo.IsTrained &&
                        x.TrainingInfo.Trainings.Any(y => y.TrainingType == TrainingType.ListenWords && y.Progress < 1 && y.NextTrainingdAt <= DateTime.UtcNow)
                    )
                )
            );

            if (collectionId != null)
            {
                predicate.And(x => x.CustomCollectionIds.Contains(collectionId));
            }

            var entities = await _dataRepository.GetManyAsync<WordEntity>(predicate, 0, limit);


            var result = new ListenWordsTrainingDto
            {
                Items = entities.Select(x => new ListenWordsTrainingItemDto()
                {
                    Word = _mapper.Map<WordDto>(x),
                    WordPronunciationAudioUrl = null, // TODO
                    CorrectAnswer = x.Word.ToLowerInvariant(),
                }),
            };
            return result;
        }

        public async Task SaveTrainingResultsForListenWordsAsync(string userId, ListenWordsTrainingResultDto results)
        {
            if (results.TrainingType != TrainingType.ListenWords)
            {
                throw new BadRequestException("Incorrect training type passed!");
            }

            var ids = results.ItemsResults.Select(x => x.WordId);
            var entities = (await _dataRepository.GetManyAsync<WordEntity>(
                x => x.UserId == userId && ids.Contains(x.Id)
            )).ToList();

            var infoAttribute = TrainingTypeHelper.GetAttribute(TrainingType.ListenWords);

            entities = entities.Select(x =>
            {
                bool isCorrect = results.ItemsResults.Any(y => y.WordId == x.Id && y.Answer?.ToLowerInvariant() == x.Word.ToLowerInvariant());

                x.TrainingInfo = x.TrainingInfo ?? new WordTrainingInfoEntity();
                x.TrainingInfo.Trainings = x.TrainingInfo.Trainings ?? new List<WordTrainingInfoEntity.WordTrainingProgressItemEntity>();

                var training = x.TrainingInfo.Trainings.FirstOrDefault(y => y.TrainingType == TrainingType.ListenWords);
                if (training == null)
                {
                    training = new WordTrainingInfoEntity.WordTrainingProgressItemEntity()
                    {
                        TrainingType = TrainingType.ListenWords,
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

            CustomValidationHelper.Validate(entities);
            await _dataRepository.UpdateManyAsync(entities);
        }

        #endregion
    }
}
