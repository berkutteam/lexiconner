using AutoMapper;
using Lexiconner.Domain.Dtos.StudyItems;
using Lexiconner.Domain.DTOs.CustomCollections;
using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Text;
using static Lexiconner.Application.ApiClients.Dtos.ImageSearchResponseDto;

namespace Lexiconner.Application.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            // StudyItem
            CreateMap<StudyItemEntity, StudyItemDto>();
            CreateMap<StudyItemImageEntity, StudyItemImageDto>();
            CreateMap<ImageSearchResponseItemDto, StudyItemImageDto>();
            CreateMap<StudyItemImageDto, StudyItemImageEntity>();
            CreateMap<StudyItemTrainingInfoEntity, StudyItemTrainingInfoDto>();

            // CustomCollection
            CreateMap<CustomCollectionEntity, CustomCollectionDto>();
        }
    }
}
