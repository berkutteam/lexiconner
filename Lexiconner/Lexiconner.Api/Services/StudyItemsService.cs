using Lexiconner.Api.DTOs.StudyItemsTrainings;
using Lexiconner.Application.Services;
using Lexiconner.Domain.Attributes;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Enums;
using Lexiconner.Persistence.Repositories.Base;
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
        Task<TrainingsStatisticsDto> GetTrainingStatistics(string userId);
        Task<FlashCardsTrainingDto> GetTrainingItemsForFlashCards(string userId, int limit);
        Task SaveTrainingResultsForFlashCards(string userId, FlashCardsTrainingResultDto results);
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

        public async Task<TrainingsStatisticsDto> GetTrainingStatistics(string userId)
        {
            long totalItemCount = await _mongoRepository.CountAllAsync<StudyItemEntity>(x => x.UserId == userId);
            long onTrainingItemCount = await _mongoRepository.CountAllAsync<StudyItemEntity>(x => x.UserId == userId && x.TrainingInfo != null && x.TrainingInfo.Trainings.Any(y => y.Progress != 1));
            long trainedItemCount = await _mongoRepository.CountAllAsync<StudyItemEntity>(x => x.UserId == userId && x.TrainingInfo != null && x.TrainingInfo.Trainings.All(y => y.Progress == 1));

            Func<string, TrainingType, Task<TrainingsStatisticsDto.TrainingStatisticsItemDto>> getTrainingStatisticsItem = async (_userId, trainingType) =>
            {
                return new TrainingsStatisticsDto.TrainingStatisticsItemDto
                {
                    TrainingType = TrainingType.FlashCards,
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

        public async Task<FlashCardsTrainingDto> GetTrainingItemsForFlashCards(string userId, int limit)
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

        public async Task SaveTrainingResultsForFlashCards(string userId, FlashCardsTrainingResultDto results)
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
    }
}
