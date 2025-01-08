using Amazon.SimpleEmail.Model.Internal.MarshallTransformations;
using AutoMapper;
using CollectIQ.Core.Dtos;
using CollectIQ.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectIQ.Core.Mappings
{
    public class ItemTypeMappingProfile : Profile
    {

        public ItemTypeMappingProfile() { 
            
            CreateMap<ItemType, ItemTypeDto>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id)
                )
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src => src.Name)
                );

        }

    }
}
