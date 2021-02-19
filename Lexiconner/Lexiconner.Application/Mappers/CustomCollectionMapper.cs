using Lexiconner.Domain.Dto.CustomCollections;
using Lexiconner.Domain.Dtos.CustomCollections;
using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Application.Mappers
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
                IsSelected = entity.IsSelected,
                Children = entity.Children.Select(x => CustomMapper.MapToDto(x)).ToList(),
                DescendantsAsList = new List<CustomCollectionDto>(),
            };
        }

        public static IEnumerable<CustomCollectionDto> MapToDto(IEnumerable<CustomCollectionEntity> entities)
        {
            return entities.Select(x => MapToDto(x)).ToList();
        }

        public static CustomCollectionsAllResponseDto MapToCustomCollectionsAllResponseDto(CustomCollectionEntity entity)
        {
            var dto = CustomMapper.MapToDto(entity);
            dto.PopulateFlattenedDescendants();
            
            return new CustomCollectionsAllResponseDto()
            {
                AsTree = dto,
                AsList = dto.FlattenToList(),
            };
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
