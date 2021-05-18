using Lexiconner.Api.Dtos.WordsTrainings;
using Lexiconner.Api.DTOs.WordsTrainings;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.General;
using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Dtos.WordSets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Application.Services.Interfacse
{
    public interface IWordSetsService
    {
        public Task<PaginationResponseDto<WordSetDto>> GetAllWordSetsAsync(
           string languageCode,
           int offset,
           int limit,
           string search = null
        );
        Task<WordSetDto> CreateWordSetAsync(string userId, WordSetCreateDto dto);
    }
}
