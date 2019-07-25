using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.Repositories.Base;
using Lexiconner.Seed.Models;
using Lexiconner.Seed.Seed.ImportAndExport;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Seed.Seed
{
    public class SeedServiceDevelopmentLocalhost : ISeedService
    {
        private readonly ILogger<ISeedService> _logger;
        private readonly IWordTxtImporter _wordTxtImporter;
        private readonly IMongoRepository _mongoRepository;
        private readonly IIdentityRepository _identityRepository;

        public SeedServiceDevelopmentLocalhost(
            ILogger<ISeedService> logger,
            IWordTxtImporter wordTxtImporter,
            IMongoRepository mongoRepository,
            IIdentityRepository identityRepository
        )
        {
            _logger = logger;
            _wordTxtImporter = wordTxtImporter;
            _mongoRepository = mongoRepository;
            _identityRepository = identityRepository;
        }

        public Task RemoveDatabaseAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SeedAsync()
        {
            _logger.LogInformation("Start seeding data...");

            _logger.LogInformation("Users...");
            // TODO
            _logger.LogInformation("Users Done.");

            // seed imported data for marked users
            _logger.LogInformation("StudyItems...");
            var usersWithImport = await _identityRepository.GetManyAsync<ApplicationUserEntity>(x => x.IsImportInitialData);
            IEnumerable<WordImportModel> wordImports = null;
            Parallel.ForEach(usersWithImport, user =>
            {
                if (!_mongoRepository.AnyAsync<StudyItemEntity>(x => x.UserId == user.Id).GetAwaiter().GetResult())
                {
                    wordImports = wordImports ?? _wordTxtImporter.Import().GetAwaiter().GetResult();
                    var entities = wordImports.Select(x => new StudyItemEntity
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
            _logger.LogInformation("StudyItems Done.");

            _logger.LogInformation("Seed completed.");
        }
    }
}
