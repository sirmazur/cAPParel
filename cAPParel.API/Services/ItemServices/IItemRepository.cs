using cAPParel.API.Entities;
using cAPParel.API.Models;

namespace cAPParel.API.Services.ItemServices
{
    public interface IItemRepository
    {
        Task<Piece> AddPieceAsync(Piece piece, int itemid);
        Task<OperationResult<FileDataDto>> DeleteFile(int id);
}
}
