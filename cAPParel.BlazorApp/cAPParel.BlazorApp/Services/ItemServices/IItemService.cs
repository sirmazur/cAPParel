using cAPParel.BlazorApp.Models;

namespace cAPParel.BlazorApp.Services.ItemServices
{
    public interface IItemService
    {
        Task<LinkedResourceList<ItemDto>?> GetItemsFriendlyAsync(ItemFilters? filters = null);
        Task<LinkedResourceList<ItemFullDto>?> GetItemsFullAsync(ItemFilters? filters = null);
        Task<LinkedResourceList<ItemDto>?> GetItemsFriendlyByQueryAsync(string query, bool? includeLinks = false);
        Task<LinkedResourceList<ItemFullDto>?> GetItemsFullByQueryAsync(string query, bool? includeLinks = false);
        Task<ItemDto> GetItemAsync(int itemId);
        Task<ItemDto> CreateItemAsync(ItemForCreationDto itemToCreate);
        Task DeletePieceAsync(int pieceId);
        Task DeleteItemAsync(int itemId);
        Task<PieceDto> CreatePieceAsync(int itemId, PieceForCreationDto piece);
    }
}