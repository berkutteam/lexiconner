using Lexiconner.Api.DTOs;
using Lexiconner.Api.DTOs.StudyItemsTrainings;
using Lexiconner.Api.Mappers;
using Lexiconner.Api.Models;
using Lexiconner.Api.Services.Interfaces;
using Lexiconner.Application.Exceptions;
using Lexiconner.Application.Helpers;
using Lexiconner.Application.Services;
using Lexiconner.Application.Validation;
using Lexiconner.Domain.Attributes;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.StudyItems;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Enums;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence.Repositories.MongoDb;
using LinqKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Lexiconner.Api.Services
{
    public class StudyItemsService : IStudyItemsService
    {
        private readonly IDataRepository _dataRepository;
        private readonly IImageService _imageService;

        public StudyItemsService(
            IDataRepository MongoDataRepository,
            IImageService imageService
        )
        {
            _dataRepository = MongoDataRepository;
            _imageService = imageService;
        }

        #region Study items

        public async Task<PaginationResponseDto<StudyItemDto>> GetAllStudyItemsAsync(
            string userId, 
            int offset, 
            int limit, 
            StudyItemsSearchFilterModel searchFilter = null, 
            string collectionId = null
        )
        {
            var predicate = PredicateBuilder.New<StudyItemEntity>(x => x.UserId == userId);

            if(searchFilter != null)
            {
                if (!String.IsNullOrEmpty(searchFilter.Search))
                {
                    string search = searchFilter.Search.Trim().ToLower();
                    predicate.And(x => x.Title.ToLower().Contains(search) || x.Description.ToLower().Contains(search));
                }
                if (searchFilter.IsFavourite.GetValueOrDefault(false))
                {
                    predicate.And(x => x.IsFavourite);
                }
            }

            if(collectionId != null)
            {
                predicate.And(x => x.CustomCollectionIds.Contains(collectionId));
            }

            var itemsTask = _dataRepository.GetManyAsync<StudyItemEntity>(predicate, offset, limit);
            var totalTask = _dataRepository.CountAllAsync<StudyItemEntity>(predicate);

            var total = await totalTask;
            var items = await itemsTask;

            var result = new PaginationResponseDto<StudyItemDto>
            {
              
                Items = CustomMapper.MapToDto(items),
                Pagination = new PaginationInfoDto()
                {
                    TotalCount = total,
                    ReturnedCount = items.Count(),
                    Offset = offset,
                    Limit = limit,
                }
            };

            return result;
        }

        public async Task<StudyItemDto> GetStudyItemAsync(string userId, string studyItemId)
        {
            var entity = await _dataRepository.GetOneAsync<StudyItemEntity>(x => x.Id == studyItemId && x.UserId == userId);
            return CustomMapper.MapToDto(entity);
        }

        public async Task<StudyItemDto> CreateStudyItemAsync(string userId, StudyItemCreateDto createDto)
        {
            var entity = CustomMapper.MapToEntity(userId, createDto);
            CustomValidationHelper.Validate(entity);

            // set image
            if (entity.Title.Length > 3)
            {
                var imagesResult = await _imageService.FindImagesAsync(sourceLanguageCode: entity.LanguageCode, entity.Title);

                if (imagesResult.Any())
                {
                    // try to find suitable image
                    var image = _imageService.GetSuitableImages(imagesResult);
                    if (image != null)
                    {
                        entity.Image = new StudyItemImageEntity
                        {
                            Url = image.Url,
                            Height = image.Height,
                            Width = image.Width,
                            Thumbnail = image.Thumbnail,
                            ThumbnailHeight = image.ThumbnailHeight,
                            ThumbnailWidth = image.ThumbnailWidth,
                            Base64Encoding = image.Base64Encoding,
                        };
                    }
                }
            }


            await _dataRepository.AddAsync(entity);
            return CustomMapper.MapToDto(entity);
        }

        public async Task<StudyItemDto> UpdateStudyItemAsync(string userId, string studyItemId, StudyItemUpdateDto updateDto)
        {
            var entity = await _dataRepository.GetOneAsync<StudyItemEntity>(x => x.Id == studyItemId);
            if (entity.UserId != userId)
            {
                throw new AccessDeniedException("Can't edit the item that you don't own!");
            }

            // update
            entity.UpdateSelf(updateDto);

            // set image
            if (entity.Image == null)
            {
                if (entity.Title.Length > 3)
                {
                    var imagesResult = await _imageService.FindImagesAsync(sourceLanguageCode: entity.LanguageCode, entity.Title);

                    if (imagesResult.Any())
                    {
                        // try to find suitable image
                        var image = _imageService.GetSuitableImages(imagesResult);
                        if (image != null)
                        {
                            entity.Image = new StudyItemImageEntity
                            {
                                Url = image.Url,
                                Height = image.Height,
                                Width = image.Width,
                                Thumbnail = image.Thumbnail,
                                ThumbnailHeight = image.ThumbnailHeight,
                                ThumbnailWidth = image.ThumbnailWidth,
                                Base64Encoding = image.Base64Encoding,
                            };
                        }
                    }
                }
            }

            await _dataRepository.UpdateAsync(entity);
            return CustomMapper.MapToDto(entity);
        }

        public async Task DeleteStudyItem(string userId, string sutyItemId)
        {
            var existing = await _dataRepository.GetOneAsync<StudyItemEntity>(x => x.Id == sutyItemId && x.UserId == userId);
            if (existing == null)
            {
                throw new NotFoundException();
            }
            await _dataRepository.DeleteAsync<StudyItemEntity>(x => x.Id == existing.Id);
        }

        #endregion


        #region Trainings

        public async Task<TrainingsStatisticsDto> GetTrainingStatisticsAsync(string userId)
        {
            long totalItemCount = await _dataRepository.CountAllAsync<StudyItemEntity>(x => x.UserId == userId);
            long onTrainingItemCount = await _dataRepository.CountAllAsync<StudyItemEntity>(x => 
                x.UserId == userId && 
                x.TrainingInfo != null && 
                !x.TrainingInfo.IsTrained
            );
            long trainedItemCount = await _dataRepository.CountAllAsync<StudyItemEntity>(x => 
                x.UserId == userId && 
                x.TrainingInfo != null &&
                x.TrainingInfo.IsTrained
            );

            Func<string, TrainingType, Task<TrainingsStatisticsDto.TrainingStatisticsItemDto>> getTrainingStatisticsItem = async (_userId, trainingType) =>
            {
                return new TrainingsStatisticsDto.TrainingStatisticsItemDto
                {
                    TrainingType = trainingType,
                    TrainingTypeFormatted = EnumHelper<TrainingType>.GetDisplayValue(trainingType),
                    OnTrainingItemCount = await _dataRepository.CountAllAsync<StudyItemEntity>(
                            x => x.UserId == _userId && x.TrainingInfo != null && x.TrainingInfo.Trainings.Any(y => y.TrainingType == trainingType && y.Progress != 1)
                        ),
                    TrainedItemCount = await _dataRepository.CountAllAsync<StudyItemEntity>(
                            x => x.UserId == _userId && x.TrainingInfo != null && x.TrainingInfo.Trainings.Any(y => y.TrainingType == trainingType && y.Progress == 1)
                        ),
                };
            };

            var result = new TrainingsStatisticsDto()
            {
                TotalItemCount = totalItemCount,
                OnTrainingItemCount = onTrainingItemCount,
                TrainedItemCount = trainedItemCount,

                TrainingStats = new List<TrainingsStatisticsDto.TrainingStatisticsItemDto>()
                {
                    await getTrainingStatisticsItem(userId, TrainingType.FlashCards),
                    await getTrainingStatisticsItem(userId, TrainingType.WordMeaning),
                    await getTrainingStatisticsItem(userId, TrainingType.MeaningWord),
                }
            };

            return result;
        }

        public async Task MarkStudyItemAsTrainedAsync(string userId, string studyItemId)
        {
            var entity = await _dataRepository.GetOneAsync<StudyItemEntity>(x => x.UserId == userId && x.Id == studyItemId);
            entity.MarkAsTrained();
            entity.RecalculateTotalTrainingProgress();
            await _dataRepository.UpdateAsync(entity);
        }

        public async Task MarkStudyItemAsNotTrainedAsync(string userId, string studyItemId)
        {
            var entity = await _dataRepository.GetOneAsync<StudyItemEntity>(x => x.UserId == userId && x.Id == studyItemId);
            entity.MarkAsNotTrained();
            entity.ResetTotalTrainingProgress();
            await _dataRepository.UpdateAsync(entity);
        }

        public async Task<FlashCardsTrainingDto> GetTrainingItemsForFlashCardsAsync(string userId, string collectionId, int limit)
        {
            var predicate = PredicateBuilder.New<StudyItemEntity>(x =>
                x.UserId == userId &&
                (
                    x.TrainingInfo == null ||
                    !x.TrainingInfo.Trainings.Any() ||
                    (
                        x.TrainingInfo != null && 
                        !x.TrainingInfo.IsTrained &&
                        x.TrainingInfo.Trainings.Any(y => y.TrainingType == TrainingType.FlashCards && y.Progress < 1 && y.NextTrainingdAt <= DateTime.UtcNow)
                    )
                )
            );

            if (collectionId != null)
            {
                predicate.And(x => x.CustomCollectionIds.Contains(collectionId));
            }

            var entities = await _dataRepository.GetManyAsync<StudyItemEntity>(predicate, 0, limit);

            return new FlashCardsTrainingDto
            {
                Items = entities.ToList(),
            };
        }

        public async Task SaveTrainingResultsForFlashCardsAsync(string userId, FlashCardsTrainingResultDto results)
        {
            var ids = results.ItemsResults.Select(x => x.ItemId);
            var entities = (await _dataRepository.GetManyAsync<StudyItemEntity>(
                x => x.UserId == userId && ids.Contains(x.Id)
            )).ToList();

            var infoAttribute = TrainingTypeHelper.GetAttribute(TrainingType.FlashCards);

            entities = entities.Select(x =>
            {
                bool isCorrect = results.ItemsResults.Any(y => y.ItemId == x.Id && y.IsCorrect);

                x.TrainingInfo = x.TrainingInfo ?? new StudyItemTrainingInfoEntity();
                x.TrainingInfo.Trainings = x.TrainingInfo.Trainings ?? new List<StudyItemTrainingInfoEntity.StudyItemTrainingProgressItemEntity>();

                var training = x.TrainingInfo.Trainings.FirstOrDefault(y => y.TrainingType == TrainingType.FlashCards);
                if(training == null)
                {
                    training = new StudyItemTrainingInfoEntity.StudyItemTrainingProgressItemEntity() {
                        TrainingType = TrainingType.FlashCards,
                    };
                    x.TrainingInfo.Trainings.Add(training);
                }

                if (isCorrect)
                {
                    training.Progress = training.Progress + infoAttribute.CorrectAnswerProgressRate;
                    training.Progress = Math.Min(training.Progress, 1);
                }
                else
                {
                    training.Progress = training.Progress + infoAttribute.WrongAnswerProgressRate;
                    training.Progress = Math.Max(training.Progress, 0);
                }
                training.Progress = Math.Round(training.Progress, 2);

                training.LastTrainingdAt = DateTimeOffset.UtcNow;

                if(training.Progress < 1)
                {
                    training.NextTrainingdAt = DateTimeOffset.UtcNow.Add(infoAttribute.TrainIntervalTimespan);
                } 
                else
                {
                    training.NextTrainingdAt = DateTimeOffset.UtcNow.Add(infoAttribute.TrainIntervalForRepeatTimespan);
                }

                x.RecalculateTotalTrainingProgress();

                return x;
            }).ToList();

            await _dataRepository.UpdateManyAsync(entities);
        }

        #endregion


        #region Favourites

        public async Task AddToFavouritesAsync(string userId, IEnumerable<string> itemIds)
        {
            var entities = (await _dataRepository.GetManyAsync<StudyItemEntity>(x => x.UserId == userId && itemIds.Contains(x.Id))).ToList();

            entities = entities.Select(x =>
            {
                x.IsFavourite = true;
                return x;
            }).ToList();

            await _dataRepository.UpdateManyAsync<StudyItemEntity>(entities);
        }

        public async Task DeleteFromFavouritesAsync(string userId, IEnumerable<string> itemIds)
        {
            var entities = (await _dataRepository.GetManyAsync<StudyItemEntity>(x => x.UserId == userId && itemIds.Contains(x.Id))).ToList();

            entities = entities.Select(x =>
            {
                x.IsFavourite = false;
                return x;
            }).ToList();

            await _dataRepository.UpdateManyAsync<StudyItemEntity>(entities);
        }

        #endregion
    }
}
