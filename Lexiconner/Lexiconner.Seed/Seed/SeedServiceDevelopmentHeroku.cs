using IdentityServer4.Models;
using Lexiconner.Application.ApiClients;
using Lexiconner.Application.ApiClients.Dtos;
using Lexiconner.Application.Exceptions;
using Lexiconner.Application.ImportAndExport;
using Lexiconner.Application.Services;
using Lexiconner.Domain.Config;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Entitites.Cache;
using Lexiconner.Domain.Entitites.IdentityModel;
using Lexiconner.IdentityServer4.Config;
using Lexiconner.Persistence.Cache;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence.Repositories.MongoDb;
using Lexiconner.Seed.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lexiconner.Application.ApiClients.Dtos.GoogleTranslateResponseDto;
using static Lexiconner.Application.ApiClients.Dtos.ImageSearchResponseDto;

namespace Lexiconner.Seed.Seed
{
    public class SeedServiceDevelopmentHeroku : ISeedService
    {
        private readonly IConfigurationRoot _configuration;
        private readonly ILogger<ISeedService> _logger;
        private readonly IWordTxtImporter _wordTxtImporter;
        private readonly IDataRepository _dataRepository;
        private readonly IIdentityDataRepository _identityRepository;
        private readonly IIdentityServerConfig _identityServerConfig;
        private readonly IImageService _imageService;
        private readonly SeedServiceDevelopmentLocalhost _seedServiceDevelopmentLocalhost;

        private readonly UserManager<ApplicationUserEntity> _userManager;
        private readonly RoleManager<ApplicationRoleEntity> _roleManager;

        public SeedServiceDevelopmentHeroku(
            IConfigurationRoot configuration,
            ILogger<ISeedService> logger,
            IWordTxtImporter wordTxtImporter,
            IDataRepository dataRepository,
            IIdentityDataRepository identityRepository,
            IIdentityServerConfig identityServerConfig,
            IImageService imageService,
            UserManager<ApplicationUserEntity> userManager,
            RoleManager<ApplicationRoleEntity> roleManager,
            SeedServiceDevelopmentLocalhost seedServiceDevelopmentLocalhost
        )
        {
            _configuration = configuration;
            _logger = logger;
            _wordTxtImporter = wordTxtImporter;
            _dataRepository = dataRepository;
            _identityRepository = identityRepository;
            _identityServerConfig = identityServerConfig;
            _imageService = imageService;
            _userManager = userManager;
            _roleManager = roleManager;
            _seedServiceDevelopmentLocalhost = seedServiceDevelopmentLocalhost;
        }

        public Task RemoveDatabaseAsync()
        {
            return _seedServiceDevelopmentLocalhost.RemoveDatabaseAsync();
        }

        public Task SeedAsync()
        {
            return _seedServiceDevelopmentLocalhost.SeedAsync();
        }
    }
}
