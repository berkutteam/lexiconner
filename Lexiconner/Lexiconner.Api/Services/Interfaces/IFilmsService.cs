using Lexiconner.Api.Models;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.UserFilms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.Services.Interfaces
{
    public interface IFilmsService
    {
        Task<PaginationResponseDto<UserFilmDto>> GetAllUserFilmsAsync(
            string userId,
            int offset,
            int limit,
            UserFilmsSearchFilterModel searchFilter = null
        );
        Task<UserFilmDto> GetUserFilmAsync(string userId, string userFilmId);
        Task<UserFilmDto> CreateUserFilmAsync(string userId, UserFilmCreateDto createDto);
        Task<UserFilmDto> UpdateUserFilmAsync(string userId, string studyItemId, UserFilmUpdateDto updateDto);
        Task DeleteUserFilm(string userId, string userFilmId);
    }
}
