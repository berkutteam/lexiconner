using Bogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Lexiconner.Persistence.UnitTests.Utils;

namespace Lexiconner.Persistence.UnitTests
{
    public abstract class TestBase : IClassFixture<TestFixture>, IDisposable
    {
        protected readonly TestFixture _fixture;
        protected readonly Faker _faker;
        protected readonly ApplicationSettings _config;

        public TestBase(TestFixture fixture)
        {
            _fixture = fixture;
            _faker = new Faker();
            _config = _fixture.ServiceProvider.GetService<IOptions<ApplicationSettings>>().Value;


            // Do "global" initialization here; Called before every test method.
        }

        public void Dispose()
        {
            // Do "global" teardown here; Called after every test method.
        }
    }
}
