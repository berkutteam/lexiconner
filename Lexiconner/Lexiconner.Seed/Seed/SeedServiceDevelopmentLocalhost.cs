using Lexiconner.Application.ApiClients;
using Lexiconner.Application.ApiClients.Dtos;
using Lexiconner.Application.Exceptions;
using Lexiconner.Application.ImportAndExport;
using Lexiconner.Application.Services;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Entitites.Cache;
using Lexiconner.IdentityServer4.Config;
using Lexiconner.Persistence.Cache;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence.Repositories.MongoDb;
using Lexiconner.Seed.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lexiconner.Application.ApiClients.Dtos.GoogleTranslateResponseDto;
using static Lexiconner.Application.ApiClients.Dtos.ImageSearchResponseDto;

namespace Lexiconner.Seed.Seed
{
    public class SeedServiceDevelopmentLocalhost : ISeedService
    {
        private readonly ILogger<ISeedService> _logger;
        private readonly IWordTxtImporter _wordTxtImporter;
        private readonly IDataRepository _dataRepository;
        private readonly IIdentityDataRepository _identityRepository;
        private readonly IIdentityServerConfig _identityServerConfig;
        private readonly IImageService _imageService;

        public SeedServiceDevelopmentLocalhost(
            ILogger<ISeedService> logger,
            IWordTxtImporter wordTxtImporter,
            IDataRepository dataRepository,
            IIdentityDataRepository identityRepository,
            IIdentityServerConfig identityServerConfig,
            IImageService imageService
        )
        {
            _logger = logger;
            _wordTxtImporter = wordTxtImporter;
            _dataRepository = dataRepository;
            _identityRepository = identityRepository;
            _identityServerConfig = identityServerConfig;
            _imageService = imageService;
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
            IEnumerable<StudyItemEntity> studyItems = null;
            foreach (var user in usersWithImport)
            {
                if (!_dataRepository.ExistsAsync<StudyItemEntity>(x => x.UserId == user.Id).GetAwaiter().GetResult())
                {
                    studyItems = studyItems ?? GetStudyItems().GetAwaiter().GetResult();
                    studyItems = studyItems.Select(x =>
                    {
                        // fix same ids for different users
                        x.RegenerateId();
                        x.Image?.RegenerateId();

                        x.UserId = user.Id;
                        return x;
                    });

                    const int chunkSize = 50;
                    int chunkCount = (int)(Math.Ceiling((double)studyItems.Count() / (double)chunkSize));

                    for (int chunkNumber = 0; chunkNumber < chunkCount; chunkNumber++)
                    {
                        var items = studyItems.Skip(chunkNumber * chunkSize).Take(chunkSize).ToList();
                        _dataRepository.AddManyAsync(items).GetAwaiter().GetResult();
                        _logger.LogInformation($"StudyItems processed chunnk {chunkNumber + 1}/{chunkCount}.");
                    }
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
            }).ToList();

            _logger.LogInformation("Making translations and adding images to StudyItems...");

            // get translation ru -> en
            // https://cloud.google.com/translate/docs/languages
            string sourceLanguageCode = _wordTxtImporter.SourceLanguageCode;
            string targetLanguageCode = "en";

            const int preferredImageWidth = 600;
            const int maxImageWidth = 800;

            foreach (StudyItemEntity entity in entities)
            {
                try
                {
                    sourceLanguageCode = "";
                    var imagesResult = await _imageService.FindImagesAsync(sourceLanguageCode, entity.Title);

                    if (imagesResult.Any())
                    {
                        // try to find suitable image
                        ImageSearchResponseDto.ImageSearchResponseItemDto image = null;
                        image = imagesResult.FirstOrDefault(x => int.Parse(x.Width) <= preferredImageWidth);
                        image = image ?? imagesResult.FirstOrDefault(x => int.Parse(x.Width) <= maxImageWidth);
                        // image = image ?? imagesResult.First(); // do not take big images

                        if(image != null)
                        {
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
                catch (ApiRateLimitExceededException ex)
                {
                    // break
                    return entities;
                }
                catch (ApiErrorException ex)
                {
                    // break
                    return entities;
                }
                catch (Exception ex)
                {
                    // rethrow
                    throw;
                }
            }

            return entities;
        }
    }
}
