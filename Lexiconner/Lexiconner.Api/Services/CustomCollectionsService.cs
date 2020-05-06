using Lexiconner.Api.Mappers;
using Lexiconner.Api.Services.Interfaces;
using Lexiconner.Application.Exceptions;
using Lexiconner.Domain.Config;
using Lexiconner.Domain.Dto.CustomCollections;
using Lexiconner.Domain.DTOs.CustomCollections;
using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.Services
{
    public class CustomCollectionsService : ICustomCollectionsService
    {
        private readonly IDataRepository _dataRepository;

        public CustomCollectionsService(
            IDataRepository MongoDataRepository
        )
        {
            _dataRepository = MongoDataRepository;
        }

        public async Task<CustomCollectionsAllResponseDto> GetAllCustomCollectionsAsync(string userId)
        {
            var rootEntity = await _dataRepository.GetOneAsync<CustomCollectionEntity>(x => x.UserId == userId);
            if (rootEntity == null)
            {
                // add root
                rootEntity = await this.CreateCustomCollectionRootAsync(userId);
            }
            var dto = CustomMapper.MapToDto(rootEntity);
            return new CustomCollectionsAllResponseDto()
            {
                AsTree = dto,
                AsList = dto.FlattenToList(),
            };
        }

        public async Task<CustomCollectionsAllResponseDto> CreateCustomCollectionAsync(string userId, CustomCollectionCreateDto createDto)
        {
            var rootEntity = await _dataRepository.GetOneAsync<CustomCollectionEntity>(x => x.UserId == userId);
            if (rootEntity == null)
            {
                // add root
                rootEntity = await this.CreateCustomCollectionRootAsync(userId);
            }

            var entity = CustomMapper.MapToEntity(userId, createDto);
            rootEntity.AddChildCollection(createDto.ParentCollectionId, entity);
            await _dataRepository.UpdateAsync(rootEntity);

            var dto = CustomMapper.MapToDto(rootEntity);
            return new CustomCollectionsAllResponseDto()
            {
                AsTree = dto,
                AsList = dto.FlattenToList(),
            };
        }

        public async Task<CustomCollectionsAllResponseDto> UpdateCustomCollectionAsync(string userId, string collectionId, CustomCollectionUpdateDto updateDto)
        {
            var rootEntity = await _dataRepository.GetOneAsync<CustomCollectionEntity>(x => x.UserId == userId);
            if (rootEntity == null)
            {
                throw new NotFoundException();
            }
            rootEntity.UpdateChildCollection(collectionId, updateDto);
            await _dataRepository.UpdateAsync(rootEntity);

            var dto = CustomMapper.MapToDto(rootEntity);
            return new CustomCollectionsAllResponseDto()
            {
                AsTree = dto,
                AsList = dto.FlattenToList(),
            };
        }

        public async Task<CustomCollectionsAllResponseDto> DeleteCustomCollectionAsync(string userId, string customCollectionId)
        {
            var rootEntity = await _dataRepository.GetOneAsync<CustomCollectionEntity>(x => x.UserId == userId);
            if (rootEntity == null)
            {
                throw new NotFoundException();
            }
            rootEntity.RemoveChildCollection(customCollectionId);
            await _dataRepository.UpdateAsync(rootEntity);

            var dto = CustomMapper.MapToDto(rootEntity);
            return new CustomCollectionsAllResponseDto()
            {
                AsTree = dto,
                AsList = dto.FlattenToList(),
            };
        }

        public async Task<CustomCollectionsAllResponseDto> DuplicateCustomCollectionAsync(string userId, string collectionId)
        {
            var rootEntity = await _dataRepository.GetOneAsync<CustomCollectionEntity>(x => x.UserId == userId);
            if (rootEntity == null)
            {
                throw new NotFoundException();
            }
            rootEntity.DuplicateCollection(collectionId);
            await _dataRepository.UpdateAsync(rootEntity);

            var dto = CustomMapper.MapToDto(rootEntity);
            return new CustomCollectionsAllResponseDto()
            {
                AsTree = dto,
                AsList = dto.FlattenToList(),
            };
        }

        #region Private

        private async Task<CustomCollectionEntity> CreateCustomCollectionRootAsync(string userId)
        {
            var rootEntity = new CustomCollectionEntity()
            {
                UserId = userId,
                Name = CustomCollectionConfig.RootCollectionName,
                IsRoot = true,
            };
            await _dataRepository.AddAsync(rootEntity);
            return rootEntity;
        }

        #endregion
    }
}
