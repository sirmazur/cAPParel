using cAPParel.BlazorApp.Models;
using cAPParel.BlazorApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using Color = cAPParel.BlazorApp.Models.Color;
using cAPParel.BlazorApp.HttpClients;
using Microsoft.AspNetCore.JsonPatch;

namespace cAPParel.BlazorApp.Services.ItemServices
{
    public class ItemService : IItemService
    {
        private readonly cAPParelAPIClient _client;
        public ItemService(cAPParelAPIClient client)
        {
            _client = client ??
                throw new ArgumentNullException(nameof(client));
        }

        //"application/json",
        //    "application/vnd.capparel.hateoas+json",
        //    "application/vnd.capparel.item.full+json",
        //    "application/vnd.capparel.item.full.hateoas+json",
        //    "application/vnd.capparel.item.friendly+json",
        //    "application/vnd.capparel.item.friendly.hateoas+json"

        public async Task<ItemDto> CreateItemAsync(ItemForCreationDto itemToCreate)
        {
            var item = await _client.CreateResourceAsync<ItemForCreationDto, ItemDto>(itemToCreate, "api/items", "application/json");
            return item;
        }

        public async Task<ItemDto> GetItemAsync(int itemId)
        {
            var item = await _client.GetResourceAsync<ItemDto>($"api/items/{itemId}", "application/vnd.capparel.item.friendly+json");
            return item;
        }
        public async Task<ItemFullDto> GetItemFullAsync(int itemId)
        {
            var item = await _client.GetResourceAsync<ItemFullDto>($"api/items/{itemId}", "application/vnd.capparel.item.full+json");
            return item;
        }
        public async Task<LinkedResourceList<ItemFullDto>?> GetItemsFullAsync(ItemFilters? filters = null)
        {
            var route = "api/items";
            if (filters is null)
            {
                return await _client.GetResourcesAsync<ItemFullDto>(route, "application/vnd.capparel.item.full+json");
            }

            var queryString = QueryStringBuilder.BuildQueryString(
                ("size", filters.size),
                ("categoryid", filters.categoryid),
                ("isavailable", filters.isAvailable),
                ("ids", filters.ids)
            );

            if (!string.IsNullOrEmpty(queryString))
            {
                route = $"{route}?{queryString}";
            }

            if(filters.includeLinks is not null && filters.includeLinks is true)
                return await _client.GetResourcesAsync<ItemFullDto>(route, "application/vnd.capparel.item.full.hateoas+json");
            else
                return await _client.GetResourcesAsync<ItemFullDto>(route, "application/vnd.capparel.item.full+json");


        }

        public async Task PatchItemAsync(double newMultiplier, int itemToUpdateId)
        {
            var patchDocument = new JsonPatchDocument<ItemForUpdateDto>();
            patchDocument.Replace(o => o.PriceMultiplier, newMultiplier);
            await _client.PatchResourceAsync<ItemForUpdateDto>(patchDocument, $"api/items/{itemToUpdateId}", "application/json");
        }

        public async Task<LinkedResourceList<ItemDto>?> GetItemsFriendlyAsync(ItemFilters? filters = null)
        {
            var route = "api/items";
            if(filters is null)
            {
                return await _client.GetResourcesAsync<ItemDto>(route, "application/vnd.capparel.item.full+json");
            }
            var queryString = QueryStringBuilder.BuildQueryString(
                ("size", filters.size),
                ("categoryid", filters.categoryid),
                ("isAvailable", filters.isAvailable),
                ("color", filters.color.ToString()),
                ("ids", filters.ids)
            );

            if (!string.IsNullOrEmpty(queryString))
            {
                route = $"{route}?{queryString}";
            }

            if (filters.includeLinks is not null && filters.includeLinks is true)
                return await _client.GetResourcesAsync<ItemDto>(route, "application/vnd.capparel.item.friendly.hateoas+json");
            else
                return await _client.GetResourcesAsync<ItemDto>(route, "application/vnd.capparel.item.friendly+json");
        }

        public async Task<LinkedResourceList<ItemDto>?> GetItemsFriendlyByQueryAsync(string query, bool? includeLinks = false)
        {
            var route = $"api/items?{query}";

            if (includeLinks is not null && includeLinks is true)
                return await _client.GetResourcesAsync<ItemDto>(route, "application/vnd.capparel.item.friendly.hateoas+json");
            else
                return await _client.GetResourcesAsync<ItemDto>(route, "application/vnd.capparel.item.friendly+json");
        }

        public async Task<LinkedResourceList<ItemFullDto>?> GetItemsFullByQueryAsync(string query, bool? includeLinks = false)
        {
            var route = $"api/items?{query}";

            if (includeLinks is not null && includeLinks is true)
                return await _client.GetResourcesAsync<ItemFullDto>(route, "application/vnd.capparel.item.full.hateoas+json");
            else
                return await _client.GetResourcesAsync<ItemFullDto>(route, "application/vnd.capparel.item.full+json");
        }

        public async Task DeletePieceAsync(int pieceId)
        {
            var route = $"api/items/pieces/{pieceId}";
            try
            {
                await _client.DeleteResourceAsync(route);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await Task.Delay(3000);
            }
        }

        public async Task DeleteItemAsync(int itemId)
        {
            var route = $"api/items/{itemId}";
            try
            {
                await _client.DeleteResourceAsync(route);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await Task.Delay(3000);
            }
        }

        public async Task<PieceDto> CreatePieceAsync(int itemId, PieceForCreationDto piece)
        {
            var route = $"api/items/{itemId}/pieces";
            var createdPiece = await _client.CreateResourceAsync<PieceForCreationDto, PieceDto>(piece, route);
            return createdPiece;
        }
    }

    public class ItemFilters
    {
        public string? size = null;
        public string? OrderBy = null;
        public int? categoryid = null;
        public bool? includeLinks = null;
        public bool? isAvailable = null;
        public Color? color = null;
        public List<int>? ids = null;
    }
}
