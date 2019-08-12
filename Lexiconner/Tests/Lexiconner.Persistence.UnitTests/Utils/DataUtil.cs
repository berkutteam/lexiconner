using Bogus;
using Lexiconner.Persistence.Repositories.MongoDb;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lexiconner.Persistence.UnitTests.Utils
{
    public class DataUtil
    {
        private readonly ApplicationSettings _config;
        private readonly IMongoDataRepository _mongoDataRepository;
        private readonly Faker _faker;

        public DataUtil(
            IOptions<ApplicationSettings> config,
            IMongoDataRepository mongoDataRepository
        )
        {
            _config = config.Value;
            _mongoDataRepository = mongoDataRepository;
            _faker = new Faker();
        }


        #region Cleanup

        /// <summary>
        /// Cleans up data after test class run
        /// </summary>
        /// <returns></returns>
        public async Task CleanUpAsync()
        {
            await CleanUpMongoDbAsync();
        }

        public async Task CleanUpMongoDbAsync()
        {
            await Task.WhenAll(new List<Task>()
            {
                _mongoDataRepository.DropDatabaseAsync()
            });
        }

        #endregion
    }
}
