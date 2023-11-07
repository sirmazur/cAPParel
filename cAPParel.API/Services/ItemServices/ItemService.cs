using cAPParel.API.Entities;
using cAPParel.API.Models;
using cAPParel.API.Services.Basic;

namespace cAPParel.API.Services.ItemServices
{
    public class ItemService : BasicService<ItemFullDto, Item, ItemFullDto, ItemForCreationDto, ItemForUpdateDto>, IItemService
    {
    }
}
