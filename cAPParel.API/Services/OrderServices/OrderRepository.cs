using cAPParel.API.DbContexts;
using cAPParel.API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace cAPParel.API.Services.OrderServices
{
    public class OrderRepository : IOrderRepository
    {
        private readonly cAPParelContext _context;
        public OrderRepository(cAPParelContext context)
        {
            _context = context;
        }
        public async Task<List<Piece>> GetPiecesAsync(List<int> ids, params Expression<Func<Piece, object>>[] includeProperties)
        {
            var query = _context.Set<Piece>().AsQueryable();
            List<Piece> pieces = new List<Piece>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            foreach(var id in ids)
            {
                var piece = await query.SingleOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
                if (piece == null)
                    throw new Exception("Piece not found");
                pieces.Add(piece);
            }
            return pieces;
        }
        public async Task<User> GetUserAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

    }
}
