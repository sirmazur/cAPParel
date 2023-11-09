using cAPParel.API.DbContexts;
using cAPParel.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace cAPParel.API.Services.ItemServices
{
    public class ItemRepository : IItemRepository
    {
        private readonly cAPParelContext _context;
        public ItemRepository(cAPParelContext context)
        {
            _context = context;
        }

        public async Task<Piece> AddPieceAsync(Piece piece,int itemid)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemid);
            if (item == null) 
            {
                throw new Exception($"Item with id={itemid} was not found.");
            }
            item.Pieces.Add(piece);
            await _context.SaveChangesAsync();
            return piece;
        }
    }
}
