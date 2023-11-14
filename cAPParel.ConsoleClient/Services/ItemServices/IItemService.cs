using cAPParel.ConsoleClient.Models;

namespace cAPParel.ConsoleClient.Services.ItemServices
{
    public interface IItemService
    {
        Task<LinkedResourceList<ItemDto>?> GetItemsDefault(bool includeLinks = false);
        Task<LinkedResourceList<ItemDto>?> GetItemsFriendly(bool includeLinks = false);
        Task<LinkedResourceList<ItemFullDto>?> GetItemsFull(bool includeLinks = false);
    }
}