using cAPParel.API.Entities;
using cAPParel.API.Models;
using cAPParel.API.Services.Basic;

namespace cAPParel.API.Services.ItemServices
{
    public interface IItemService : IBasicService<ItemDto, Item, ItemFullDto, ItemForCreationDto, ItemForUpdateDto>
    {
        Task<PieceDto> CreatePieceAsync(PieceForCreationDto pieceForCreationDto, int itemid);
    }
}
