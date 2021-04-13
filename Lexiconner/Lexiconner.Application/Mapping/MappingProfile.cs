using AutoMapper;
using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Dtos.CustomCollections;
using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Text;
using static Lexiconner.Application.ApiClients.Dtos.ImageSearchResponseDto;
using Lexiconner.Domain.Dtos.UserFilms;
using Lexiconner.Domain.Dtos.Users;
using Lexiconner.Domain.Entitites.General;
using Lexiconner.Domain.Dtos.General;
using Lexiconner.Domain.Dtos.WordSets;

namespace Lexiconner.Application.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            // General
            CreateMap<GeneralImageDto, GeneralImageEntity>();
            CreateMap<GeneralImageEntity, GeneralImageDto>();

            // User
            CreateMap<ApplicationUserEntity, UserDto>();

            // Word
            CreateMap<WordEntity, WordDto>();
            CreateMap<WordCreateDto, WordEntity>();
            CreateMap<ImageSearchResponseItemDto, GeneralImageDto>();
            CreateMap<WordTrainingInfoEntity, WordTrainingInfoDto>();

            // WordSet
            CreateMap<WordSetEntity, WordSetDto>();

            // CustomCollection
            CreateMap<CustomCollectionEntity, CustomCollectionDto>();

            // Film
            CreateMap<UserFilmEntity, UserFilmDto>();
            CreateMap<UserFilmCreateDto, UserFilmEntity>();
        }
    }
}
