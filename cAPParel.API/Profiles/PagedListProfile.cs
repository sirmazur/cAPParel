using AutoMapper;
using cAPParel.API.Helpers;

namespace cAPParel.API.Profiles
{
    public class PagedListProfile : Profile
    {
        public PagedListProfile()
        {
            CreateMap(typeof(PagedList<>), typeof(PagedList<>)).ConvertUsing(typeof(PagedListConverter<,>));
        }
    }
}
