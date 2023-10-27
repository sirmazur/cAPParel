﻿using cAPParel.API.Filters;
using cAPParel.API.Helpers;
using cAPParel.API.Models;
using cAPParel.API.Services.CategoryServices;
using cAPParel.API.Services.FieldsValidationServices;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
        private readonly IFieldsValidationService _fieldsValidationService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public CategoryController(ICategoryService categoryService,
            IFieldsValidationService fieldsValidationService,
            ProblemDetailsFactory problemDetailsFactory)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _fieldsValidationService = fieldsValidationService ?? throw new ArgumentNullException(nameof(fieldsValidationService));
            _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
        }

        [HttpGet(Name = "GetCategories")]
        [HttpHead]
        public async Task<IActionResult> GetCategories(
            int? parentCategoryId, [FromQuery] ResourceParameters resourceParameters)
        {
            if(!_fieldsValidationService.TypeHasProperties<CategoryDto>(resourceParameters.Fields))
            {
                return BadRequest(
                    _problemDetailsFactory.CreateProblemDetails(HttpContext,
                    statusCode: 400,
                    detail: $"Not all provided data shaping fields exist" +
                    $" on the resource: {resourceParameters.Fields}"));
            }
            List<IFilter> filters = new List<IFilter>();
            if(parentCategoryId != null)
            {
                filters.Add( new NumericFilter("ParentCategoryId", parentCategoryId));
            }
            PagedList<CategoryDto>? categories = null;
            try
            {
                categories = await _categoryService.GetAllAsync(filters, resourceParameters);
            }
            catch (Exception ex) 
            {
                return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                    statusCode: 400,
                    detail: ex.Message));
            }
           

            var paginationMetadata = new
            {
                totalCount = categories.TotalCount,
                pageSize = categories.PageSize,
                currentPage = categories.CurrentPage,
                totalPages = categories.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            var links = CreateLinksForCollection(
                resourceParameters, 
                filters, 
                categories.HasNext,
                categories.HasPrevious
                );

            var shapedCategories = categories
                .ShapeData(resourceParameters.Fields);
            int categoriesIenumerator = 0;
            var shapedCategoriesWithLinks = shapedCategories
                .Select(category =>
                {
                    var categoryAsDictionary = category as IDictionary<string, object?>;
                    var categoryLinks = CreateLinks(
                        categories[categoriesIenumerator].Id, 
                        resourceParameters.Fields);
                    categoriesIenumerator++;
                    categoryAsDictionary.Add("links", categoryLinks);
                    return categoryAsDictionary;
                });
            
            var linkedCollectionResource = new
            {
                value = shapedCategoriesWithLinks,
                links
            };

            if(categories.Count() == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(linkedCollectionResource);
            }
        }
        [HttpGet("{categoryid}", Name = "GetCategory")]
        public async Task<IActionResult> GetCategory(int categoryid, string? fields)
        {
            if (!_fieldsValidationService.TypeHasProperties<CategoryDto>(fields))
            {
                return BadRequest(
                    _problemDetailsFactory.CreateProblemDetails(HttpContext,
                    statusCode: 400,
                    detail: $"Not all provided data shaping fields exist" +
                    $" on the resource: {fields}"));
            }

            var item = await _categoryService.GetExtendedByIdWithEagerLoadingAsync(categoryid);
            var links = CreateLinks(categoryid, fields);
            var linkedResourceToReturn = item.ShapeDataForObject(fields) as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);
                if (item!=null)
                {
                    return Ok(linkedResourceToReturn);
                }
                else
                {
                    return NotFound();
                }
        }
       

        [HttpDelete("{categorytodeleteid}", Name = "DeleteCategory")]
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

        [HttpPost(Name = "CreateCategory")]
        public async Task<ActionResult<CategoryDto>> CreateCategoryAsync(CategoryForCreationDto category)
        {
            var categoryDto = await _categoryService.CreateAsync(category);
            var links = CreateLinks(categoryDto.Id, null);
            var linkedResourceToReturn = categoryDto.ShapeDataForObject(null) as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);
            return CreatedAtRoute("GetCategory", new { categoryId = linkedResourceToReturn["Id"] }, linkedResourceToReturn);
        }

        [HttpPut("{categorytoupdateid}", Name = "UpdateCategory")]
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
        [HttpPatch("{categorytoupdateid}", Name = "PartiallyUpdateCategory")]
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
                         fields = resourceParameters.Fields,
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
                        fields = resourceParameters.Fields,
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
                        fields = resourceParameters.Fields,
                        pageNumber = resourceParameters.PageNumber,
                        pageSize = resourceParameters.PageSize,
                        parentCategoryId = parentId,
                        searchQuery = resourceParameters.SearchQuery,
                        orderBy = resourceParameters.OrderBy

                    });
            }

        }

        private IEnumerable<LinkDto> CreateLinks(
            int categoryid,
            string? fields)
        {
            var links = new List<LinkDto>();
            if(string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new (Url.Link("GetCategory", new { categoryid }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new (Url.Link("GetCategory", new { categoryid, fields }),
                    "self",
                    "GET"));
            }
            
            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForCollection(
            ResourceParameters resourceParameters,
            List<IFilter> filters,
            bool hasNext,
            bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(
            new (CreateCategoriesResourceUri(
            resourceParameters,
            filters,
            ResourceUriType.Current),
            "self",
            "GET"));

            if(hasNext)
            {
                links.Add(
                    new (CreateCategoriesResourceUri(
                        resourceParameters,
                        filters,
                        ResourceUriType.NextPage),
                        "nextPage",
                        "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new(CreateCategoriesResourceUri(
                        resourceParameters,
                        filters,
                        ResourceUriType.PreviousPage),
                        "nextPage",
                        "GET"));
            }
            return links;
        }

    }
}
