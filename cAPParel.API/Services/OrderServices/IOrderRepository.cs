using cAPParel.API.Entities;
using System.Linq.Expressions;

namespace cAPParel.API.Services.OrderServices
{
    public interface IOrderRepository
    {
        Task<List<Piece>> GetPiecesAsync(List<int> ids, params Expression<Func<Piece, object>>[] includeProperties);
        Task<User> GetUserAsync(int id);
    }
}
