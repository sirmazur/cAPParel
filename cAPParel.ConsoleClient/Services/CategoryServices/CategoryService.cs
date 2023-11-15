using cAPParel.ConsoleClient.Helpers;
using cAPParel.ConsoleClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.ConsoleClient.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly cAPParelAPIClient _client;
        public CategoryService(cAPParelAPIClient client)
        {
            _client = client;
        }

        public async Task<LinkedResourceList<CategoryFullDto>?> GetCategoriesFull(int? parentcategoryid = null, bool? includeLinks = false)
        {
            var route = "api/categories";

            var queryString = QueryStringBuilder.BuildQueryString(
                ("parentcategoryid", parentcategoryid)
            );

            if (!string.IsNullOrEmpty(queryString))
            {
                route = $"{route}?{queryString}";
            }

            if (includeLinks is not null && includeLinks is true)
                return await _client.GetResourcesAsync<CategoryFullDto>(route, "application/vnd.capparel.category.full.hateoas+json");
            else
                return await _client.GetResourcesAsync<CategoryFullDto>(route, "application/vnd.capparel.category.full+json");


        }

        public async Task<LinkedResourceList<CategoryDto>?> GetCategoriesFriendly(int? parentcategoryid = null, bool? includeLinks = false)
        {
            var route = "api/categories";

            var queryString = QueryStringBuilder.BuildQueryString(
                ("parentcategoryid", parentcategoryid)
            );

            if (!string.IsNullOrEmpty(queryString))
            {
                route = $"{route}?{queryString}";
            }

            if (includeLinks is not null && includeLinks is true)
                return await _client.GetResourcesAsync<CategoryDto>(route, "application/vnd.capparel.category.friendly.hateoas+json");
            else
                return await _client.GetResourcesAsync<CategoryDto>(route, "application/vnd.capparel.category.friendly+json");


        }
    }
}
