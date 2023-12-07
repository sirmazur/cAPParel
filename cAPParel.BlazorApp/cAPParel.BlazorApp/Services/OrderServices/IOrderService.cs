﻿using cAPParel.BlazorApp.Models;

namespace cAPParel.BlazorApp.Services.OrderServices
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(List<int> piecesIds);
        Task CancelOrderAsync(int orderId);
        Task<OrderFullDto> GetOrderFullAsync(int orderId);
        Task<LinkedResourceList<OrderFullDto>?> GetOrdersFullAsync(string? orderBy, bool? includeLinks = false);
        Task PatchOrderAsync(State targetState, int orderToUpdateId);
    }
}