using cAPParel.ConsoleClient.Models;

namespace cAPParel.ConsoleClient.Services.OrderServices
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(List<int> piecesIds);
        Task CancelOrderAsync(int orderId);
        Task<OrderFullDto> GetOrderFullAsync(int orderId);
    }
}