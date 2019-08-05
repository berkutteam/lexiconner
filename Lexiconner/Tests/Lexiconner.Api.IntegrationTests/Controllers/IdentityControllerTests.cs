using FluentAssertions;
using Lexiconner.Api.IntegrationTests.Auth;
using Lexiconner.Api.IntegrationTests.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Lexiconner.Api.IntegrationTests.Controllers
{
    public class IdentityControllerTests : TestBase
    {
        public IdentityControllerTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact(DisplayName = "Should return user id (sub claim from token)")]
        public async Task Get()
        {
            var result = await _apiUtil.PingAsync();
            result.Should().Be("pong");

            var userEntity = await _dataUtil.CreateUserAsync();
            var userInfoEntity = await _dataUtil.CreateUserInfoAsync(userEntity.Id);
            var accessToken = TestAuthenticationHelper.GenerateAccessToken(userEntity);

            var userId = await _apiUtil.GetIdentityAsync(accessToken);

            userId.Should().Be(userEntity.Id);
            userInfoEntity.IdentityUserId.Should().Be(userEntity.Id);
        }
    }
}
