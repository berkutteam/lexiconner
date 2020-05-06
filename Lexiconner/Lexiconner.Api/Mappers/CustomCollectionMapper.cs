using Lexiconner.Domain.DTOs.CustomCollections;
using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.Mappers
{
    public static partial class CustomMapper
    {
        public static CustomCollectionDto MapToDto(CustomCollectionEntity entity)
        {
            return new CustomCollectionDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Name = entity.Name,
                IsRoot = entity.IsRoot,
                Children = entity.ChildrenCollections.Select(x => CustomMapper.MapToDto(x)).ToList(),
            };
        }

        public static IEnumerable<CustomCollectionDto> MapToDto(IEnumerable<CustomCollectionEntity> entities)
        {
            return entities.Select(x => MapToDto(x)).ToList();
        }

        public static CustomCollectionEntity MapToEntity(string userId, CustomCollectionCreateDto dto)
        {
            return new CustomCollectionEntity
            {
                UserId = userId,
                Name = dto.Name,
            };
        }
    }
}
