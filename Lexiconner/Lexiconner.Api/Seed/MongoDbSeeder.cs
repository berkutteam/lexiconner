using Lexiconner.Api.ImportAndExport;
using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence.Repositories.Base;
using Lexiconner.Persistence.Repositories.MongoDb;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.Seed
{
    public class MongoDbSeeder : ISeeder
    {
        private readonly IConfiguration _configuration;
        private readonly IWordTxtImporter _wordTxtImporter;
        private readonly IMongoRepository _mongoRepository;
        private readonly IIdentityRepository _identityRepository;

        public MongoDbSeeder(
            IConfiguration configuration, 
            IWordTxtImporter wordTxtImporter, 
            IMongoRepository mongoRepository,
            IIdentityRepository identityRepository
        )
        {
            _configuration = configuration;
            _wordTxtImporter = wordTxtImporter;
            _mongoRepository = mongoRepository;
            _identityRepository = identityRepository;
        }

        public async Task Seed()
        {
            Console.WriteLine($"Seeding db...");

            // seed imported data for marked users
            var usersWithImport = await _identityRepository.GetManyAsync<ApplicationUserEntity>(x => x.IsImportInitialData);
            Parallel.ForEach(usersWithImport, user =>
            {
                if(!_mongoRepository.AnyAsync<StudyItemEntity>(x => x.UserId == user.Id).GetAwaiter().GetResult())
                {
                    var words = _wordTxtImporter.Import().GetAwaiter().GetResult();
                    var entities = words.Select(x => new StudyItemEntity
                    {
                        UserId = user.Id,
                        Title = x.Word,
                        Description = x.Description,
                        ExampleText = x.ExampleText,
                        Tags = x.Tags,
                    });
                    _mongoRepository.AddAsync(entities).GetAwaiter().GetResult();
                }
            });

            Console.WriteLine($"Seed finished.");
        }
    }
}
