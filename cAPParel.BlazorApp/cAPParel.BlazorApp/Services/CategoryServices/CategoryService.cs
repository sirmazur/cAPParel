using cAPParel.BlazorApp.Helpers;
using cAPParel.BlazorApp.HttpClients;
using cAPParel.BlazorApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.BlazorApp.Services.CategoryServices
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

        public async Task GeneratePricingPdf(int categoryId)
        {

            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            try
            {
                await _client.DownloadFileAsync($"api/categories/{categoryId}/pricelists", folderPath);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryForCreationDto category)
        {

            try
            {
                var response = await _client.CreateResourceAsync<CategoryForCreationDto,CategoryDto>(category, "api/categories");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.Delay(3000);
                return new CategoryDto();
            }

        }

        public async Task DeleteCategoryAsync(int id)
        {
            try
            {
                await _client.DeleteResourceAsync($"api/categories/{id}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
