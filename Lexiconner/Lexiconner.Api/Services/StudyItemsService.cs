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
using System.Threading.Tasks;

namespace Lexiconner.Api.Services
{
    public interface IStudyItemsService
    {
        Task<TrainingsStatisticsDto> GetTrainingStatistics(string userId);
        Task<FlashCardsTrainingDto> GetTrainingItemsForFlashCards(string userId);
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

        public async Task<FlashCardsTrainingDto> GetTrainingItemsForFlashCards(string userId)
        {
            var entities = await _mongoRepository.GetManyAsync<StudyItemEntity>(
                x => x.UserId == userId && 
                (x.TrainingInfo == null || (x.TrainingInfo != null && x.TrainingInfo.Trainings.Any(y => y.TrainingType == TrainingType.FlashCards && y.Progress != 1 && y.NextTrainingdAt >= DateTime.UtcNow)))
            );

            return new FlashCardsTrainingDto
            {
                Items = entities.ToList(),
            };
        }

        public async Task SaveTrainingResultsForFlashCards(string userId, FlashCardsTrainingResultDto results)
        {
            var ids = results.ItemsResults.Select(x => x.ItemId);
            var entities = await _mongoRepository.GetManyAsync<StudyItemEntity>(
                x => x.UserId == userId && ids.Contains(x.Id)
            );

            var infoAttribute = results.TrainingType.GetType().GetCustomAttributes(typeof(TrainingTypeInfoAttribute), false).First() as TrainingTypeInfoAttribute;
            
            entities = entities.Select(x =>
            {
                bool isCorrect = results.ItemsResults.Any(y => y.ItemId == x.Id && y.IsCorrect);

                var training = x.TrainingInfo.Trainings.FirstOrDefault(y => y.TrainingType == TrainingType.FlashCards) ?? new StudyItemTrainingInfoEntity.StudyItemTrainingProgressItemEntity();

                if (isCorrect)
                {
                    training.Progress = training.Progress + infoAttribute.CorrectAnswerProgressRate;
                    training.Progress = Math.Min(training.Progress, 1);
                }
                else
                {
                    training.Progress = training.Progress - infoAttribute.WrongAnswerProgressRate;
                    training.Progress = Math.Max(training.Progress, 0);
                }

                return x;
            });

            await _mongoRepository.UpdateManyAsync(entities);
        }
    }
}
