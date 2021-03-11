using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.UserFilms;
using System.Threading.Tasks;

namespace Lexiconner.Application.Services.Interfacse
{
    public interface IFilmsService
    {
        Task<PaginationResponseDto<UserFilmDto>> GetAllUserFilmsAsync(
            string userId,
            int offset,
            int limit,
            string search = null
        );
        Task<UserFilmDto> GetUserFilmAsync(string userId, string userFilmId);
        Task<UserFilmDto> CreateUserFilmAsync(string userId, UserFilmCreateDto createDto);
        Task<UserFilmDto> UpdateUserFilmAsync(string userId, string wordId, UserFilmUpdateDto updateDto);
        Task DeleteUserFilm(string userId, string userFilmId);
    }
}
