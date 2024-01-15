using AutoMapper;
using cAPParel.API.Entities;
using cAPParel.API.Models;
using cAPParel.API.Services.Basic;
using cAPParel.API.Services.ItemServices;

namespace cAPParel.API.Services.OrderServices
{
    public class OrderService : BasicService<OrderDto, Order, OrderFullDto, OrderForCreationDto, OrderForUpdateDto>, IOrderService
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderRepository _orderRepository;

        public OrderService(IMapper mapper, IConfiguration configuration, IBasicRepository<Order> basicRepository, IOrderRepository orderRepository) : base(mapper, basicRepository)
        {
            _configuration = configuration;
            _orderRepository = orderRepository;
        }

        public async Task CancelOrderAsync(int orderId, int userId, Role role)
        {
            var order = await _basicRepository.GetByIdWithEagerLoadingAsync(orderId, c => c.Pieces);
            if(order.UserId != userId && role != Role.Admin || (order.State == State.Cancelled || order.State==State.Completed))
                throw new Exception("You are not allowed to cancel this order!");
            foreach (var piece in order.Pieces)
            {
                piece.IsAvailable = true;
            }
            order.State = State.Cancelled;
            var user = await _orderRepository.GetUserAsync(order.UserId);
            user.Saldo += order.TotalPrice;
            await _basicRepository.SaveChangesAsync();
        }

        public async Task<OrderDto> CreateOrder(OrderForCreationDto order)
        {
            var orderToAdd = _mapper.Map<Order>(order);
            var pieces = await _orderRepository.GetPiecesAsync(order.PiecesIds, c => c.Item);
            double total = 0;
            foreach (var piece in pieces)
            {
                total += piece.Item.Price * piece.Item.PriceMultiplier;
                if (piece.IsAvailable == false)
                    throw new Exception("One or more pieces are not available");
                piece.IsAvailable = false;
            }
            var user = await _orderRepository.GetUserAsync(order.UserId);
            if (user.Saldo < total)
                throw new Exception("Not enough money");
            else
                user.Saldo -= total;
            await _basicRepository.SaveChangesAsync();
            orderToAdd.TotalPrice = total;
            orderToAdd.Pieces = pieces;
            await _basicRepository.AddAsync(orderToAdd);
            await _basicRepository.SaveChangesAsync();
            return _mapper.Map<OrderDto>(orderToAdd);
        }

        public async Task<double> GetTotal(List<int> ids)
        {
            var pieces = await _orderRepository.GetPiecesAsync(ids, c => c.Item);
            double total = 0;
            foreach (var piece in pieces)
            {
                total += piece.Item.Price;
            }
            return total;
        }
    }
}
