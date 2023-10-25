using cAPParel.API.Filters;
using cAPParel.API.Helpers;
using cAPParel.API.Models;
using cAPParel.API.Services.CategoryServices;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Runtime.Versioning;
using System.Security.Permissions;

namespace cAPParel.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        [HttpGet(Name = "GetCategories")]
        [HttpHead]
        public async Task<ActionResult<PagedList<CategoryDto>>> GetCategories(
            int? parentCategoryId, [FromQuery] ResourceParameters resourceParameters)
        {
            List<IFilter> filters = new List<IFilter>();
            if(parentCategoryId != null)
            {
                filters.Add( new NumericFilter("ParentCategoryId", parentCategoryId));
            }
            var categories = await _categoryService.GetAllAsync(filters,resourceParameters);
           
            var previousPageLink = categories.HasPrevious 
                ? CreateCategoriesResourceUri(
                    resourceParameters,
                    filters,
                    ResourceUriType.PreviousPage) : null;

            var nextPageLink = categories.HasNext
                ? CreateCategoriesResourceUri(
                    resourceParameters,
                    filters,
                    ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = categories.TotalCount,
                pageSize = categories.PageSize,
                currentPage = categories.CurrentPage,
                totalPages = categories.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            if(categories.Count() == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(categories);
            }
        }
        [HttpGet("{categoryid}", Name = "GetCategory")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int categoryid)
        {         
                var item = await _categoryService.GetExtendedByIdWithEagerLoadingAsync(categoryid);
                if (item!=null)
                {
                    return Ok(item);
                }
                else
                {
                    return NotFound();
                }
        }
       

        [HttpDelete("{categorytodeleteid}")]
        public async Task<ActionResult> DeleteCategory(int categorytodeleteid)
        {
            var operationResult = await _categoryService.DeleteByIdAsync(categorytodeleteid);
            if (operationResult.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategoryAsync(CategoryForCreationDto category)
        {
            var categoryDto = await _categoryService.CreateAsync(category);
            return CreatedAtRoute("GetCategory", new { categoryId = categoryDto.Id }, categoryDto);
        }

        [HttpPut("{categorytoupdateid}")]
        public async Task<IActionResult> UpdateCategory(int categorytoupdateid, CategoryForUpdateDto category)
        {
            var operationResult = await _categoryService.UpdateAsync(categorytoupdateid, category);
            if (operationResult.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }
        [HttpPatch("{categorytoupdateid}")]
        public async Task<IActionResult> PartialUpdateCategory(int categorytoupdateid, JsonPatchDocument<CategoryForUpdateDto> patchDocument)
        {
            var operationResult = await _categoryService.PartialUpdateAsync(categorytoupdateid, patchDocument);
            if (operationResult.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }

        [HttpOptions()]
        public IActionResult GetCategoriesOptions()
        {
            Response.Headers.Add("Allow", "GET,HEAD,POST,PUT,PATCH,DELETE,OPTIONS");
            return Ok();
        }

        private string? CreateCategoriesResourceUri(
            ResourceParameters resourceParameters,
            List<IFilter> filters,
            ResourceUriType type)
        {
            int? parentId = (filters.Count()>1) ? Convert.ToInt32(filters[0].Value) : null;
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetCategories",
                     new
                     {
                         pageNumber = resourceParameters.PageNumber - 1,
                         pageSize = resourceParameters.PageSize,
                         parentCategoryId = parentId,
                         searchQuery = resourceParameters.SearchQuery,
                         orderBy = resourceParameters.OrderBy
                     });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCategories",
                    new
                    {
                        pageNumber = resourceParameters.PageNumber + 1,
                        pageSize = resourceParameters.PageSize,
                        parentCategoryId = parentId,
                        searchQuery = resourceParameters.SearchQuery,
                        orderBy = resourceParameters.OrderBy
                    });
                default:
                    return Url.Link("GetCategories",
                    new
                    {
                        pageNumber = resourceParameters.PageNumber,
                        pageSize = resourceParameters.PageSize,
                        parentCategoryId = parentId,
                        searchQuery = resourceParameters.SearchQuery,
                        orderBy = resourceParameters.OrderBy

                    });
            }

        }

    }
}
