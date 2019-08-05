using FluentAssertions;
using Lexiconner.Api.IntegrationTests.Auth;
using Lexiconner.Api.IntegrationTests.Utils;
using Lexiconner.Api.Models.RequestModels;
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
            var userEntity = await _dataUtil.CreateUserAsync();
            var userInfoEntity = await _dataUtil.CreateUserInfoAsync(userEntity.Id);
            var accessToken = TestAuthenticationHelper.GenerateAccessToken(userEntity);

            int count = 7;
            var studyItemsEntities = await _dataUtil.CreateStudyItemsAsync(userEntity.Id, count);

            var response = await _apiUtil.GetStudyItemsAsync(accessToken, new GetAllRequestModel { Offset = 0, Limit = count });

            response.TotalCount.Should().Be(count);
            response.Items.Should().NotBeEmpty();
            response.Items.ToList().ForEach(x =>
            {
                Assert.Contains(studyItemsEntities, y => y.Id == x.Id);
            });
        }

        [Fact(DisplayName = "Should return item by id")]
        public async Task GetById()
        {
            var userEntity = await _dataUtil.CreateUserAsync();
            var userInfoEntity = await _dataUtil.CreateUserInfoAsync(userEntity.Id);
            var accessToken = TestAuthenticationHelper.GenerateAccessToken(userEntity);

            var studyItemEntity = await _dataUtil.CreateStudyItemAsync(userEntity.Id);

            var response = await _apiUtil.GetStudyItemByIdAsync(accessToken, studyItemEntity.Id);

            response.Id.Should().Be(studyItemEntity.Id);
        }

        [Fact(DisplayName = "Should create item")]
        public async Task Create()
        {
            var userEntity = await _dataUtil.CreateUserAsync();
            var userInfoEntity = await _dataUtil.CreateUserInfoAsync(userEntity.Id);
            var accessToken = TestAuthenticationHelper.GenerateAccessToken(userEntity);

            var createDto = _dataUtil.PrepareStudyItemCreateDto();

            var apiCreatedDto = await _apiUtil.CreateStudyItemAsync(accessToken, createDto);

            var dbEntity = await _dataUtil.GetStudyItemAsync(apiCreatedDto.Id);
            dbEntity.Should().NotBeNull();

            apiCreatedDto.Id.Should().Be(createDto.Id);
            apiCreatedDto.UserId.Should().Be(userEntity.Id);
        }

        [Fact(DisplayName = "Should update item")]
        public async Task Update()
        {
            var userEntity = await _dataUtil.CreateUserAsync();
            var userInfoEntity = await _dataUtil.CreateUserInfoAsync(userEntity.Id);
            var accessToken = TestAuthenticationHelper.GenerateAccessToken(userEntity);

            var createDto = _dataUtil.PrepareStudyItemCreateDto();
            var apiCreatedDto = await _apiUtil.CreateStudyItemAsync(accessToken, createDto);

            var updateDto = _dataUtil.PrepareStudyItemUpdateDto(apiCreatedDto);
            var apiUpdatedDto = await _apiUtil.UpdateStudyItemAsync(accessToken, apiCreatedDto.Id, updateDto);

            var dbEntity = await _dataUtil.GetStudyItemAsync(apiCreatedDto.Id);
            dbEntity.Should().NotBeNull();

            dbEntity.Title.Should().NotBe(apiCreatedDto.Title);
            dbEntity.Title.Should().Be(apiUpdatedDto.Title);
        }

        [Fact(DisplayName = "Should delete item")]
        public async Task Delete()
        {
            var userEntity = await _dataUtil.CreateUserAsync();
            var userInfoEntity = await _dataUtil.CreateUserInfoAsync(userEntity.Id);
            var accessToken = TestAuthenticationHelper.GenerateAccessToken(userEntity);

            var createDto = _dataUtil.PrepareStudyItemCreateDto();
            var apiCreatedDto = await _apiUtil.CreateStudyItemAsync(accessToken, createDto);

            await _apiUtil.DeleteStudyItemAsync(accessToken, apiCreatedDto.Id);

            var dbEntity = await _dataUtil.GetStudyItemAsync(apiCreatedDto.Id);
            dbEntity.Should().BeNull();
        }
    }
}
