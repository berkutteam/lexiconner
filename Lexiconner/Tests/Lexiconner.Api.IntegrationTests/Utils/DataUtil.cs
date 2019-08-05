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
using Lexiconner.Persistence.Repositories.Base;
using Lexiconner.Domain.Entitites;

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
        private readonly IMongoRepository _dataRepository;
        private readonly IIdentityRepository _identityRepository;
        private readonly Faker _faker;

        public DataUtil(CustomWebApplicationFactory<TStartup> factory)
        {
            _factory = factory;
            _config = factory.Server.Host.Services.GetService<IOptions<ApplicationSettings>>().Value;
            _dataRepository = factory.Server.Host.Services.GetService<IMongoRepository>();
            _identityRepository = factory.Server.Host.Services.GetService<IIdentityRepository>();
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
            await _identityRepository.AddAsync<ApplicationUserEntity>(entity);

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


        #region Study items

        public StudyItemEntity PrepareStudyItemCreateDto()
        {
            return new StudyItemEntity
            {
                Title = _faker.Lorem.Word(),
                Description = _faker.Lorem.Text(),
                ExampleText = _faker.Lorem.Text(),
                Tags = new List<string>
                    {
                        _faker.Lorem.Word(),
                        _faker.Lorem.Word(),
                    },
            };
        }

        public StudyItemEntity PrepareStudyItemUpdateDto(StudyItemEntity dto)
        {
            return new StudyItemEntity
            {
                Id = dto.Id,
                Title = _faker.Lorem.Word(),
                Description = _faker.Lorem.Word()
            };
        }

        public async Task<StudyItemEntity> GetStudyItemAsync(
            string id
        )
        {
            return await _dataRepository.GetOneAsync<StudyItemEntity>(x => x.Id == id);
        }

        public async Task<List<StudyItemEntity>> CreateStudyItemsAsync(
            string userId,
            int count = 10
        )
        {
            var entities = Enumerable.Range(0, count).Select(x =>
            {
                return new StudyItemEntity()
                {
                    UserId = userId,
                    Title = _faker.Lorem.Word(),
                    Description = _faker.Lorem.Text(),
                    ExampleText = _faker.Lorem.Text(),
                    Tags = new List<string>
                    {
                        _faker.Lorem.Word(),
                        _faker.Lorem.Word(),
                    },
                    Image = null
                };
            }).ToList();

            await _dataRepository.AddManyAsync<StudyItemEntity>(entities);

            return entities;
        }

        public async Task<StudyItemEntity> CreateStudyItemAsync(
            string userId
        )
        {
            var entity =new StudyItemEntity()
            {
                UserId = userId,
                Title = _faker.Lorem.Word(),
                Description = _faker.Lorem.Text(),
                ExampleText = _faker.Lorem.Text(),
                Tags = new List<string>
                    {
                        _faker.Lorem.Word(),
                        _faker.Lorem.Word(),
                    },
                Image = null
            };

            await _dataRepository.AddAsync<StudyItemEntity>(entity);

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
                _dataRepository.DropDatabaseAsync()
            });
        }

        #endregion
    }
}
