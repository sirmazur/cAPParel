using AutoMapper;

namespace cAPParel.API.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Entities.Category, Models.CategoryDto>();
            CreateMap<Models.CategoryForCreationDto, Entities.Category>().ReverseMap();
        }
    }
}
