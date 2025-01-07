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
    public class ItemMappingProfile : Profile
    {
        public ItemMappingProfile()
        {
            // Polymorphic mapping for base type `Item`
            CreateMap<Item, ItemDto>()
                .Include<Cologne, ItemDto>()
                .Include<Watch, ItemDto>()
                .Include<Sneaker, ItemDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DateAcquired, opt => opt.MapFrom(src => src.DateAcquired))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.SerialNumber, opt => opt.MapFrom(src => src.SerialNumber))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.ItemTypeName, opt => opt.MapFrom(src => src.ItemType.Name))
                .ForMember(dest => dest.ItemTypeId, opt => opt.MapFrom(src => src.ItemTypeId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ReverseMap(); // Reverse mapping for `ItemDto` to `Item`

            // Mapping for `Cologne`
            CreateMap<Cologne, ItemDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Concentration, opt => opt.MapFrom(src => src.Concentration))
                .ForMember(dest => dest.FragranceNotes, opt => opt.MapFrom(src => src.FragranceNotes))
                .IncludeBase<Item, ItemDto>()
                .ReverseMap(); // Reverse mapping for `ItemDto` to `Cologne`

            CreateMap<CreateItemDto, Cologne>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Concentration, opt => opt.MapFrom(src => src.Concentration))
                .ForMember(dest => dest.FragranceNotes, opt => opt.MapFrom(src => src.FragranceNotes));

            CreateMap<UpdateItemDto, Cologne>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Concentration, opt => opt.MapFrom(src => src.Concentration))
                .ForMember(dest => dest.FragranceNotes, opt => opt.MapFrom(src => src.FragranceNotes));

            // Mapping for `Watch`
            CreateMap<Watch, ItemDto>()
                .ForMember(dest => dest.MovementType, opt => opt.MapFrom(src => src.MovementType))
                .ForMember(dest => dest.CaseMaterial, opt => opt.MapFrom(src => src.CaseMaterial))
                .ForMember(dest => dest.CaseDiameter, opt => opt.MapFrom(src => src.CaseDiameter))
                .ForMember(dest => dest.CaseThickness, opt => opt.MapFrom(src => src.CaseThickness))
                .ForMember(dest => dest.BandMaterial, opt => opt.MapFrom(src => src.BandMaterial))
                .ForMember(dest => dest.BandWidth, opt => opt.MapFrom(src => src.BandWidth))
                .IncludeBase<Item, ItemDto>()
                .ReverseMap(); // Reverse mapping for `ItemDto` to `Watch`

            CreateMap<CreateItemDto, Watch>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.MovementType, opt => opt.MapFrom(src => src.MovementType))
                .ForMember(dest => dest.CaseMaterial, opt => opt.MapFrom(src => src.CaseMaterial))
                .ForMember(dest => dest.CaseDiameter, opt => opt.MapFrom(src => src.CaseDiameter))
                .ForMember(dest => dest.CaseThickness, opt => opt.MapFrom(src => src.CaseThickness))
                .ForMember(dest => dest.BandMaterial, opt => opt.MapFrom(src => src.BandMaterial))
                .ForMember(dest => dest.BandWidth, opt => opt.MapFrom(src => src.BandWidth));

            CreateMap<UpdateItemDto, Watch>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.MovementType, opt => opt.MapFrom(src => src.MovementType))
                .ForMember(dest => dest.CaseMaterial, opt => opt.MapFrom(src => src.CaseMaterial))
                .ForMember(dest => dest.CaseDiameter, opt => opt.MapFrom(src => src.CaseDiameter))
                .ForMember(dest => dest.CaseThickness, opt => opt.MapFrom(src => src.CaseThickness))
                .ForMember(dest => dest.BandMaterial, opt => opt.MapFrom(src => src.BandMaterial))
                .ForMember(dest => dest.BandWidth, opt => opt.MapFrom(src => src.BandWidth));

            // Mapping for `Sneaker`
            CreateMap<Sneaker, ItemDto>()
                .ForMember(dest => dest.Colorway, opt => opt.MapFrom(src => src.Colorway))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
                .IncludeBase<Item, ItemDto>()
                .ReverseMap(); // Reverse mapping for `ItemDto` to `Sneaker`

            CreateMap<CreateItemDto, Sneaker>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Colorway, opt => opt.MapFrom(src => src.Colorway))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size));

            CreateMap<UpdateItemDto, Sneaker>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Colorway, opt => opt.MapFrom(src => src.Colorway))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size));



        }
    }
}
