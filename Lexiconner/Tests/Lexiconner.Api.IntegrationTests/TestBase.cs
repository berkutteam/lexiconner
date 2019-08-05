using Bogus;
using FluentAssertions;
using Lexiconner.Api.IntegrationTests.Utils;
using Lexiconner.Infrastructure.Tests.Utils;
using Lexiconner.Persistence.Repositories.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace Lexiconner.Api.IntegrationTests
{
    /// <summary>
    /// Base test class
    /// </summary>
    public abstract class TestBase : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    {
        protected readonly CustomWebApplicationFactory<Startup> _factory;
        protected readonly HttpClient _client;
        protected readonly DataUtil<Startup> _dataUtil;
        protected readonly ApiUtil<Startup> _apiUtil;
        protected readonly HttpUtil _httpUtil;
        protected readonly Faker _faker;
        protected readonly ApplicationSettings _config;
        protected readonly IMongoRepository _dataRepository;

        public TestBase(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _dataUtil = new DataUtil<Startup>(factory);
            _apiUtil = new ApiUtil<Startup>(factory);
            _httpUtil = new HttpUtil(_client);
            _faker = new Faker();
            _config = factory.Server.Host.Services.GetService<IOptions<ApplicationSettings>>().Value;
            _dataRepository = factory.Server.Host.Services.GetService<IMongoRepository>();

            // Do "global" initialization here; Called before every test method.
        }

        protected ApplicationSettings GetAppConfig()
        {
            if (_factory.Server == null)
            {
                throw new Exception("Create client first");
            }

            var config = _factory.Server.Host.Services.GetRequiredService<IOptions<ApplicationSettings>>().Value;
            return config;
        }

        public void Dispose()
        {
            // Do "global" teardown here; Called after every test method.
        }
    }
}
