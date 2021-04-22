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
using Lexiconner.Domain.Dtos.UserDictionaries;

namespace Lexiconner.Application.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            // General
            CreateMap<GeneralImageDto, GeneralImageEntity>();
            CreateMap<GeneralImageEntity, GeneralImageDto>();
            CreateMap<WordGeneralEntity, WordEntity>();
            CreateMap<WordEntity, WordGeneralEntity>();
            CreateMap<WordGeneralEntity, WordSetWordEntity>();
            CreateMap<WordSetWordEntity, WordGeneralEntity>();

            // User
            CreateMap<ApplicationUserEntity, UserDto>();

            // Word
            CreateMap<WordEntity, WordDto>();
            CreateMap<WordCreateDto, WordEntity>();
            CreateMap<ImageSearchResponseItemDto, GeneralImageDto>();
            CreateMap<WordTrainingInfoEntity, WordTrainingInfoDto>();

            // WordSet
            CreateMap<WordSetEntity, WordSetDto>();
            CreateMap<WordSetWordEntity, WordSetWordDto>();
            CreateMap<WordSetWordEntity, WordEntity>();

            // CustomCollection
            CreateMap<CustomCollectionEntity, CustomCollectionDto>();

            // UserDictionary
            CreateMap<UserDictionaryEntity, UserDictionaryDto>();
            CreateMap<UserDictionaryEntity, UserDictionaryDetailedDto>();
            CreateMap<UserDictionaryWordSetEntity, UserDictionaryWordSetDto>();

            // Film
            CreateMap<UserFilmEntity, UserFilmDto>();
            CreateMap<UserFilmCreateDto, UserFilmEntity>();
        }
    }
}
