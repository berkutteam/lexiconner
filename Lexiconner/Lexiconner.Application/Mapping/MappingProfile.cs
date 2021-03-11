using AutoMapper;
using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Dtos.CustomCollections;
using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Text;
using static Lexiconner.Application.ApiClients.Dtos.ImageSearchResponseDto;
using Lexiconner.Domain.Dtos.UserFilms;

namespace Lexiconner.Application.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            // Word
            CreateMap<WordEntity, WordDto>();
            CreateMap<WordImageEntity, WordImageDto>();
            CreateMap<WordCreateDto, WordEntity>();
            CreateMap<ImageSearchResponseItemDto, WordImageDto>();
            CreateMap<WordImageDto, WordImageEntity>();
            CreateMap<WordTrainingInfoEntity, WordTrainingInfoDto>();

            // CustomCollection
            CreateMap<CustomCollectionEntity, CustomCollectionDto>();

            // Film
            CreateMap<UserFilmEntity, UserFilmDto>();
            CreateMap<UserFilmCreateDto, UserFilmEntity>();
        }
    }
}
