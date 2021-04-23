using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUlid;
using Bogus;
using System.Linq;
using NUlid.Rng;
using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.Repositories.MongoDb;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Domain.Entitites.General;

namespace Lexiconner.Api.IntegrationTests.Utils
{
    /// <summary>
    /// Responsible for preparing data and CRUD operations
    /// </summary>
    /// <typeparam name="TStartup"></typeparam>
    public class DataUtil<TStartup> where TStartup : class
    {
        private readonly CustomWebApplicationFactory<TStartup> _factory;
        private readonly ApplicationSettings _config;
        private readonly IIdentityDataRepository _identityDataRepository;
        private readonly IDataRepository _dataRepository;
        private readonly IMongoDataRepository _mongoIdentityDataRepository;
        private readonly IMongoDataRepository _mongoDataRepository;
        private readonly Faker _faker;

        public DataUtil(CustomWebApplicationFactory<TStartup> factory)
        {
            _factory = factory;
            _config = factory.Server.Host.Services.GetService<IOptions<ApplicationSettings>>().Value;
            _identityDataRepository = factory.Server.Host.Services.GetService<IIdentityDataRepository>();
            _dataRepository = factory.Server.Host.Services.GetService<IDataRepository>();
            _mongoIdentityDataRepository = _identityDataRepository as IMongoDataRepository;
            _mongoDataRepository = _dataRepository as IMongoDataRepository;
            _faker = new Faker();
        }

        #region Users

        public async Task<ApplicationUserEntity> CreateUserAsync(
            List<string> roles = null
        )
        {
            var entity = new ApplicationUserEntity()
            {
                Name = _faker.Name.FullName(),
                Email = _faker.Internet.Email(),
                EmailConfirmed = true,
                UserName = _faker.Internet.UserName(),
                PhoneNumber = _faker.Phone.PhoneNumber(),
                PhoneNumberConfirmed = true,
                Roles = roles ?? new List<string>(),
            };

            // do not set password
            // create user directly
            await _identityDataRepository.AddAsync<ApplicationUserEntity>(entity);

            return entity;
        }

        public async Task<UserInfoEntity> CreateUserInfoAsync(
            string userId
        )
        {
            var entity = new UserInfoEntity
            {
                IdentityUserId = userId,
            };

            await _dataRepository.AddAsync<UserInfoEntity>(entity);

            return entity;
        }

        #region User info

        public async Task<UserInfoEntity> GetUserInfoByUserId(string userId)
        {
            return await _dataRepository.GetOneAsync<UserInfoEntity>(x => x.IdentityUserId == userId);
        }

        #endregion

        #endregion


        #region Words

        public WordEntity PrepareWordCreateDto()
        {
            return new WordEntity
            {
                Word = _faker.Lorem.Word(),
                Meaning = _faker.Lorem.Text(),
                Examples = new List<string>() { _faker.Lorem.Text() },
                Tags = new List<string>
                    {
                        _faker.Lorem.Word(),
                        _faker.Lorem.Word(),
                    },
            };
        }

        public WordEntity PrepareWordUpdateDto(WordEntity dto)
        {
            return new WordEntity
            {
                Id = dto.Id,
                Word = _faker.Lorem.Word(),
                Meaning = _faker.Lorem.Word()
            };
        }

        public async Task<WordEntity> GetWordAsync(
            string id
        )
        {
            return await _dataRepository.GetOneAsync<WordEntity>(x => x.Id == id);
        }

        public async Task<List<WordEntity>> GetWordsAsync(
           string userId
       )
        {
            return (await _dataRepository.GetManyAsync<WordEntity>(x => x.UserId == userId)).ToList();
        }

        public async Task<List<WordEntity>> CreateWordsAsync(
            string userId,
            int count = 10
        )
        {
            var entities = Enumerable.Range(0, count).Select(x =>
            {
                return new WordEntity()
                {
                    UserId = userId,
                    Word = _faker.Lorem.Word(),
                    Meaning = _faker.Lorem.Text(),
                    Examples = new List<string>() { _faker.Lorem.Text() },
                    Tags = new List<string>
                    {
                        _faker.Lorem.Word(),
                        _faker.Lorem.Word(),
                    },
                    Images = new List<GeneralImageEntity>(),
                };
            }).ToList();

            await _dataRepository.AddManyAsync<WordEntity>(entities);

            return entities;
        }

        public async Task<WordEntity> CreateWordAsync(
            string userId
        )
        {
            var entity =new WordEntity()
            {
                UserId = userId,
                Word = _faker.Lorem.Word(),
                Meaning = _faker.Lorem.Text(),
                Examples = new List<string>() { _faker.Lorem.Text() },
                Tags = new List<string>
                    {
                        _faker.Lorem.Word(),
                        _faker.Lorem.Word(),
                    },
                Images = new List<GeneralImageEntity>(),
            };

            await _dataRepository.AddAsync<WordEntity>(entity);

            return entity;
        }

        #endregion


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
                _mongoIdentityDataRepository.DropDatabaseAsync(),
                _mongoDataRepository.DropDatabaseAsync()
            });
        }

        #endregion
    }
}
