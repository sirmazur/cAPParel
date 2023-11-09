using cAPParel.API.Entities;

namespace cAPParel.API.Services.ItemServices
{
    public interface IItemRepository
    {
        Task<Piece> AddPieceAsync(Piece piece, int itemid);
    }
}
