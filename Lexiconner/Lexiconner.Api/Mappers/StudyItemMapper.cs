﻿using Lexiconner.Api.DTOs.StudyItems;
using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.Mappers
{
    public static partial class CustomMapper
    {
        public static StudyItemDto MapToDto(StudyItemEntity entity)
        {
            return new StudyItemDto
            {
                Id = entity.Id,
                CustomCollectionIds = entity.CustomCollectionIds,
                Title = entity.Title,
                Description = entity.Description,
                ExampleTexts = entity.ExampleTexts,
                IsFavourite = entity.IsFavourite,
                LanguageCode = entity.LanguageCode,
                Tags = entity.Tags,
                Image = entity.Image,
            };
        }

        public static IEnumerable<StudyItemDto> MapToDto(IEnumerable<StudyItemEntity> entities)
        {
            return entities.Select(x => MapToDto(x)).ToList();
        }

        public static StudyItemEntity MapToEntity(string userId, StudyItemCreateDto dto)
        {
            return new StudyItemEntity
            {
                UserId = userId,
                Title = dto.Title,
                Description = dto.Description,
                ExampleTexts = dto.ExampleTexts,
                IsFavourite = dto.IsFavourite,
                LanguageCode = dto.LanguageCode,
                Tags = dto.Tags,
            };
        }
    }
}
