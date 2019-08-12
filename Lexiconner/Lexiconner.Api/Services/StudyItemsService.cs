﻿using Lexiconner.Api.DTOs;
using Lexiconner.Api.DTOs.StudyItemsTrainings;
using Lexiconner.Api.Models;
using Lexiconner.Application.Services;
using Lexiconner.Domain.Attributes;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Enums;
using Lexiconner.Persistence.Repositories.Base;
using LinqKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Lexiconner.Api.Services
{
    public interface IStudyItemsService
    {
        Task<PaginationResponseDto<StudyItemEntity>> GetAllStudyItemsAsync(string userId, int offset, int limit, StudyItemsSearchFilter searchFilter = null);

        Task<TrainingsStatisticsDto> GetTrainingStatisticsAsync(string userId);
        Task<FlashCardsTrainingDto> GetTrainingItemsForFlashCardsAsync(string userId, int limit);
        Task SaveTrainingResultsForFlashCardsAsync(string userId, FlashCardsTrainingResultDto results);

        Task AddToFavouritesAsync(string userId, IEnumerable<string> itemIds);
        Task DeleteFromFavouritesAsync(string userId, IEnumerable<string> itemIds);
    }

    public class StudyItemsService : IStudyItemsService
    {
        private readonly IMongoRepository _mongoRepository;
        private readonly IImageService _imageService;

        public StudyItemsService(
            IMongoRepository mongoRepository,
            IImageService imageService
        )
        {
            _mongoRepository = mongoRepository;
            _imageService = imageService;
        }

        #region Study items

        public async Task<PaginationResponseDto<StudyItemEntity>> GetAllStudyItemsAsync(string userId, int offset, int limit, StudyItemsSearchFilter searchFilter = null)
        {
            var predicate = PredicateBuilder.New<StudyItemEntity>(x => x.UserId == userId);

            if(searchFilter != null)
            {
                if (!String.IsNullOrEmpty(searchFilter.Search))
                {
                    string search = searchFilter.Search.Trim().ToLower();
                    predicate.And(x => x.Title.ToLower().Contains(search) || x.Description.ToLower().Contains(search) || x.ExampleText.ToLower().Contains(search));
                }
                if (searchFilter.IsFavourite.GetValueOrDefault(false))
                {
                    predicate.And(x => x.IsFavourite);
                }
            }

            var itemsTask = _mongoRepository.GetManyAsync<StudyItemEntity>(predicate, offset, limit);
            var totalTask = _mongoRepository.CountAllAsync<StudyItemEntity>(predicate);

            var total = await totalTask;
            var items = await itemsTask;

            var result = new PaginationResponseDto<StudyItemEntity>
            {
                TotalCount = total,
                ReturnedCount = items.Count(),
                Offset = offset,
                Limit = limit,
                Items = items,
            };

            return result;
        }

        #endregion


        #region Trainings

        public async Task<TrainingsStatisticsDto> GetTrainingStatisticsAsync(string userId)
        {
            long totalItemCount = await _mongoRepository.CountAllAsync<StudyItemEntity>(x => x.UserId == userId);
            long onTrainingItemCount = await _mongoRepository.CountAllAsync<StudyItemEntity>(x => x.UserId == userId && x.TrainingInfo != null && x.TrainingInfo.Trainings.Any(y => y.Progress != 1));
            long trainedItemCount = await _mongoRepository.CountAllAsync<StudyItemEntity>(x => x.UserId == userId && x.TrainingInfo != null && !x.TrainingInfo.Trainings.Any(y => y.Progress != 1));

            Func<string, TrainingType, Task<TrainingsStatisticsDto.TrainingStatisticsItemDto>> getTrainingStatisticsItem = async (_userId, trainingType) =>
            {
                return new TrainingsStatisticsDto.TrainingStatisticsItemDto
                {
                    TrainingType = trainingType,
                    OnTrainingItemCount = await _mongoRepository.CountAllAsync<StudyItemEntity>(
                            x => x.UserId == _userId && x.TrainingInfo != null && x.TrainingInfo.Trainings.Any(y => y.TrainingType == trainingType && y.Progress != 1)
                        ),
                    TrainedItemCount = await _mongoRepository.CountAllAsync<StudyItemEntity>(
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
                }
            };

            return result;
        }

        public async Task<FlashCardsTrainingDto> GetTrainingItemsForFlashCardsAsync(string userId, int limit)
        {
            var entities = await _mongoRepository.GetManyAsync<StudyItemEntity>(
                x => x.UserId == userId && 
                (x.TrainingInfo == null || (x.TrainingInfo != null && x.TrainingInfo.Trainings.Any(y => y.TrainingType == TrainingType.FlashCards && y.Progress < 1 && y.NextTrainingdAt <= DateTime.UtcNow))),
                0,
                limit
            );

            return new FlashCardsTrainingDto
            {
                Items = entities.ToList(),
            };
        }

        public async Task SaveTrainingResultsForFlashCardsAsync(string userId, FlashCardsTrainingResultDto results)
        {
            var ids = results.ItemsResults.Select(x => x.ItemId);
            var entities = (await _mongoRepository.GetManyAsync<StudyItemEntity>(
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

                training.LastTrainingdAt = DateTime.UtcNow;

                if(training.Progress < 1)
                {
                    training.NextTrainingdAt = DateTime.UtcNow.Add(infoAttribute.TrainIntervalTimespan);
                } else
                {
                    training.NextTrainingdAt = DateTime.UtcNow.Add(infoAttribute.TrainIntervalForRepeatTimespan);
                }

                return x;
            }).ToList();

            await _mongoRepository.UpdateManyAsync(entities);
        }

        #endregion


        #region Favourites

        public async Task AddToFavouritesAsync(string userId, IEnumerable<string> itemIds)
        {
            var entities = (await _mongoRepository.GetManyAsync<StudyItemEntity>(x => x.UserId == userId && itemIds.Contains(x.Id))).ToList();

            entities = entities.Select(x =>
            {
                x.IsFavourite = true;
                return x;
            }).ToList();

            await _mongoRepository.UpdateManyAsync<StudyItemEntity>(entities);
        }

        public async Task DeleteFromFavouritesAsync(string userId, IEnumerable<string> itemIds)
        {
            var entities = (await _mongoRepository.GetManyAsync<StudyItemEntity>(x => x.UserId == userId && itemIds.Contains(x.Id))).ToList();

            entities = entities.Select(x =>
            {
                x.IsFavourite = false;
                return x;
            }).ToList();

            await _mongoRepository.UpdateManyAsync<StudyItemEntity>(entities);
        }

        #endregion
    }
}
