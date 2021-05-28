using AutoMapper;
using Lexiconner.Api.Dtos.WordsTrainings;
using Lexiconner.Api.DTOs.WordsTrainings;
using Lexiconner.Application.ApiClients;
using Lexiconner.Application.ApiClients.Scrapers;
using Lexiconner.Application.Exceptions;
using Lexiconner.Application.Extensions;
using Lexiconner.Application.Helpers;
using Lexiconner.Application.Mappers;
using Lexiconner.Application.Services.Interfacse;
using Lexiconner.Application.Validation;
using Lexiconner.Domain.Config;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.Users;
using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Enums;
using Lexiconner.Persistence.Repositories;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lexiconner.Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly IMapper _mapper;
        private readonly IIdentityDataRepository _identityDataRepository;
        private readonly IDataRepository _dataRepository;

        public UsersService(
            IMapper mapper,
            IIdentityDataRepository identityDataRepository,
            IDataRepository dataRepository
        )
        {
            _mapper = mapper;
            _dataRepository = dataRepository;
            _identityDataRepository = identityDataRepository;
        }

        public async Task<UserDto> GetUserAsync(string userId)
        {
            var entity = await _identityDataRepository.GetOneAsync<ApplicationUserEntity>(x => x.Id == userId);
            if (entity == null)
            {
                throw new AccessDeniedException("User not found!");
            }

            return _mapper.Map<UserDto>(entity);
        }

        public async Task<UserDto> SelectLearningLanguageAsync(string userId, string languageCode)
        {
            var entity = await _identityDataRepository.GetOneAsync<ApplicationUserEntity>(x => x.Id == userId);
            if (entity == null)
            {
                throw new AccessDeniedException("User not found!");
            }

            // update
            entity.AddOrUpdateLearningLanguage(languageCode, isSelected: true, isSelectedForBrowserExtension: null);
            CustomValidationHelper.Validate(entity);
            await _identityDataRepository.UpdateAsync(entity);

            return _mapper.Map<UserDto>(entity);
        }

        public async Task<UserDto> BrowserExtensionSelectLearningLanguageAsync(string userId, string languageCode)
        {
            var entity = await _identityDataRepository.GetOneAsync<ApplicationUserEntity>(x => x.Id == userId);
            if (entity == null)
            {
                throw new AccessDeniedException("User not found!");
            }

            // update
            entity.AddOrUpdateLearningLanguage(languageCode, isSelected: null, isSelectedForBrowserExtension: true);
            CustomValidationHelper.Validate(entity);
            await _identityDataRepository.UpdateAsync(entity);

            return _mapper.Map<UserDto>(entity);
        }
    }
}
