using cAPParel.API.Filters;
using cAPParel.API.Helpers;
using cAPParel.API.Models;
using cAPParel.API.Services.CategoryServices;
using cAPParel.API.Services.FieldsValidationServices;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Dynamic;

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
        /// <summary>
        /// Generates a pdf file with the pricing for the category
        /// </summary>
        /// <param name="categoryid"></param>
        /// <returns></returns>
        [HttpGet("{categoryid}/pricelists", Name = "GetPricing")]
        public async Task<IActionResult> GetPricing(int categoryid)
        {
            var file = await _categoryService.GeneratePdfForCategoryAsync(categoryid);
            if (file.Item1 == null)
            {
                return NotFound();
            }
            return File(file.Item1, "application/pdf", file.Item2);
        }

        /// <summary>
        /// Gets categories
        /// </summary>
        /// <param name="parentcategoryid">Parent category filter</param>
        /// <param name="resourceParameters"></param>
        /// <param name="mediaType"></param>
        /// <returns>Linked/Unlinked IEnumerable of categories</returns>
        [Produces("application/json",
           "application/vnd.capparel.hateoas+json",
           "application/vnd.capparel.category.full+json",
           "application/vnd.capparel.category.full.hateoas+json",
           "application/vnd.capparel.category.friendly+json",
           "application/vnd.capparel.category.friendly.hateoas+json")]
        [HttpGet(Name = "GetCategories")]
        [HttpHead]
        public async Task<IActionResult> GetCategories(
            int? parentcategoryid, [FromQuery] ResourceParameters resourceParameters, [FromHeader(Name ="Accept")]string? mediaType)
        {
            if(!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: 400,
                detail: $"Accept header media type value is not supported"));
            }
            if(!_fieldsValidationService.TypeHasProperties<CategoryDto>(resourceParameters.Fields))
            {
                return BadRequest(
                    _problemDetailsFactory.CreateProblemDetails(HttpContext,
                    statusCode: 400,
                    detail: $"Not all provided data shaping fields exist" +
                    $" on the resource: {resourceParameters.Fields}"));
            }
            List<IFilter> filters = new List<IFilter>();
            if(parentcategoryid != null)
            {
                filters.Add( new NumericFilter("ParentCategoryId", parentcategoryid));
            }
            
           
            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
            IEnumerable<LinkDto> links = new List<LinkDto>();

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix
                .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) :
                parsedMediaType.SubTypeWithoutSuffix;

           IEnumerable<ExpandoObject> shapedCategories = new List<ExpandoObject>();
            IEnumerable<IDictionary<string, object?>>? shapedCategoriesToReturn = new List<IDictionary<string,object?>>();
            if(primaryMediaType == "vnd.capparel.category.full")
            {
                PagedList<CategoryFullDto>? categories = null;
                try
                {
                    categories = await _categoryService.GetFullAllAsync(filters, resourceParameters);
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

                links = CreateLinksForCollection(
                resourceParameters,
                filters,
                categories.HasNext,
                categories.HasPrevious
                );
                shapedCategories = categories.ShapeData(resourceParameters.Fields);
                int categoriesIenumerator = 0;
                shapedCategoriesToReturn = shapedCategories
                    .Select(category =>
                    {
                        var categoryAsDictionary = category as IDictionary<string, object?>;
                        
                            var categoryLinks = CreateLinks(
                            categories[categoriesIenumerator].Id,
                            resourceParameters.Fields);
                            categoriesIenumerator++;
                            categoryAsDictionary.Add("links", categoryLinks);
                                                                                            
                        return categoryAsDictionary;
                    }).ToArray();
            }
            else
            {
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

                links = CreateLinksForCollection(
                resourceParameters,
                filters,
                categories.HasNext,
                categories.HasPrevious
                );
                shapedCategories = categories.ShapeData(resourceParameters.Fields);
                int categoriesIenumerator = 0;
                shapedCategoriesToReturn = shapedCategories
                    .Select(category =>
                    {
                        var categoryAsDictionary = category as IDictionary<string, object?>;
                        if (includeLinks)
                        {
                            var categoryLinks = CreateLinks(
                            categories[categoriesIenumerator].Id,
                            resourceParameters.Fields);
                            categoryAsDictionary.Add("links", categoryLinks);
                        }
                        categoriesIenumerator++;

                        return categoryAsDictionary;
                    }).ToArray();

            }


            if(shapedCategoriesToReturn.Count() == 0)
            {
                return NotFound();
            }

            if (includeLinks)
            {
                var linkedCollectionResource = new
                {
                    value = shapedCategoriesToReturn,
                    links
                };
                return Ok(linkedCollectionResource);
            }
            else
            {
                var CollectionResource = new
                {
                    value = shapedCategoriesToReturn
                };
                return Ok(CollectionResource);
            }
            



        }

        /// <summary>
        /// Gets a single category by id
        /// </summary>
        /// <param name="categoryid"></param>
        /// <param name="fields">Optional desired fields for data shaping</param>
        /// <param name="mediaType"></param>
        /// <returns>Shaped or unshaped Category</returns>
        [Produces("application/json",
            "application/vnd.capparel.hateoas+json",
            "application/vnd.capparel.category.full+json",
            "application/vnd.capparel.category.full.hateoas+json",
            "application/vnd.capparel.category.friendly+json",
            "application/vnd.capparel.category.friendly.hateoas+json")]
        [HttpGet("{categoryid}", Name = "GetCategory")]
        public async Task<IActionResult> GetCategory(
            int categoryid, 
            string? fields,
            [FromHeader(Name ="Accept")] string? mediaType)
        {
            if(!MediaTypeHeaderValue.TryParse(mediaType, out var parsedMediaType))
            {
                return BadRequest(
                    _problemDetailsFactory.CreateProblemDetails(HttpContext,
                    statusCode: 400,
                    detail: $"Accept header media type value is not supported"));
            }

            if (!_fieldsValidationService.TypeHasProperties<CategoryDto>(fields))
            {
                return BadRequest(
                    _problemDetailsFactory.CreateProblemDetails(HttpContext,
                    statusCode: 400,
                    detail: $"Not all provided data shaping fields exist" +
                    $" on the resource: {fields}"));
            }

            

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
            IEnumerable<LinkDto> links = new List<LinkDto>();

            if(includeLinks)
            {
                links = CreateLinks(categoryid, fields);
            }

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix.Substring(
                0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) :
                parsedMediaType.SubTypeWithoutSuffix;

            if(primaryMediaType == "vnd.capparel.category.full")
            {
                var fullItem = await _categoryService.GetExtendedByIdWithEagerLoadingAsync(categoryid);
                var fullResourceToReturn = fullItem.ShapeDataForObject(fields) as IDictionary<string, object>;
                if(includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }
                return Ok(fullResourceToReturn);
            }
            var item = await _categoryService.GetExtendedByIdWithEagerLoadingAsync(categoryid);
        
            var lightResourceToReturn = item.ShapeDataForObject(fields) as IDictionary<string, object>;
            if(includeLinks)
            {
                lightResourceToReturn.Add("links", links);
            }
            return Ok(lightResourceToReturn);
        }

        /// <summary>
        /// Deletes a category by id, Requires admin token
        /// </summary>
        /// <param name="categorytodeleteid"></param>
        /// <returns></returns>
        [Authorize(Policy = "MustBeAdmin")]
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

        /// <summary>
        /// Creates a new category, Requires admin token
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [Authorize(Policy = "MustBeAdmin")]
        [HttpPost(Name = "CreateCategory")]
        public async Task<ActionResult<CategoryDto>> CreateCategoryAsync(CategoryForCreationDto category)
        {
            var categoryDto = await _categoryService.CreateAsync(category);
            var links = CreateLinks(categoryDto.Id, null);
            var linkedResourceToReturn = categoryDto.ShapeDataForObject(null) as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);
            return CreatedAtRoute("GetCategory", new { categoryId = linkedResourceToReturn["Id"] }, linkedResourceToReturn);
        }


        /// <summary>
        /// Updates a category by id, Requires admin token
        /// </summary>
        /// <param name="categorytoupdateid"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        [Authorize(Policy = "MustBeAdmin")]
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

        /// <summary>
        /// Partially updates a category by id, Requires admin token
        /// </summary>
        /// <param name="categorytoupdateid"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        [Authorize(Policy = "MustBeAdmin")]
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
