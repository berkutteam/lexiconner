using Lexiconner.Application.Exceptions;
using Lexiconner.Application.Mappers;
using Lexiconner.Application.Services.Interfacse;
using Lexiconner.Application.Validation;
using Lexiconner.Domain.Dtos;
using Lexiconner.Domain.Dtos.UserFilms;
using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.Repositories;
using LinqKit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Application.Services
{
    public class FilmsService : IFilmsService
    {
        private readonly IDataRepository _dataRepository;
        private readonly IImageService _imageService;

        public FilmsService(
            IDataRepository MongoDataRepository,
            IImageService imageService
        )
        {
            _dataRepository = MongoDataRepository;
            _imageService = imageService;
        }

        public async Task<PaginationResponseDto<UserFilmDto>> GetAllUserFilmsAsync(
            string userId,
            int offset,
            int limit,
            string search = null
        )
        {
            var predicate = PredicateBuilder.New<UserFilmEntity>(x => x.UserId == userId);

            if (!String.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                predicate.And(x => x.Title.ToLower().Contains(search));
            }

            var itemsTask = _dataRepository.GetManyAsync<UserFilmEntity>(predicate, offset, limit);
            var totalTask = _dataRepository.CountAllAsync<UserFilmEntity>(predicate);

            var total = await totalTask;
            var items = await itemsTask;

            var result = new PaginationResponseDto<UserFilmDto>
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

        public async Task<UserFilmDto> GetUserFilmAsync(string userId, string userFilmId)
        {
            var entity = await _dataRepository.GetOneAsync<UserFilmEntity>(x => x.Id == userFilmId && x.UserId == userId);
            return CustomMapper.MapToDto(entity);
        }

        public async Task<UserFilmDto> CreateUserFilmAsync(string userId, UserFilmCreateDto createDto)
        {
            var entity = CustomMapper.MapToEntity(userId, createDto);
            CustomValidationHelper.Validate(entity);

            // set image
            //if (entity.Title.Length > 3)
            //{
            //    var imagesResult = await _imageService.FindImagesAsync(sourceLanguageCode: entity.LanguageCode, entity.Title);

            //    if (imagesResult.Any())
            //    {
            //        // try to find suitable image
            //        var image = _imageService.GetSuitableImages(imagesResult);
            //        if (image != null)
            //        {
            //            entity.Image = new WordImageEntity
            //            {
            //                Url = image.Url,
            //                Height = image.Height,
            //                Width = image.Width,
            //                Thumbnail = image.Thumbnail,
            //                ThumbnailHeight = image.ThumbnailHeight,
            //                ThumbnailWidth = image.ThumbnailWidth,
            //                Base64Encoding = image.Base64Encoding,
            //            };
            //        }
            //    }
            //}

            await _dataRepository.AddAsync(entity);
            return CustomMapper.MapToDto(entity);
        }

        public async Task<UserFilmDto> UpdateUserFilmAsync(string userId, string userFilmId, UserFilmUpdateDto updateDto)
        {
            var entity = await _dataRepository.GetOneAsync<UserFilmEntity>(x => x.Id == userFilmId);
            if (entity.UserId != userId)
            {
                throw new AccessDeniedException();
            }

            // update
            entity.UpdateSelf(updateDto);

            //// set image
            //if (entity.Image == null)
            //{
            //    if (entity.Title.Length > 3)
            //    {
            //        var imagesResult = await _imageService.FindImagesAsync(sourceLanguageCode: entity.LanguageCode, entity.Title);

            //        if (imagesResult.Any())
            //        {
            //            // try to find suitable image
            //            var image = _imageService.GetSuitableImages(imagesResult);
            //            if (image != null)
            //            {
            //                entity.Image = new WordImageEntity
            //                {
            //                    Url = image.Url,
            //                    Height = image.Height,
            //                    Width = image.Width,
            //                    Thumbnail = image.Thumbnail,
            //                    ThumbnailHeight = image.ThumbnailHeight,
            //                    ThumbnailWidth = image.ThumbnailWidth,
            //                    Base64Encoding = image.Base64Encoding,
            //                };
            //            }
            //        }
            //    }
            //}

            await _dataRepository.UpdateAsync(entity);
            return CustomMapper.MapToDto(entity);
        }

        public async Task DeleteUserFilm(string userId, string userFilmId)
        {
            var existing = await _dataRepository.GetOneAsync<UserFilmEntity>(x => x.Id == userFilmId && x.UserId == userId);
            if (existing == null)
            {
                throw new NotFoundException();
            }
            await _dataRepository.DeleteAsync<UserFilmEntity>(x => x.Id == existing.Id);
        }

    }
}
