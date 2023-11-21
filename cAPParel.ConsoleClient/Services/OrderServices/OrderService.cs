using cAPParel.ConsoleClient.Helpers;
using cAPParel.ConsoleClient.Models;
using Microsoft.AspNetCore.JsonPatch;
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

        public async Task<LinkedResourceList<OrderFullDto>?> GetOrdersFullAsync(string? orderBy, bool? includeLinks = false)
        {
            var route = "api/orders";

            var queryString = QueryStringBuilder.BuildQueryString(
                ("orderBy", orderBy)
            );

            if (!string.IsNullOrEmpty(queryString))
            {
                route = $"{route}?{queryString}";
            }

            if (includeLinks is not null && includeLinks is true)
                return await _client.GetResourcesAsync<OrderFullDto>(route, "application/vnd.capparel.order.full.hateoas+json");
            else
                return await _client.GetResourcesAsync<OrderFullDto>(route, "application/vnd.capparel.order.full+json");


        }

        public async Task PatchOrderAsync(State targetState, int orderToUpdateId)
        {
            var patchDocument = new JsonPatchDocument<OrderForUpdateDto>();
            patchDocument.Replace(o => o.State, targetState);
            await _client.PatchResourceAsync<OrderForUpdateDto>(patchDocument, $"api/orders/{orderToUpdateId}", "application/json");
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
