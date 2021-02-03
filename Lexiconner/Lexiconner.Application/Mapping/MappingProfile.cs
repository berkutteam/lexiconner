using AutoMapper;
using Lexiconner.Domain.Dtos.StudyItems;
using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            // StudyItem
            CreateMap<StudyItemEntity, StudyItemDto>();
            CreateMap<StudyItemImageEntity, StudyItemImageDto>();
            CreateMap<StudyItemTrainingInfoEntity, StudyItemTrainingInfoDto>();
        }
    }
}
