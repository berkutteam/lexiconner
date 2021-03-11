using FluentAssertions;
using Lexiconner.Api.IntegrationTests.Auth;
using Lexiconner.Api.IntegrationTests.Utils;
using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Lexiconner.Api.IntegrationTests.Controllers
{
    public class WordsControllerTests : TestBase
    {
        public WordsControllerTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact(DisplayName = "Should return all items")]
        public async Task GetAll()
        {
            await PrepareTestUser();

            int count = 7;
            var wordsEntities = await _dataUtil.CreateWordsAsync(_userEntity.Id, count);

            var response = await _apiUtil.GetWordsAsync(_accessToken, new WordsRequestDto { Offset = 0, Limit = count });

            response.Pagination.TotalCount.Should().Be(count);
            response.Items.Should().NotBeEmpty();
            response.Items.ToList().ForEach(x =>
            {
                Assert.Contains(wordsEntities, y => y.Id == x.Id);
            });
        }

        [Fact(DisplayName = "Should return 0 favorite items")]
        public async Task GetAllFavouritesFalse()
        {
            await PrepareTestUser();

            int count = 7;
            var wordsEntities = await _dataUtil.CreateWordsAsync(_userEntity.Id, count);

            var response = await _apiUtil.GetWordsAsync(_accessToken, new WordsRequestDto { Offset = 0, Limit = count, IsFavourite = true });

            response.Pagination.TotalCount.Should().Be(0);
            response.Items.Should().BeEmpty();
        }

        [Fact(DisplayName = "Should return n favorite items")]
        public async Task GetAllFavouritesTrue()
        {
            await PrepareTestUser();

            int count = 7;
            var wordsEntities = await _dataUtil.CreateWordsAsync(_userEntity.Id, count);

            var favouriteEntities = wordsEntities.Take(4).Select(x =>
            {
                x.IsFavourite = true;
                return x;
            }).ToList();
            await _dataRepository.UpdateManyAsync(favouriteEntities);

            var response = await _apiUtil.GetWordsAsync(_accessToken, new WordsRequestDto { Offset = 0, Limit = 10, IsFavourite = true });

            response.Pagination.TotalCount.Should().Be(favouriteEntities.Count);
            response.Pagination.ReturnedCount.Should().Be(favouriteEntities.Count);
            response.Items.Should().NotBeEmpty();
            response.Items.ToList().ForEach(x =>
            {
                Assert.Contains(favouriteEntities, y => y.Id == x.Id);
            });
        }

        [Fact(DisplayName = "Should search items")]
        public async Task GetAllSearch()
        {
            await PrepareTestUser();

            int count = 7;
            var wordsEntities = await _dataUtil.CreateWordsAsync(_userEntity.Id, count);

            var searchEntities = wordsEntities.Take(3).ToList();
            string search = "sEaRcH";
            searchEntities[0].Word = $"xxx yyy x {search}";
            searchEntities[1].Meaning = $"xxx yyy x {search}fgfrr";
            searchEntities[2].Examples = new List<string>() { $"xxxRRRR444{search}__sd" };
            await _dataRepository.UpdateManyAsync(searchEntities);

            var response = await _apiUtil.GetWordsAsync(_accessToken, new WordsRequestDto { Offset = 0, Limit = 10, Search = search });

            response.Pagination.TotalCount.Should().Be(searchEntities.Count);
            response.Pagination.ReturnedCount.Should().Be(searchEntities.Count);
            response.Items.Count().Should().Be(searchEntities.Count);
            response.Items.ToList().ForEach(x =>
            {
                Assert.Contains(searchEntities, y => y.Id == x.Id);
            });
        }


        [Fact(DisplayName = "Should return item by id")]
        public async Task GetById()
        {
            await PrepareTestUser();

            var wordEntity = await _dataUtil.CreateWordAsync(_userEntity.Id);

            var response = await _apiUtil.GetWordByIdAsync(_accessToken, wordEntity.Id);

            response.Id.Should().Be(wordEntity.Id);
        }

        [Fact(DisplayName = "Should create item")]
        public async Task Create()
        {
            await PrepareTestUser();

            var createDto = _dataUtil.PrepareWordCreateDto();

            var apiCreatedDto = await _apiUtil.CreateWordAsync(_accessToken, createDto);

            var dbEntity = await _dataUtil.GetWordAsync(apiCreatedDto.Id);
            dbEntity.Should().NotBeNull();

            apiCreatedDto.Id.Should().Be(createDto.Id);
            apiCreatedDto.UserId.Should().Be(_userEntity.Id);
        }

        [Fact(DisplayName = "Should update item")]
        public async Task Update()
        {
            await PrepareTestUser();

            var createDto = _dataUtil.PrepareWordCreateDto();
            var apiCreatedDto = await _apiUtil.CreateWordAsync(_accessToken, createDto);

            var updateDto = _dataUtil.PrepareWordUpdateDto(apiCreatedDto);
            var apiUpdatedDto = await _apiUtil.UpdateWordAsync(_accessToken, apiCreatedDto.Id, updateDto);

            var dbEntity = await _dataUtil.GetWordAsync(apiCreatedDto.Id);
            dbEntity.Should().NotBeNull();

            dbEntity.Word.Should().NotBe(apiCreatedDto.Word);
            dbEntity.Word.Should().Be(apiUpdatedDto.Word);
        }

        [Fact(DisplayName = "Should delete item")]
        public async Task Delete()
        {
            await PrepareTestUser();

            var createDto = _dataUtil.PrepareWordCreateDto();
            var apiCreatedDto = await _apiUtil.CreateWordAsync(_accessToken, createDto);

            await _apiUtil.DeleteWordAsync(_accessToken, apiCreatedDto.Id);

            var dbEntity = await _dataUtil.GetWordAsync(apiCreatedDto.Id);
            dbEntity.Should().BeNull();
        }
    }
}
