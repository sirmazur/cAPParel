using AutoMapper;

namespace cAPParel.API.Profiles
{
    public class ImageProfile : Profile
    {
        public ImageProfile()
        {
            CreateMap<Entities.Image, Models.ImageDto>();
            CreateMap<Models.ImageForCreationDto, Entities.Image>();
        }
    }
}
