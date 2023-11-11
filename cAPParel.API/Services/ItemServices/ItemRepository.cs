using cAPParel.API.DbContexts;
using cAPParel.API.Entities;
using cAPParel.API.Models;
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

        public async Task<Piece> AddPieceAsync(Piece piece, int itemid)
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

        public async Task<OperationResult<FileDataDto>> DeleteFile(int id)
        {
            var file = await _context.Files.FirstOrDefaultAsync(f => f.Id == id);
            if (file == null)
            {
                return new OperationResult<FileDataDto> { IsSuccess=false, ErrorMessage=$"File with id={id} was not found.", HttpResponseCode=404};
            }
            _context.Files.Remove(file);
            await _context.SaveChangesAsync();
            return new OperationResult<FileDataDto> { IsSuccess=true, ErrorMessage=$"File with id={id} was not found.", HttpResponseCode=204 };
        }
    }
}
