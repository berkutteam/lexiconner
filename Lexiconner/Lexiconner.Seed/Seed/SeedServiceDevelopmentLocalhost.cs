using Lexiconner.Application.ApiClients;
using Lexiconner.Domain.Entitites;
using Lexiconner.IdentityServer4.Config;
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
        private readonly IIdentityServerConfig _identityServerConfig;
        private readonly IGoogleTranslateApiClient _googleTranslateApiClient;
        private readonly IContextualWebSearchApiClient _contextualWebSearchApiClient;

        public SeedServiceDevelopmentLocalhost(
            ILogger<ISeedService> logger,
            IWordTxtImporter wordTxtImporter,
            IMongoRepository mongoRepository,
            IIdentityRepository identityRepository,
            IIdentityServerConfig identityServerConfig,
            IGoogleTranslateApiClient googleTranslateApiClient,
            IContextualWebSearchApiClient contextualWebSearchApiClient
        )
        {
            _logger = logger;
            _wordTxtImporter = wordTxtImporter;
            _mongoRepository = mongoRepository;
            _identityRepository = identityRepository;
            _identityServerConfig = identityServerConfig;
            _googleTranslateApiClient = googleTranslateApiClient;
            _contextualWebSearchApiClient = contextualWebSearchApiClient;
        }

        public Task RemoveDatabaseAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SeedAsync()
        {
            _logger.LogInformation("Start seeding data...");

            _logger.LogInformation("Users...");
          
            _logger.LogInformation("Users Done.");

            // seed imported data for marked users
            _logger.LogInformation("StudyItems...");
            var usersWithImport = _identityServerConfig.GetInitialdentityUsers().Where(x => x.IsImportInitialData);
            // var usersWithImport = await _identityRepository.GetManyAsync<ApplicationUserEntity>(x => x.IsImportInitialData);
            IEnumerable<StudyItemEntity> studyItems = null;
            foreach (var user in usersWithImport)
            {
                // TODO DROP DEBUG DATA
                if (true || !_mongoRepository.AnyAsync<StudyItemEntity>(x => x.UserId == user.Id).GetAwaiter().GetResult())
                {
                    studyItems = studyItems ?? GetStudyItems().GetAwaiter().GetResult();
                    studyItems = studyItems.Select(x =>
                    {
                        x.UserId = user.Id;
                        return x;
                    });
                    _mongoRepository.AddAsync(studyItems).GetAwaiter().GetResult();
                    _logger.LogInformation($"StudyItems was added for user #{user.Email}.");
                }
            }
            _logger.LogInformation("StudyItems Done.");

            _logger.LogInformation("Seed completed.");
        }

        private async Task<IEnumerable<StudyItemEntity>> GetStudyItems()
        {
            _logger.LogInformation("Importing StudyItems...");
            var wordImports = await _wordTxtImporter.Import();
            var entities = wordImports.Select(x => new StudyItemEntity
            {
                UserId = null,
                Title = x.Word,
                Description = x.Description,
                ExampleText = x.ExampleText,
                Tags = x.Tags,
            });

            // TODO
            return entities;

            _logger.LogInformation("Making translations and adding images to StudyItems...");

            // get translation ru -> en
            // https://cloud.google.com/translate/docs/languages
            string sourceLanguageCode = _wordTxtImporter.SourceLanguageCode;
            string targetLanguageCode = "en";

            foreach (var entity in entities)
            {
                // TODO handle erorrs, request limits

                // translate
                var translateResult = await _googleTranslateApiClient.Translate(new List<string>() { entity.Title }, sourceLanguageCode, targetLanguageCode);

                // make contextual search
                if(translateResult.Translations.Any())
                {
                    var translation = translateResult.Translations.First().TranslatedText;
                    var imageSearchResult = await _contextualWebSearchApiClient.ImageSearchAsync(query: translation, pageSize: 2);

                    if(imageSearchResult.Value.Any())
                    {
                        var image = imageSearchResult.Value.First();
                        entity.Image = new StudyItemImageEntity
                        {
                            Url = image.Url,
                            Height = image.Height,
                            Width = image.Width,
                            Thumbnail = image.Thumbnail,
                            ThumbnailHeight = image.ThumbnailHeight,
                            ThumbnailWidth = image.ThumbnailWidth,
                            Base64Encoding = image.Base64Encoding,
                        };
                    }
                }
            }


            return entities;
        }
    }
}
