using AutoMapper;
using cAPParel.API.Entities;
using cAPParel.API.Models;

namespace cAPParel.API.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderForCreationDto, Order>();
            CreateMap<Order, OrderForUpdateDto>();
            CreateMap<OrderForUpdateDto, Order>();
            CreateMap<Order, OrderDto>();
            CreateMap<Order, OrderFullDto>();
        }
    }
}
