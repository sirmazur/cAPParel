﻿using AutoMapper;

namespace cAPParel.API.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Entities.Category, Models.CategoryDto>();
            CreateMap<Entities.Category, Models.CategoryFullDto>();
            CreateMap<Models.CategoryForCreationDto, Entities.Category>().ReverseMap();
            CreateMap<Models.CategoryForUpdateDto, Entities.Category>().ReverseMap();
        }
    }
}
