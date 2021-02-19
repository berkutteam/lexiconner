using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Application.Mappers
{
    public static partial class CustomMapper
    {
        public static WordDto MapToDto(WordEntity entity)
        {
            return new WordDto
            {
                Id = entity.Id,
                CustomCollectionIds = entity.CustomCollectionIds,
                Word = entity.Word,
                Meaning = entity.Meaning,
                Examples = entity.Examples,
                IsFavourite = entity.IsFavourite,
                WordLanguageCode = entity.WordLanguageCode,
                MeaningLanguageCode = entity.MeaningLanguageCode,
                Tags = entity.Tags,
                Images = MapToDto(entity.Images).ToList(),
                TrainingInfo = MapToDto(entity.TrainingInfo),
            };
        }

        public static IEnumerable<WordDto> MapToDto(IEnumerable<WordEntity> entities)
        {
            return entities.Select(x => MapToDto(x)).ToList();
        }

        public static WordImageDto MapToDto(WordImageEntity entity)
        {
            if(entity == null)
            {
                return null;
            }

            return new WordImageDto
            {
                Url = entity.Url,
                Height = entity.Height,
                Width = entity.Width,
                Thumbnail = entity.Thumbnail,
                ThumbnailHeight = entity.ThumbnailHeight,
                ThumbnailWidth = entity.ThumbnailWidth,
                Base64Encoding = entity.Base64Encoding,
            };
        }

        public static IEnumerable<WordImageDto> MapToDto(IEnumerable<WordImageEntity> entities)
        {
            return entities.Select(x => MapToDto(x)).ToList();
        }

        public static WordTrainingInfoDto MapToDto(WordTrainingInfoEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new WordTrainingInfoDto
            {
                TotalProgress = entity.TotalProgress,
                IsTrained = entity.IsTrained,
            };
        }

        public static WordEntity MapToEntity(string userId, WordCreateDto dto)
        {
            return new WordEntity
            {
                UserId = userId,
                CustomCollectionIds = dto.CustomCollectionIds,
                Word = dto.Word,
                Meaning = dto.Meaning,
                Examples = dto.Examples,
                IsFavourite = dto.IsFavourite,
                WordLanguageCode = dto.WordLanguageCode,
                MeaningLanguageCode = dto.MeaningLanguageCode,
                Tags = dto.Tags,
            };
        }
    }
}
