using AutoMapper;

namespace cAPParel.API.Profiles
{
    public class FileDataProfile : Profile
    {
        public FileDataProfile() 
        {
            CreateMap<Entities.FileData, Models.FileDataDto>();
            CreateMap<Models.FileDataForCreationDto, Entities.FileData>();
        }
    }
}
