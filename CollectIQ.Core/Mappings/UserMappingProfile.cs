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
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserRegistrationDto, User>();
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
