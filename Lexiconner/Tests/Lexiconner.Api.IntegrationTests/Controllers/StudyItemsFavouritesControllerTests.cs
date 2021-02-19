using FluentAssertions;
using Lexiconner.Api.DTOs.WordsTrainings;
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
    public class WordsFavouritesControllerTests : TestBase
    {
        public WordsFavouritesControllerTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact(DisplayName = "Should add item to favourites")]
        public async Task AddToFavourites()
        {
            await PrepareTestUser();

            var wordsEntityBefore = await _dataUtil.CreateWordAsync(_userEntity.Id);

            await _apiUtil.AddToFavouritesAsync(_accessToken, wordsEntityBefore.Id);

            var wordsEntityAfter = await _dataUtil.GetWordAsync(wordsEntityBefore.Id);

            wordsEntityBefore.IsFavourite.Should().BeFalse();
            wordsEntityAfter.IsFavourite.Should().BeTrue();
        }

        [Fact(DisplayName = "Should delte item from favourites")]
        public async Task DeleteFromFavourites()
        {
            await PrepareTestUser();

            var wordsEntityBefore = await _dataUtil.CreateWordAsync(_userEntity.Id);

            await _apiUtil.AddToFavouritesAsync(_accessToken, wordsEntityBefore.Id);
            wordsEntityBefore = await _dataUtil.GetWordAsync(wordsEntityBefore.Id);

            await _apiUtil.DeleteFromFavouritesAsync(_accessToken, wordsEntityBefore.Id);

            var wordsEntityAfter = await _dataUtil.GetWordAsync(wordsEntityBefore.Id);

            wordsEntityBefore.IsFavourite.Should().BeTrue();
            wordsEntityAfter.IsFavourite.Should().BeFalse();
        }
    }
}
