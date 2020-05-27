using Lexiconner.Domain.Dtos.UserFilms;
using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.Mappers
{
    public static partial class CustomMapper
    {
        public static UserFilmDto MapToDto(UserFilmEntity entity)
        {
            return new UserFilmDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Title = entity.Title,
                MyRating = entity.MyRating,
                Comment = entity.Comment,
                WatchedAt = entity.WatchedAt,
                ReleaseYear = entity.ReleaseYear,
                Genres = entity.Genres,
                LanguageCode = entity.LanguageCode,
                Details = entity.Details,
            };
        }

        public static IEnumerable<UserFilmDto> MapToDto(IEnumerable<UserFilmEntity> entities)
        {
            return entities.Select(x => MapToDto(x)).ToList();
        }

        public static UserFilmEntity MapToEntity(string userId, UserFilmCreateDto dto)
        {
            return new UserFilmEntity
            {
                UserId = userId,
                Title = dto.Title,
                MyRating = dto.MyRating,
                Comment = dto.Comment,
                WatchedAt = dto.WatchedAt,
                ReleaseYear = dto.ReleaseYear,
                Genres = dto.Genres,
                LanguageCode = dto.LanguageCode,
            };
        }
    }
}
