using FluentAssertions;
using Lexiconner.Api.Models;
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

        #region Ping

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
    }
}
