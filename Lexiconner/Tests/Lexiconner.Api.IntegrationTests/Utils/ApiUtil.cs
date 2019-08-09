using FluentAssertions;
using Lexiconner.Api.DTOs.StudyItemsTrainings;
using Lexiconner.Api.Models;
using Lexiconner.Api.Models.RequestModels;
using Lexiconner.Api.Models.ResponseModels;
using Lexiconner.Domain.Entitites;
using Lexiconner.Infrastructure.Tests.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace Lexiconner.Api.IntegrationTests.Utils
{
    public class ApiUtil<TStartup> where TStartup : class
    {
        private readonly CustomWebApplicationFactory<TStartup> _factory;
        protected readonly HttpClient _client;
        protected readonly HttpUtil _httpUtil;

        public ApiUtil(CustomWebApplicationFactory<TStartup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _httpUtil = new HttpUtil(_client);
        }

        #region Ping

        public async Task<string> PingAsync()
        {
            var httpResponse = await _httpUtil.GetAsync($"/api/v2/ping");
            _httpUtil.EnsureSuccessStatusCode(httpResponse);

            string stringResponse = await httpResponse.Content.ReadAsStringAsync();

            stringResponse.Should().NotBeNull();

            return stringResponse;
        }

        #endregion

        #region Identity

        public async Task<string> GetIdentityAsync(string accessToken)
        {
            var httpResponse = await _httpUtil.GetAsync($"/api/v2/identity", accessToken);
            _httpUtil.EnsureSuccessStatusCode(httpResponse);

            string stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<BaseApiResponseModel<string>>(stringResponse);

            responseModel.Should().NotBeNull();
            responseModel.Ok.Should().BeTrue();
            responseModel.Data.Should().NotBeNull();

            return responseModel.Data;
        }

        #endregion

        #region User info

        //public async Task<UserInfoDto> GetUserInfoAsync(string accessToken, string userId)
        //{
        //    var httpResponse = await _httpUtil.GetAsync($"/api/v1/userinfo/{userId}", accessToken);
        //    _httpUtil.EnsureSuccessStatusCode(httpResponse);

        //    string stringResponse = await httpResponse.Content.ReadAsStringAsync();
        //    var responseModel = JsonConvert.DeserializeObject<BaseResponseModel<UserInfoDto>>(stringResponse);

        //    responseModel.Should().NotBeNull();
        //    responseModel.Ok.Should().BeTrue();
        //    responseModel.Data.Should().NotBeNull();

        //    return responseModel.Data;
        //}

        //public async Task<UserInfoDto> UpdateUserInfoAsync(string accessToken, string userId, UserInfoUpdateDto dto)
        //{
        //    var httpResponse = await _httpUtil.PutJsonAsync($"/api/v1/userinfo/{userId}", dto, accessToken);
        //    _httpUtil.EnsureSuccessStatusCode(httpResponse);

        //    string stringResponse = await httpResponse.Content.ReadAsStringAsync();
        //    var responseModel = JsonConvert.DeserializeObject<BaseResponseModel<UserInfoDto>>(stringResponse);

        //    responseModel.Should().NotBeNull();
        //    responseModel.Ok.Should().BeTrue();
        //    responseModel.Data.Should().NotBeNull();

        //    return responseModel.Data;
        //}

        #endregion

        #region Study items

        public async Task<GetAllResponseModel<StudyItemEntity>> GetStudyItemsAsync(string accessToken, GetAllRequestModel model)
        {
            var httpResponse = await _httpUtil.GetAsync($"/api/v2/studyitems?Search={model.Search}&Offset={model.Offset}&Limit={model.Limit}", accessToken);
            _httpUtil.EnsureSuccessStatusCode(httpResponse);

            string stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<BaseApiResponseModel<GetAllResponseModel<StudyItemEntity>>>(stringResponse);

            responseModel.Should().NotBeNull();
            responseModel.Ok.Should().BeTrue();
            responseModel.Data.Should().NotBeNull();

            return responseModel.Data;
        }

        public async Task<StudyItemEntity> GetStudyItemByIdAsync(string accessToken, string id)
        {
            var httpResponse = await _httpUtil.GetAsync($"/api/v2/studyitems/{id}", accessToken);
            _httpUtil.EnsureSuccessStatusCode(httpResponse);

            string stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<BaseApiResponseModel<StudyItemEntity>>(stringResponse);

            responseModel.Should().NotBeNull();
            responseModel.Ok.Should().BeTrue();
            responseModel.Data.Should().NotBeNull();

            return responseModel.Data;
        }

        public async Task<StudyItemEntity> CreateStudyItemAsync(string accessToken, StudyItemEntity dto)
        {
            var httpResponse = await _httpUtil.PostJsonAsync($"/api/v2/studyitems", dto, accessToken);
            _httpUtil.EnsureSuccessStatusCode(httpResponse);

            string stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<BaseApiResponseModel<StudyItemEntity>>(stringResponse);

            responseModel.Should().NotBeNull();
            responseModel.Ok.Should().BeTrue();
            responseModel.Data.Should().NotBeNull();

            return responseModel.Data;
        }

        public async Task<StudyItemEntity> UpdateStudyItemAsync(string accessToken, string id, StudyItemEntity dto)
        {
            var httpResponse = await _httpUtil.PutJsonAsync($"/api/v2/studyitems/{id}", dto, accessToken);
            _httpUtil.EnsureSuccessStatusCode(httpResponse);

            string stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<BaseApiResponseModel<StudyItemEntity>>(stringResponse);

            responseModel.Should().NotBeNull();
            responseModel.Ok.Should().BeTrue();
            responseModel.Data.Should().NotBeNull();

            return responseModel.Data;
        }

        public async Task DeleteStudyItemAsync(string accessToken, string id)
        {
            var httpResponse = await _httpUtil.DeleteAsync($"/api/v2/studyitems/{id}",accessToken);
            _httpUtil.EnsureSuccessStatusCode(httpResponse);

            string stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<BaseApiResponseModel<object>>(stringResponse);

            responseModel.Should().NotBeNull();
            responseModel.Ok.Should().BeTrue();
            responseModel.Data.Should().BeNull();
        }

        #endregion


        #region Study items trainings

        public async Task<FlashCardsTrainingDto> FlashcardsStartTraining(string accessToken, int limit)
        {
            var httpResponse = await _httpUtil.GetAsync($"/api/v2/studyitems/trainings/flashcards?limit={limit}", accessToken);
            _httpUtil.EnsureSuccessStatusCode(httpResponse);

            string stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<BaseApiResponseModel<FlashCardsTrainingDto>>(stringResponse);

            responseModel.Should().NotBeNull();
            responseModel.Ok.Should().BeTrue();
            responseModel.Data.Should().NotBeNull();

            return responseModel.Data;
        }
        public async Task FlashcardsSaveTrainingResults(string accessToken, FlashCardsTrainingResultDto dto)
        {
            var httpResponse = await _httpUtil.PostJsonAsync($"/api/v2/studyitems/trainings/flashcards/save", dto, accessToken);
            _httpUtil.EnsureSuccessStatusCode(httpResponse);

            string stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<BaseApiResponseModel<object>>(stringResponse);

            responseModel.Should().NotBeNull();
            responseModel.Ok.Should().BeTrue();
            responseModel.Data.Should().BeNull();
        }

        #endregion

    }
}
