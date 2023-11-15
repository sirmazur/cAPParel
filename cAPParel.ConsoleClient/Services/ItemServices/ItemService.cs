using cAPParel.ConsoleClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.ConsoleClient.Services.ItemServices
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

        public async Task<LinkedResourceList<ItemFullDto>?> GetItemsFull(bool includeLinks = false)
        {
            if (includeLinks)
            {
                return await _client.GetResourcesAsync<ItemFullDto>("api/items","application/vnd.capparel.item.full.hateoas+json");
            }
            else
            {
                return await _client.GetResourcesAsync<ItemFullDto>("api/items", "application/vnd.capparel.item.full+json");
            }
        }

        public async Task<LinkedResourceList<ItemDto>?> GetItemsFriendly(bool includeLinks = false)
        {
            if (includeLinks)
            {
                return await _client.GetResourcesAsync<ItemDto>("api/items", "application/vnd.capparel.item.friendly.hateoas+json");
            }
            else
            {
                return await _client.GetResourcesAsync<ItemDto>("api/items", "application/vnd.capparel.item.friendly+json");
            }
        }
    }
}
