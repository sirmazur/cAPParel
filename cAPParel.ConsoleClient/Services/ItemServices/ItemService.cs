﻿using cAPParel.ConsoleClient.Models;
using cAPParel.ConsoleClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;

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

        public async Task<LinkedResourceList<ItemFullDto>?> GetItemsFull(ItemFilters? filters = null)
        {
            var route = "api/items";
            if (filters is null)
            {
                return await _client.GetResourcesAsync<ItemFullDto>(route, "application/vnd.capparel.item.full+json");
            }

            var queryString = QueryStringBuilder.BuildQueryString(
                ("size", filters.size),
                ("categoryid", filters.categoryid),
                ("isavailable", filters.isAvailable)
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

        public async Task<LinkedResourceList<ItemDto>?> GetItemsFriendly(ItemFilters? filters = null)
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
                ("color", filters.color.ToString())
            );

            if (!string.IsNullOrEmpty(queryString))
            {
                route = $"{route}?{queryString}";
            }

            if (filters.includeLinks is not null && filters.includeLinks is true)
                return await _client.GetResourcesAsync<ItemDto>(route, "application/vnd.capparel.item.full.hateoas+json");
            else
                return await _client.GetResourcesAsync<ItemDto>(route, "application/vnd.capparel.item.full+json");
        }
    }

    public class ItemFilters
    {
        public string? size = null;
        public int? categoryid = null;
        public bool? includeLinks = null;
        public bool isAvailable = false;
        public Color? color = null;
    }
}
