﻿using AutoMapper;

namespace cAPParel.API.Profiles
{
    public class ItemProfile : Profile
    {
        public ItemProfile() 
        {
            CreateMap<Entities.Item, Models.ItemFullDto>();
            CreateMap<Models.ItemForCreationDto, Entities.Item>();
            CreateMap<Entities.Item, Models.ItemDto>();
            CreateMap<Entities.Item, Models.ItemForUpdateDto>();
            CreateMap<Models.ItemForUpdateDto, Entities.Item>();
        }
    }
}
