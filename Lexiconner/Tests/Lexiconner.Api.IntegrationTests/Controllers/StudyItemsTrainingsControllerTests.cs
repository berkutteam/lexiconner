using FluentAssertions;
using Lexiconner.Api.DTOs.StudyItemsTrainings;
using Lexiconner.Api.IntegrationTests.Auth;
using Lexiconner.Api.IntegrationTests.Utils;
using Lexiconner.Api.Models.RequestModels;
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
           // TODO
        }


        #region Flashcards

        [Fact(DisplayName = "Should return training items for flashcards")]
        public async Task GetTrainingItemsForFlashCards()
        {
            var userEntity = await _dataUtil.CreateUserAsync();
            var userInfoEntity = await _dataUtil.CreateUserInfoAsync(userEntity.Id);
            var accessToken = TestAuthenticationHelper.GenerateAccessToken(userEntity);

            int count = 15;
            var studyItemsEntities = await _dataUtil.CreateStudyItemsAsync(userEntity.Id, count);

            var trainingDto = await _apiUtil.FlashcardsStartTraining(accessToken);

            trainingDto.Items.Should().NotBeEmpty();
            trainingDto.Items.ToList().ForEach(x =>
            {
                Assert.Contains(studyItemsEntities, y => y.Id == x.Id);
            });
        }

        [Fact(DisplayName = "Should save training results for flashcards")]
        public async Task FlashcardsSaveTrainingResults()
        {
            var userEntity = await _dataUtil.CreateUserAsync();
            var userInfoEntity = await _dataUtil.CreateUserInfoAsync(userEntity.Id);
            var accessToken = TestAuthenticationHelper.GenerateAccessToken(userEntity);

            int count = 15;
            var studyItemsEntities = await _dataUtil.CreateStudyItemsAsync(userEntity.Id, count);

            var trainingDto = await _apiUtil.FlashcardsStartTraining(accessToken);

           
            var requestDto = new FlashCardsTrainingResultDto() {

                TrainingType = TrainingType.FlashCards,
                ItemsResults = trainingDto.Items.Select((x, i) => new FlashCardsTrainingResultDto.FlashCardsTrainingResultForItemDto
                {
                    ItemId = x.Id,
                    IsCorrect = i % 2 == 0 ? true : false,
                }).ToList()
            };
            var correctItemIds = requestDto.ItemsResults.Where(x => x.IsCorrect).Select(x => x.ItemId).ToList();
            var inCorrectItemIds = requestDto.ItemsResults.Where(x => !x.IsCorrect).Select(x => x.ItemId).ToList();

            await _apiUtil.FlashcardsSaveTrainingResults(accessToken, requestDto);

            studyItemsEntities = await _dataUtil.GetStudyItemsAsync(userEntity.Id);

            var infoAttribute = TrainingTypeHelper.GetAttribute(TrainingType.FlashCards);
            studyItemsEntities
                .Where(x => requestDto.ItemsResults.Any(y => y.ItemId == x.Id))
                .ToList()
                .ForEach(x =>
            {
                Assert.Contains(x.TrainingInfo.Trainings, y => y.TrainingType == TrainingType.FlashCards);
                Assert.Contains(x.TrainingInfo.Trainings, y => y.LastTrainingdAt < DateTime.UtcNow);
                Assert.Contains(x.TrainingInfo.Trainings, y => y.NextTrainingdAt > DateTime.UtcNow);

                if(correctItemIds.Contains(x.Id)) {
                    Assert.Contains(x.TrainingInfo.Trainings, y => y.Progress == infoAttribute.CorrectAnswerProgressRate);
                }
                else if (inCorrectItemIds.Contains(x.Id)) {
                    Assert.Contains(x.TrainingInfo.Trainings, y => y.Progress == 0);
                }
            });
        }


        #endregion

    }
}
