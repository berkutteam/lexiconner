﻿using FluentAssertions;
using Lexiconner.Api.DTOs.StudyItems;
using Lexiconner.Api.IntegrationTests.Auth;
using Lexiconner.Api.IntegrationTests.Utils;
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
    public class StudyItemsControllerTests : TestBase
    {
        public StudyItemsControllerTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact(DisplayName = "Should return all items")]
        public async Task GetAll()
        {
            await PrepareTestUser();

            int count = 7;
            var studyItemsEntities = await _dataUtil.CreateStudyItemsAsync(_userEntity.Id, count);

            var response = await _apiUtil.GetStudyItemsAsync(_accessToken, new StudyItemsRequestDto { Offset = 0, Limit = count });

            response.TotalCount.Should().Be(count);
            response.Items.Should().NotBeEmpty();
            response.Items.ToList().ForEach(x =>
            {
                Assert.Contains(studyItemsEntities, y => y.Id == x.Id);
            });
        }

        [Fact(DisplayName = "Should return 0 favorite items")]
        public async Task GetAllFavouritesFalse()
        {
            await PrepareTestUser();

            int count = 7;
            var studyItemsEntities = await _dataUtil.CreateStudyItemsAsync(_userEntity.Id, count);

            var response = await _apiUtil.GetStudyItemsAsync(_accessToken, new StudyItemsRequestDto { Offset = 0, Limit = count, IsFavourite = true });

            response.TotalCount.Should().Be(0);
            response.Items.Should().BeEmpty();
        }

        [Fact(DisplayName = "Should return n favorite items")]
        public async Task GetAllFavouritesTrue()
        {
            await PrepareTestUser();

            int count = 7;
            var studyItemsEntities = await _dataUtil.CreateStudyItemsAsync(_userEntity.Id, count);

            var favouriteEntities = studyItemsEntities.Take(4).Select(x =>
            {
                x.IsFavourite = true;
                return x;
            }).ToList();
            await _dataRepository.UpdateManyAsync(favouriteEntities);

            var response = await _apiUtil.GetStudyItemsAsync(_accessToken, new StudyItemsRequestDto { Offset = 0, Limit = 10, IsFavourite = true });

            response.TotalCount.Should().Be(favouriteEntities.Count);
            response.ReturnedCount.Should().Be(favouriteEntities.Count);
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
            var studyItemsEntities = await _dataUtil.CreateStudyItemsAsync(_userEntity.Id, count);

            var searchEntities = studyItemsEntities.Take(3).ToList();
            string search = "sEaRcH";
            searchEntities[0].Title = $"xxx yyy x {search}";
            searchEntities[1].Description = $"xxx yyy x {search}fgfrr";
            searchEntities[2].ExampleText = $"xxxRRRR444{search}__sd";
            await _dataRepository.UpdateManyAsync(searchEntities);

            var response = await _apiUtil.GetStudyItemsAsync(_accessToken, new StudyItemsRequestDto { Offset = 0, Limit = 10, Search = search });

            response.TotalCount.Should().Be(searchEntities.Count);
            response.ReturnedCount.Should().Be(searchEntities.Count);
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

            var studyItemEntity = await _dataUtil.CreateStudyItemAsync(_userEntity.Id);

            var response = await _apiUtil.GetStudyItemByIdAsync(_accessToken, studyItemEntity.Id);

            response.Id.Should().Be(studyItemEntity.Id);
        }

        [Fact(DisplayName = "Should create item")]
        public async Task Create()
        {
            await PrepareTestUser();

            var createDto = _dataUtil.PrepareStudyItemCreateDto();

            var apiCreatedDto = await _apiUtil.CreateStudyItemAsync(_accessToken, createDto);

            var dbEntity = await _dataUtil.GetStudyItemAsync(apiCreatedDto.Id);
            dbEntity.Should().NotBeNull();

            apiCreatedDto.Id.Should().Be(createDto.Id);
            apiCreatedDto.UserId.Should().Be(_userEntity.Id);
        }

        [Fact(DisplayName = "Should update item")]
        public async Task Update()
        {
            await PrepareTestUser();

            var createDto = _dataUtil.PrepareStudyItemCreateDto();
            var apiCreatedDto = await _apiUtil.CreateStudyItemAsync(_accessToken, createDto);

            var updateDto = _dataUtil.PrepareStudyItemUpdateDto(apiCreatedDto);
            var apiUpdatedDto = await _apiUtil.UpdateStudyItemAsync(_accessToken, apiCreatedDto.Id, updateDto);

            var dbEntity = await _dataUtil.GetStudyItemAsync(apiCreatedDto.Id);
            dbEntity.Should().NotBeNull();

            dbEntity.Title.Should().NotBe(apiCreatedDto.Title);
            dbEntity.Title.Should().Be(apiUpdatedDto.Title);
        }

        [Fact(DisplayName = "Should delete item")]
        public async Task Delete()
        {
            await PrepareTestUser();

            var createDto = _dataUtil.PrepareStudyItemCreateDto();
            var apiCreatedDto = await _apiUtil.CreateStudyItemAsync(_accessToken, createDto);

            await _apiUtil.DeleteStudyItemAsync(_accessToken, apiCreatedDto.Id);

            var dbEntity = await _dataUtil.GetStudyItemAsync(apiCreatedDto.Id);
            dbEntity.Should().BeNull();
        }
    }
}