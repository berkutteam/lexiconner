using Lexiconner.Domain.Dtos.Words;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Application.Services.Interfacse
{
    public interface IWordsService
    {
        Task<WordExamplesDto> GetWordExamplesAsync(string languageCode, string word);
    }
}
