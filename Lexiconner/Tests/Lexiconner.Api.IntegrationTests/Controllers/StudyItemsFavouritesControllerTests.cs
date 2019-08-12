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
    public class StudyItemsFavouritesControllerTests : TestBase
    {
        public StudyItemsFavouritesControllerTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact(DisplayName = "Should add item to favourites")]
        public async Task AddToFavourites()
        {
            await PrepareTestUser();

            var studyItemsEntityBefore = await _dataUtil.CreateStudyItemAsync(_userEntity.Id);

            await _apiUtil.AddToFavouritesAsync(_accessToken, studyItemsEntityBefore.Id);

            var studyItemsEntityAfter = await _dataUtil.GetStudyItemAsync(studyItemsEntityBefore.Id);

            studyItemsEntityBefore.IsFavourite.Should().BeFalse();
            studyItemsEntityAfter.IsFavourite.Should().BeTrue();
        }

        [Fact(DisplayName = "Should delte item from favourites")]
        public async Task DeleteFromFavourites()
        {
            await PrepareTestUser();

            var studyItemsEntityBefore = await _dataUtil.CreateStudyItemAsync(_userEntity.Id);

            await _apiUtil.AddToFavouritesAsync(_accessToken, studyItemsEntityBefore.Id);
            studyItemsEntityBefore = await _dataUtil.GetStudyItemAsync(studyItemsEntityBefore.Id);

            await _apiUtil.DeleteFromFavouritesAsync(_accessToken, studyItemsEntityBefore.Id);

            var studyItemsEntityAfter = await _dataUtil.GetStudyItemAsync(studyItemsEntityBefore.Id);

            studyItemsEntityBefore.IsFavourite.Should().BeTrue();
            studyItemsEntityAfter.IsFavourite.Should().BeFalse();
        }
    }
}
