using cAPParel.API.Entities;
using cAPParel.API.Models;
using cAPParel.API.Services.Basic;

namespace cAPParel.API.Services.OrderServices
{
    public interface IOrderService : IBasicService<OrderDto, Order, OrderFullDto, OrderForCreationDto, OrderForUpdateDto>
    {
        Task<double> GetTotal(List<int> ids);
        Task<OrderDto> CreateOrder(OrderForCreationDto order);
    }
}
