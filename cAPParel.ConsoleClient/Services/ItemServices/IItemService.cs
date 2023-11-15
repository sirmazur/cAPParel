using cAPParel.ConsoleClient.Models;

namespace cAPParel.ConsoleClient.Services.ItemServices
{
    public interface IItemService
    {
        Task<LinkedResourceList<ItemDto>?> GetItemsFriendly(ItemFilters? filters = null);
        Task<LinkedResourceList<ItemFullDto>?> GetItemsFull(ItemFilters? filters = null);
    }
}