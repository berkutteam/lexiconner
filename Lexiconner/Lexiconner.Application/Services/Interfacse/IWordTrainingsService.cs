using Lexiconner.Api.Dtos.WordsTrainings;
using Lexiconner.Api.DTOs.WordsTrainings;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Dtos.WordTrainings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Application.Services.Interfacse
{
    public interface IWordTrainingsService
    {
        Task<TrainingsStatisticsDto> GetTrainingStatisticsAsync(string userId);
        Task MarkWordAsTrainedAsync(string userId, string wordId);
        Task MarkWordAsNotTrainedAsync(string userId, string wordId);
        Task<FlashCardsTrainingDto> GetTrainingItemsForFlashCardsAsync(string userId, string collectionId, int limit);
        Task SaveTrainingResultsForFlashCardsAsync(string userId, FlashCardsTrainingResultDto results);
        Task<WordMeaningTrainingDto> GetTrainingItemsForWordMeaningAsync(string userId, string collectionId, int limit);
        Task SaveTrainingResultsForWordMeaningAsync(string userId, WordMeaningTrainingResultDto results);
        Task<MeaningWordTrainingDto> GetTrainingItemsForMeaningWordAsync(string userId, string collectionId, int limit);
        Task SaveTrainingResultsForMeaningWordAsync(string userId, MeaningWordTrainingResultDto results);
        Task<MatchWordsTrainingDto> GetTrainingItemsForMatchWordsAsync(string userId, string collectionId);
        Task SaveTrainingResultsForMatchWordsAsync(string userId, MatchWordsTrainingResultDto results);
        Task<BuildWordsTrainingDto> GetTrainingItemsForBuildWordsAsync(string userId, string collectionId, int limit);
        Task SaveTrainingResultsForBuildWordsAsync(string userId, BuildWordsTrainingResultDto results);
        Task<ListenWordsTrainingDto> GetTrainingItemsForListenWordsAsync(string userId, string collectionId, int limit);
        Task SaveTrainingResultsForListenWordsAsync(string userId, ListenWordsTrainingResultDto results);
    }
}
