using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUlid;
using NUlid.Rng;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using Newtonsoft.Json;
using Lexiconner.Persistence.UnitTests.Utils;
using Lexiconner.Persistence.Repositories.Base;
using Lexiconner.Persistence.Exceptions;
using Lexiconner.Domain.Entitites;
using Lexiconner.Infrastructure.Tests.Assertions;
using Lexiconner.Domain.Entitites.Testing;

namespace Lexiconner.Persistence.UnitTests
{
    public class MongoDbRepositoryTests : TestBase
    {
        protected readonly IMongoRepository _mongoDbDataRepository;

        public MongoDbRepositoryTests(TestFixture fixture) : base(fixture)
        {
            _mongoDbDataRepository = _fixture.ServiceProvider.GetService<IMongoRepository>();
        }

        [Fact(DisplayName = "Should throw MongoDbCollectionException if collection is not in config")]
        public async Task CollectionInConfig()
        {
            string collectionId = "collectionThatIsNotInApplicationSettings";
            string id = Ulid.NewUlid(new CSUlidRng()).ToString();

            await CustomAssert.AllThrowsAsync<MongoDbCollectionException>(new List<Task>()
            {
                _mongoDbDataRepository.CollectionExistsAsync<UnexistingEntity>(),
                _mongoDbDataRepository.InitializeCollectionAsync<UnexistingEntity>(),
                _mongoDbDataRepository.GetAllAsync<UnexistingEntity>(),
                _mongoDbDataRepository.GetAllAsync<UnexistingEntity>(0, 10),
                _mongoDbDataRepository.GetOneAsync<UnexistingEntity>(x => true),
                _mongoDbDataRepository.GetManyAsync<UnexistingEntity>(x => true),
                _mongoDbDataRepository.GetManyAsync<UnexistingEntity>(x => true, 0, 10),
                _mongoDbDataRepository.AddAsync<UnexistingEntity>(new UnexistingEntity()),
                _mongoDbDataRepository.AddManyAsync<UnexistingEntity>(new List<UnexistingEntity>()),
                _mongoDbDataRepository.UpdateAsync<UnexistingEntity>(new UnexistingEntity()),
                _mongoDbDataRepository.UpdateManyAsync<UnexistingEntity>(new List<UnexistingEntity>()),
                _mongoDbDataRepository.DeleteAsync<UnexistingEntity>(x => true),
                _mongoDbDataRepository.DeleteAllAsync<UnexistingEntity>(),
                _mongoDbDataRepository.DeleteNDocumentsAsync<UnexistingEntity>(x => true, x => x.Id, 100),
                _mongoDbDataRepository.ExistsAsync<UnexistingEntity>(x => true),
                _mongoDbDataRepository.CountAllAsync<UnexistingEntity>(x => true),
            });
        }


        //[Fact(DisplayName = "Should throw AccessDeniedException if entity inherits IReadOnlyEntity")]
        //public async Task CheckIReadOnlyEntity()
        //{
        //    string collectionId = _config.CosmosDb.Collections.Telemetry;

        //    // test as read only
        //    await CustomAssert.AllThrowsAsync<AccessDeniedException>(new List<Task>()
        //    {
        //        _mongoDbDataRepository.CreateDocumentIfNotExistsAsync<ReadOnlyTelemetryEntityTest>(collectionId, new ReadOnlyTelemetryEntityTest(), x => true),
        //        _mongoDbDataRepository.CreateDocumentAsync<ReadOnlyTelemetryEntityTest>(collectionId, new ReadOnlyTelemetryEntityTest()),
        //        _mongoDbDataRepository.UpdateDocumentAsync<ReadOnlyTelemetryEntityTest>(collectionId, new ReadOnlyTelemetryEntityTest()),
        //        _mongoDbDataRepository.UpsertDocumentAsync<ReadOnlyTelemetryEntityTest>(collectionId, new ReadOnlyTelemetryEntityTest()),
        //        _mongoDbDataRepository.DeleteDocumentAsync<ReadOnlyTelemetryEntityTest>(collectionId, new ReadOnlyTelemetryEntityTest()) ,
        //        _mongoDbDataRepository.DeleteDocumentsAsync<ReadOnlyTelemetryEntityTest>(collectionId, x => true),
        //    });
        //}


        #region IMongoRepository

        //[Fact(DisplayName = "Should return GetMongoClient")]
        //public void GetMongoClient()
        //{
        //    var client = _mongoDbDataRepository.GetMongoClient();
        //    client.Should().NotBeNull();
        //}

        [Fact(DisplayName = "Should return IMongoDatabase")]
        public void GetMongoDatabase()
        {
            var database = _mongoDbDataRepository.GetDatabase();
            database.Should().NotBeNull();
        }

        #endregion


        #region Collection actions

        [Fact(DisplayName = "Should check that collection exists")]
        public async Task CollectionExistsAsync()
        {
            var exists = await _mongoDbDataRepository.CollectionExistsAsync<SimpleTestEntity>();
            exists.Should().BeTrue();
        }


        [Fact(DisplayName = "Should check initialize collection")]
        public async Task InitializeCollectionAsync()
        {
            await _mongoDbDataRepository.InitializeCollectionAsync<SimpleTestEntity>();
            var exists = await _mongoDbDataRepository.CollectionExistsAsync<SimpleTestEntity>();
            exists.Should().BeTrue();
        }

        //[Fact(DisplayName = "Should delete collection")]
        //public async Task DeleteDocumentCollection()
        //{
        //    await _mongoDbDataRepository.DeleteDocumentCollectionAsync(_config.CosmosDb.Collections.Telemetry);
        //    var exists = await _mongoDbDataRepository.DocumentCollectionExistsAsync<TelemetryEntityTest>(_config.CosmosDb.Collections.Telemetry);
        //    exists.Should().BeFalse();
        //}

        #endregion


        #region Get

        [Fact(DisplayName = "Should return all documents")]
        public async Task GetAll()
        {
            await _mongoDbDataRepository.DeleteAllAsync<SimpleTestEntity>();

            var docs = Enumerable.Range(0, 3).Select(x =>
            {
                var doc = new SimpleTestEntity
                {
                    Title = Ulid.NewUlid(new CSUlidRng()).ToString(),
                };
                _mongoDbDataRepository.AddAsync<SimpleTestEntity>(doc).GetAwaiter().GetResult();
                return doc;
            }).ToList();
            var docIds = docs.Select(x => x.Id);

            var dbDocs = await _mongoDbDataRepository.GetAllAsync<SimpleTestEntity>();

            Assert.True(docIds.All(x => dbDocs.Any(y => y.Id == x)));
        }

        [Fact(DisplayName = "Should return all documents with offset, limit, search")]
        public async Task GetAllWithOffsetLimitSearch()
        {
            await _mongoDbDataRepository.DeleteAllAsync<SimpleTestEntity>();

            var docs = Enumerable.Range(0, 3).Select(x =>
            {
                var doc = new SimpleTestEntity
                {
                    Title = Ulid.NewUlid(new CSUlidRng()).ToString(),
                };
                _mongoDbDataRepository.AddAsync<SimpleTestEntity>(doc).GetAwaiter().GetResult();
                return doc;
            }).ToList();
            var docIds = docs.Select(x => x.Id);

            var dbDocs = await _mongoDbDataRepository.GetAllAsync<SimpleTestEntity>(0, docs.Count);

            Assert.True(docIds.All(x => dbDocs.Any(y => y.Id == x)));
        }

        [Fact(DisplayName = "Should return one document")]
        public async Task GetOne()
        {
            var doc = new SimpleTestEntity
            {
                Title = Ulid.NewUlid(new CSUlidRng()).ToString(),
            };
            await _mongoDbDataRepository.AddAsync<SimpleTestEntity>(doc);
            var dbDoc = await _mongoDbDataRepository.GetOneAsync<SimpleTestEntity>(x => x.Id == doc.Id);
            dbDoc.Should().NotBeNull();
            dbDoc.Id.Should().Be(doc.Id);
        }

        [Fact(DisplayName = "Should return many documents")]
        public async Task GetMany()
        {
            await _mongoDbDataRepository.DeleteAllAsync<SimpleTestEntity>();

            var docs = Enumerable.Range(0, 5).Select(x =>
            {
                var doc = new SimpleTestEntity
                {
                    Title = Ulid.NewUlid(new CSUlidRng()).ToString(),
                };
                _mongoDbDataRepository.AddAsync<SimpleTestEntity>(doc).GetAwaiter().GetResult();
                return doc;
            }).ToList();
            var docIds = docs.Select(x => x.Id);

            var dbDocs = await _mongoDbDataRepository.GetManyAsync<SimpleTestEntity>(x => true);

            Assert.True(docIds.All(x => dbDocs.Any(y => y.Id == x)));
        }

        [Fact(DisplayName = "Should return many documents with offset, limit, search")]
        public async Task GetManyWithOffsetLimitSearch()
        {
            await _mongoDbDataRepository.DeleteAllAsync<SimpleTestEntity>();

            var docs = Enumerable.Range(0, 5).Select(x =>
            {
                var doc = new SimpleTestEntity
                {
                    Title = Ulid.NewUlid(new CSUlidRng()).ToString(),
                };
                _mongoDbDataRepository.AddAsync<SimpleTestEntity>(doc).GetAwaiter().GetResult();
                return doc;
            }).ToList();
            var docIds = docs.Select(x => x.Id);

            var dbDocs = await _mongoDbDataRepository.GetManyAsync<SimpleTestEntity>(x => true, 0, docs.Count - 2);

            Assert.True(dbDocs.All(x => docIds.Any(y => y == x.Id)));
        }

        #endregion


        #region Create, Update

        [Fact(DisplayName = "Should create document")]
        public async Task Add()
        {
            var doc = new SimpleTestEntity
            {
                Title = Ulid.NewUlid(new CSUlidRng()).ToString(),
            };
            await _mongoDbDataRepository.AddAsync<SimpleTestEntity>(doc);
            var dbDoc = await _mongoDbDataRepository.GetOneAsync<SimpleTestEntity>(x => x.Id == doc.Id);

            dbDoc.Should().NotBeNull();
            dbDoc.Id.Should().Be(doc.Id);
        }

        [Fact(DisplayName = "Should create many documents")]
        public async Task AddMany()
        {
            await _mongoDbDataRepository.DeleteAllAsync<SimpleTestEntity>();

            var docs = Enumerable.Range(0, 5).Select(x =>
            {
                var doc = new SimpleTestEntity
                {
                    Title = Ulid.NewUlid(new CSUlidRng()).ToString(),
                };
                return doc;
            }).ToList();
            var docIds = docs.Select(x => x.Id);
          
            await _mongoDbDataRepository.AddManyAsync<SimpleTestEntity>(docs);
            var dbDocs = await _mongoDbDataRepository.GetAllAsync<SimpleTestEntity>();

            Assert.True(docIds.All(x => dbDocs.Any(y => y.Id == x)));
        }

        [Fact(DisplayName = "Should update document")]
        public async Task Update()
        {
            var doc = new SimpleTestEntity
            {
                Title = Ulid.NewUlid(new CSUlidRng()).ToString(),
            };
            await _mongoDbDataRepository.AddAsync<SimpleTestEntity>(doc);
            var docCreated = doc;
            var docUpdated = JsonConvert.DeserializeObject<SimpleTestEntity>(JsonConvert.SerializeObject(docCreated));
            docUpdated.Title = Ulid.NewUlid(new CSUlidRng()).ToString();

            await _mongoDbDataRepository.UpdateAsync<SimpleTestEntity>(docUpdated);

            var dbDocInitial = await _mongoDbDataRepository.GetOneAsync<SimpleTestEntity>(x => x.Title == docCreated.Title);
            var dbDocUpdated = await _mongoDbDataRepository.GetOneAsync<SimpleTestEntity>(x => x.Title == docUpdated.Title);

            dbDocInitial.Should().BeNull();
            dbDocUpdated.Should().NotBeNull();
            dbDocUpdated.Title.Should().NotBe(docCreated.Title);
        }

        [Fact(DisplayName = "Should update many documents")]
        public async Task UpdateMany()
        {
            await _mongoDbDataRepository.DeleteAllAsync<SimpleTestEntity>();

            var docs = Enumerable.Range(0, 5).Select(x =>
            {
                var doc = new SimpleTestEntity
                {
                    Title = Ulid.NewUlid(new CSUlidRng()).ToString(),
                };
                return doc;
            }).ToList();

            await _mongoDbDataRepository.AddManyAsync<SimpleTestEntity>(docs);

            var docsCreated = JsonConvert.DeserializeObject<IEnumerable<SimpleTestEntity>>(JsonConvert.SerializeObject(docs));
            var docsUpdated = JsonConvert.DeserializeObject<IEnumerable<SimpleTestEntity>>(JsonConvert.SerializeObject(docs));
            docsUpdated = docsUpdated.Select(x =>
            {
                x.Title = Ulid.NewUlid(new CSUlidRng()).ToString();
                return x;
            }).ToList();

            var docsCreatedTitles = docsCreated.Select(x => x.Title).ToList();
            var docsUpdatedTitles = docsUpdated.Select(x => x.Title).ToList();

            await _mongoDbDataRepository.UpdateManyAsync<SimpleTestEntity>(docsUpdated);

            var dbDocsInitial = await _mongoDbDataRepository.GetManyAsync<SimpleTestEntity>(x => docsCreatedTitles.Contains(x.Title));
            var dbDocsUpdated = await _mongoDbDataRepository.GetManyAsync<SimpleTestEntity>(x => docsUpdatedTitles.Contains(x.Title));

            dbDocsInitial.Should().BeEmpty();
            dbDocsUpdated.Should().NotBeEmpty();
            dbDocsUpdated.ToList().ForEach(dbDocUpdated =>
            {
                dbDocUpdated.Title.Should().NotBe(docsCreated.First(x => x.Id == dbDocUpdated.Id).Title);
            });
        }

        #endregion


        #region Delete

        [Fact(DisplayName = "Should delete document by predicate")]
        public async Task Delete()
        {
            var doc = new SimpleTestEntity
            {
                Title = Ulid.NewUlid(new CSUlidRng()).ToString(),
            };
            await _mongoDbDataRepository.AddAsync<SimpleTestEntity>(doc);

            await _mongoDbDataRepository.DeleteAsync<SimpleTestEntity>(x => x.Id == doc.Id);

            var dbDoc = await _mongoDbDataRepository.GetOneAsync<SimpleTestEntity>(x => x.Id == doc.Id);

            dbDoc.Should().BeNull();
        }

        [Fact(DisplayName = "Should delete all documents")]
        public async Task DeleteAll()
        {
            var docs = Enumerable.Range(0, 3).Select(x =>
            {
                var doc = new SimpleTestEntity
                {
                    Title = Ulid.NewUlid(new CSUlidRng()).ToString(),
                };
                return doc;
            }).ToList();
            await _mongoDbDataRepository.AddManyAsync<SimpleTestEntity>(docs);

            await _mongoDbDataRepository.DeleteAllAsync<SimpleTestEntity>();

            var dbDocs = await _mongoDbDataRepository.GetAllAsync<SimpleTestEntity>();

            dbDocs.Should().BeEmpty();
        }


        [Fact(DisplayName = "Should delete N documents documents")]
        public async Task DeleteNDcouments()
        {
            // TODO: fix
            await _mongoDbDataRepository.DeleteAllAsync<SimpleTestEntity>();

            var docs = Enumerable.Range(0, 10).Select(x =>
            {
                var doc = new SimpleTestEntity
                {
                    Title = Ulid.NewUlid(new CSUlidRng()).ToString(),
                    Order = x,
                };
                return doc;
            }).ToList();
            await _mongoDbDataRepository.AddManyAsync<SimpleTestEntity>(docs);

            var dbDocs = (await _mongoDbDataRepository.GetAllAsync<SimpleTestEntity>()).ToList();

            Expression<Func<SimpleTestEntity, bool>> predicate = (x) => x.Order < 5;
            int beforeCount = dbDocs.Count(x => predicate.Compile()(x));
            int deleteCount = 2;
            await _mongoDbDataRepository.DeleteNDocumentsAsync<SimpleTestEntity>(predicate, x => x.CreatedAt, deleteCount: deleteCount);

            var dbDocs2 = await _mongoDbDataRepository.GetAllAsync<SimpleTestEntity>();
            int afterCount = dbDocs2.Count(x => predicate.Compile()(x));

            dbDocs2.Should().NotBeEmpty();
            dbDocs2.Count().Should().Be(dbDocs.Count() - deleteCount);
            afterCount.Should().Be(beforeCount - deleteCount);
        }

        #endregion


        #region Exists, Count, Any

        [Fact(DisplayName = "Should check if document exists")]
        public async Task DocumentExistsAsync()
        {
            var doc = new SimpleTestEntity
            {
                Title = Ulid.NewUlid(new CSUlidRng()).ToString(),
            };
            await _mongoDbDataRepository.AddAsync<SimpleTestEntity>(doc);

            var exists1 = await _mongoDbDataRepository.ExistsAsync<SimpleTestEntity>(x => x.Title == doc.Title);
            var exists2 = await _mongoDbDataRepository.ExistsAsync<SimpleTestEntity>(x => x.Title == "FAKE");

            exists1.Should().BeTrue();
            exists2.Should().BeFalse();
        }

        [Fact(DisplayName = "Should count all documents in collection")]
        public async Task CountAllAsync()
        {
            // delete all first
            await _mongoDbDataRepository.DeleteAllAsync<SimpleTestEntity>();

            var docs = Enumerable.Range(0, 3).Select(x =>
            {
                var doc = new SimpleTestEntity
                {
                    Title = Ulid.NewUlid(new CSUlidRng()).ToString(),
                };
                return doc;
            }).ToList();
            await _mongoDbDataRepository.AddManyAsync<SimpleTestEntity>(docs);

            var count = await _mongoDbDataRepository.CountAllAsync<SimpleTestEntity>(x => true);

            count.Should().Be(docs.Count);
        }

        [Fact(DisplayName = "Should count documents by predicate")]
        public async Task DocumentCountByPredicate()
        {
            // delete all first
            await _mongoDbDataRepository.DeleteAllAsync<SimpleTestEntity>();

            var docs = Enumerable.Range(0, 10).Select(x =>
            {
                var doc = new SimpleTestEntity
                {
                    Title = Ulid.NewUlid(new CSUlidRng()).ToString(),
                    Order = x
                };
                return doc;
            }).ToList();
            await _mongoDbDataRepository.AddManyAsync<SimpleTestEntity>(docs);

            var count = await _mongoDbDataRepository.CountAllAsync<SimpleTestEntity>(x => x.Order > docs.Count / 2);

            count.Should().Be(docs.Count(x => x.Order > docs.Count / 2));
        }

        #endregion
    }
}
