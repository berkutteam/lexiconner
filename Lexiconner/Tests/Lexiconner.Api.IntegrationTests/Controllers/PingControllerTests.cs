using FluentAssertions;
using Lexiconner.Api.IntegrationTests.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Lexiconner.Api.IntegrationTests.Controllers
{
    public class PingControllerTests : TestBase
    {
        public PingControllerTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact(DisplayName = "Should return pong")]
        public async Task Get()
        {
            var result = await _apiUtil.PingAsync();
            result.Should().Be("pong");
        }
    }
}
