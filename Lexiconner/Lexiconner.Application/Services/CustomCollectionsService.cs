﻿using AutoMapper;
using Lexiconner.Application.Exceptions;
using Lexiconner.Application.Mappers;
using Lexiconner.Application.Services.Interfacse;
using Lexiconner.Domain.Config;
using Lexiconner.Domain.Dto.CustomCollections;
using Lexiconner.Domain.Dtos.CustomCollections;
using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Application.Services
{
    public class CustomCollectionsService : ICustomCollectionsService
    {
        private readonly IMapper _mapper;
        private readonly IDataRepository _dataRepository;

        public CustomCollectionsService(
            IMapper mapper,
            IDataRepository MongoDataRepository
        )
        {
            _mapper = mapper;
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

            var dto = CustomMapper.MapToCustomCollectionsAllResponseDto(rootEntity);
            return dto;

            // TODO: calc items count in each collection using System.Threading.Tasks.Dataflow
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

            var dto = CustomMapper.MapToCustomCollectionsAllResponseDto(rootEntity);
            return dto;
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

            var dto = CustomMapper.MapToCustomCollectionsAllResponseDto(rootEntity);
            return dto;
        }

        public async Task<CustomCollectionsAllResponseDto> MarkCustomCollectionAsSelectedAsync(string userId, string collectionId)
        {
            var rootEntity = await _dataRepository.GetOneAsync<CustomCollectionEntity>(x => x.UserId == userId);
            if (rootEntity == null)
            {
                throw new NotFoundException();
            }
            rootEntity.MarkCollectionAsSelected(collectionId);
            await _dataRepository.UpdateAsync(rootEntity);

            var dto = CustomMapper.MapToCustomCollectionsAllResponseDto(rootEntity);
            return dto;
        }

        public async Task<CustomCollectionsAllResponseDto> DeleteCustomCollectionAsync(string userId, string customCollectionId, bool isDeleteItems)
        {
            var rootEntity = await _dataRepository.GetOneAsync<CustomCollectionEntity>(x => x.UserId == userId);
            if (rootEntity == null)
            {
                throw new NotFoundException();
            }
            rootEntity.RemoveChildCollection(customCollectionId);
            await _dataRepository.UpdateAsync(rootEntity);

            // handle items
            if(isDeleteItems)
            {
                // delete items
                await _dataRepository.DeleteAsync<WordEntity>(x => x.CustomCollectionIds.Contains(customCollectionId));
            }
            else
            {
                // delete deleted collection from items
                var itemEntities = await _dataRepository.GetManyAsync<WordEntity>(x => x.CustomCollectionIds.Contains(customCollectionId));
                itemEntities.ToList().ForEach(x => x.RemoveCollection(customCollectionId));
                await _dataRepository.UpdateManyAsync(itemEntities);
            }

            var dto = CustomMapper.MapToCustomCollectionsAllResponseDto(rootEntity);
            return dto;
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

            var dto = CustomMapper.MapToCustomCollectionsAllResponseDto(rootEntity);
            return dto;
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
