using cAPParel.ConsoleClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.ConsoleClient.Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly cAPParelAPIClient _client;
        public OrderService(cAPParelAPIClient client)
        {
            _client = client ??
                throw new ArgumentNullException(nameof(client));
        }

        public async Task<OrderDto> CreateOrderAsync(List<int> piecesIds)
        {

            try
            {
                var response = await _client.CreateResourceAsync<List<int>, OrderDto>(piecesIds, "api/orders");
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<OrderFullDto> GetOrderFullAsync(int orderId)
        {
            var order = await _client.GetResourceAsync<OrderFullDto>($"api/orders/{orderId}", "application/vnd.capparel.order.full+json");
            return order;
        }

        public async Task CancelOrderAsync(int orderId)
        {
            try
            {
                await _client.DeleteResourceAsync($"api/orders/{orderId}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
