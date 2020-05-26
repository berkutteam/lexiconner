using FluentAssertions;
using Lexiconner.Api.DTOs.StudyItemsTrainings;
using Lexiconner.Api.IntegrationTests.Auth;
using Lexiconner.Api.IntegrationTests.Utils;
using Lexiconner.Domain.Attributes;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Lexiconner.Api.IntegrationTests.Controllers
{
    public class StudyItemsTrainingsControllerTests : TestBase
    {
        public StudyItemsTrainingsControllerTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact(DisplayName = "Should return training statistics")]
        public async Task GetTrainingStatistics()
        {
            await GetAndSaveTrainings(out string accessToken);

            var statsDto = await _apiUtil.GetTrainingStatistics(accessToken);

            statsDto.TotalItemCount.Should().NotBe(0);
            statsDto.TrainedItemCount.Should().NotBe(1);
            statsDto.OnTrainingItemCount.Should().NotBe(0);

            statsDto.TrainingStats.Count.Should().Be(3);
            statsDto.TrainingStats.First(x => x.TrainingType == TrainingType.FlashCards).OnTrainingItemCount.Should().NotBe(0);
            statsDto.TrainingStats.First(x => x.TrainingType == TrainingType.FlashCards).TrainedItemCount.Should().Be(0);
            statsDto.TrainingStats.First(x => x.TrainingType == TrainingType.WordMeaning).OnTrainingItemCount.Should().Be(0);
            statsDto.TrainingStats.First(x => x.TrainingType == TrainingType.WordMeaning).TrainedItemCount.Should().Be(0);
            statsDto.TrainingStats.First(x => x.TrainingType == TrainingType.MeaningWord).OnTrainingItemCount.Should().Be(0);
            statsDto.TrainingStats.First(x => x.TrainingType == TrainingType.MeaningWord).TrainedItemCount.Should().Be(0);
        }


        #region Flashcards

        [Fact(DisplayName = "Should return training items for flashcards")]
        public async Task GetTrainingItemsForFlashCards()
        {
            var userEntity = await _dataUtil.CreateUserAsync();
            var userInfoEntity = await _dataUtil.CreateUserInfoAsync(userEntity.Id);
            var accessToken = TestAuthenticationHelper.GenerateAccessToken(userEntity);

            int count = 15;
            int limit = count;
            var studyItemsEntities = await _dataUtil.CreateStudyItemsAsync(userEntity.Id, count);

            var trainingDto = await _apiUtil.FlashcardsTrainingStart(accessToken, limit);

            trainingDto.TrainingType.Should().Be(TrainingType.FlashCards);
            trainingDto.Items.Should().NotBeEmpty();
            trainingDto.Items.Count.Should().Be(limit);
            trainingDto.Items.ToList().ForEach(x =>
            {
                Assert.Contains(studyItemsEntities, y => y.Id == x.Id);
            });
        }

        [Fact(DisplayName = "Should save training results for flashcards")]
        public async Task FlashcardsSaveTrainingResults()
        {
            await GetAndSaveTrainings(out string accessToken);
        }

        private Task GetAndSaveTrainings(out string accessToken)
        {
            var userEntity = _dataUtil.CreateUserAsync().GetAwaiter().GetResult();
            var userInfoEntity = _dataUtil.CreateUserInfoAsync(userEntity.Id).GetAwaiter().GetResult();
            accessToken = TestAuthenticationHelper.GenerateAccessToken(userEntity);

            int count = 15;
            int limit = count;
            var studyItemsEntities = _dataUtil.CreateStudyItemsAsync(userEntity.Id, count).GetAwaiter().GetResult();

            var trainingDto = _apiUtil.FlashcardsTrainingStart(accessToken, limit).GetAwaiter().GetResult();


            var requestDto = new FlashCardsTrainingResultDto()
            {

                TrainingType = TrainingType.FlashCards,
                ItemsResults = trainingDto.Items.Select((x, i) => new FlashCardsTrainingResultDto.FlashCardsTrainingResultForItemDto
                {
                    ItemId = x.Id,
                    IsCorrect = i % 2 == 0 ? true : false,
                }).ToList()
            };
            var correctItemIds = requestDto.ItemsResults.Where(x => x.IsCorrect).Select(x => x.ItemId).ToList();
            var inCorrectItemIds = requestDto.ItemsResults.Where(x => !x.IsCorrect).Select(x => x.ItemId).ToList();

            _apiUtil.FlashcardsTrainingSave(accessToken, requestDto).GetAwaiter().GetResult();

            studyItemsEntities = _dataUtil.GetStudyItemsAsync(userEntity.Id).GetAwaiter().GetResult();

            var infoAttribute = TrainingTypeHelper.GetAttribute(TrainingType.FlashCards);
            studyItemsEntities
                .Where(x => requestDto.ItemsResults.Any(y => y.ItemId == x.Id))
                .ToList()
                .ForEach(x =>
                {
                    Assert.Contains(x.TrainingInfo.Trainings, y => y.TrainingType == TrainingType.FlashCards);
                    Assert.Contains(x.TrainingInfo.Trainings, y => y.LastTrainingdAt < DateTime.UtcNow);
                    Assert.Contains(x.TrainingInfo.Trainings, y => y.NextTrainingdAt > DateTime.UtcNow);

                    if (correctItemIds.Contains(x.Id))
                    {
                        Assert.Contains(x.TrainingInfo.Trainings, y => y.Progress == infoAttribute.CorrectAnswerProgressRate);
                    }
                    else if (inCorrectItemIds.Contains(x.Id))
                    {
                        Assert.Contains(x.TrainingInfo.Trainings, y => y.Progress == 0);
                    }
                });

            return Task.CompletedTask;
        }


        #endregion

    }
}
