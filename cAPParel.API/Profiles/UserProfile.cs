﻿using AutoMapper;
using cAPParel.API.Entities;
using cAPParel.API.Models;

namespace cAPParel.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserForCreationDto, User>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<User, UserForUpdateDto>();
            CreateMap<User, UserDto>();
            CreateMap<User, UserFullDto>();
        }
    }
}
