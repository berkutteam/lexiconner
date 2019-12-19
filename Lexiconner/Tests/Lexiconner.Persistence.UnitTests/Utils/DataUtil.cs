using Bogus;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence.Repositories.MongoDb;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lexiconner.Persistence.UnitTests.Utils
{
    public class DataUtil
    {
        private readonly ApplicationSettings _config;
        protected readonly IDataRepository _dataRepository;
        protected readonly IMongoDataRepository _mongoDataRepository;
        private readonly Faker _faker;

        public DataUtil(
            IOptions<ApplicationSettings> config,
            IDataRepository dataRepository
        )
        {
            _config = config.Value;
            _dataRepository = dataRepository;
            _mongoDataRepository = _dataRepository as IMongoDataRepository;
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
