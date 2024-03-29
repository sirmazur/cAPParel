﻿using cAPParel.API.Entities;
using cAPParel.API.Filters;
using cAPParel.API.Helpers;
using cAPParel.API.Models;
using cAPParel.API.Services.CategoryServices;
using cAPParel.API.Services.FieldsValidationServices;
using cAPParel.API.Services.ItemServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Any;
using Newtonsoft.Json;
using System.Dynamic;
using System.Linq.Expressions;
using System.Net;

namespace cAPParel.API.Controllers
{
    [ApiController]
    [Route("api/items")]
    public class ItemController : ControllerBase
    {

        private readonly IItemService _itemService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IFieldsValidationService _fieldsValidationService;
        private readonly ICategoryService _categoryService;
        public ItemController(IItemService itemService, ProblemDetailsFactory problemDetailsFactory, IFieldsValidationService fieldsValidationService, ICategoryService categoryService)
        {
            _itemService = itemService;
            _problemDetailsFactory = problemDetailsFactory;
            _fieldsValidationService = fieldsValidationService;
            _categoryService = categoryService;
        }

        /// <summary>
        /// Adds a file to an item
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("{itemid}/files", Name = "CreateFile")]
        [Authorize(Policy = "MustBeAdmin")]
        public async Task<ActionResult<FileDataDto>> CreateImage(int itemid, FileDataForCreationDto file)
        {
            try
            {
                var result = await _itemService.AddFileDataToItemAsync(itemid, file);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Deletes a file by id
        /// </summary>
        /// <param name="fileid"></param>
        /// <returns></returns>
        [HttpDelete("files/{fileid}", Name = "DeleteFile")]
        [Authorize(Policy = "MustBeAdmin")]
        public async Task<ActionResult> DeleteFile(int fileid)
        {
            var result = await _itemService.DeleteFile(fileid);
            if(result.IsSuccess==true)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(result.HttpResponseCode, result.ErrorMessage);
            }
        }

        /// <summary>
        /// Deletes a piece by id
        /// </summary>
        /// <param name="pieceid"></param>
        /// <returns></returns>
        [HttpDelete("pieces/{pieceid}", Name = "DeletePiece")]
        [Authorize(Policy = "MustBeAdmin")]
        public async Task<ActionResult> DeletePiece(int pieceid)
        {
            var result = await _itemService.DeleteByIdAsync(pieceid);
            if (result.IsSuccess == true)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(result.HttpResponseCode, result.ErrorMessage);
            }
        }

        /// <summary>
        /// Deletes an item by id
        /// </summary>
        /// <param name="itemid"></param>
        /// <returns></returns>
        [HttpDelete("{itemid}", Name = "DeleteItem")]
        [Authorize(Policy = "MustBeAdmin")]
        public async Task<ActionResult> DeleteItem(int itemid)
        {
            var result = await _itemService.DeleteByIdAsync(itemid);
            if (result.IsSuccess == true)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(result.HttpResponseCode, result.ErrorMessage);
            }
        }

        /// <summary>
        /// Creates an item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost(Name = "CreateItem")]
        public async Task<ActionResult<ItemDto>> CreateItem(ItemForCreationDto item)
        {
            try
            {
                var result = await _itemService.CreateAsync(item);
                return CreatedAtRoute("GetItem", new { itemid = result.Id }, result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Creates a piece of an item with the given id
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="piece"></param>
        /// <returns></returns>
        [HttpPost("{itemid}/pieces", Name = "CreatePiece")]
        public async Task<ActionResult<PieceDto>> CreatePiece(int itemid, PieceForCreationDto piece)
        {
            try
            {
                var result = await _itemService.CreatePieceAsync(piece, itemid);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Gets shaped or unshaped item
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="fields"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        [Produces("application/json",
            "application/vnd.capparel.hateoas+json",
            "application/vnd.capparel.item.full+json",
            "application/vnd.capparel.item.full.hateoas+json",
            "application/vnd.capparel.item.friendly+json",
            "application/vnd.capparel.item.friendly.hateoas+json")]
        [HttpGet("{itemid}", Name = "GetItem")]
        public async Task<IActionResult> GetItem(
            int itemid,
            string? fields,
            [FromHeader(Name = "Accept")] string? mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out var parsedMediaType))
            {
                return BadRequest(
                    _problemDetailsFactory.CreateProblemDetails(HttpContext,
                    statusCode: 400,
                    detail: $"Accept header media type value is not supported"));
            }

            if (!_fieldsValidationService.TypeHasProperties<ItemFullDto>(fields))
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

            if (includeLinks)
            {
                links = CreateLinks(itemid, fields);
            }

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix.Substring(
                0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) :
                parsedMediaType.SubTypeWithoutSuffix;

            if (primaryMediaType == "vnd.capparel.item.full")
            {
                Expression<Func<Item, object>>[] includeProperties = { c => c.Pieces, c => c.FileData };
                var fullItem = await _itemService.GetExtendedByIdWithEagerLoadingAsync(itemid, includeProperties);
                var fullResourceToReturn = fullItem.ShapeDataForObject(fields) as IDictionary<string, object>;
                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }
                return Ok(fullResourceToReturn);
            }

            var item = await _itemService.GetExtendedByIdWithEagerLoadingAsync(itemid);

            var lightResourceToReturn = item.ShapeDataForObject(fields) as IDictionary<string, object>;
            if (includeLinks)
            {
                lightResourceToReturn.Add("links", links);
            }
            return Ok(lightResourceToReturn);
        }


        /// <summary>
        /// Gets optionally filtered or shaped items
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="categoryid"></param>
        /// <param name="isavailable"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="resourceParameters"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        [Produces("application/json",
          "application/vnd.capparel.hateoas+json",
          "application/vnd.capparel.item.full+json",
          "application/vnd.capparel.item.full.hateoas+json",
          "application/vnd.capparel.item.friendly+json",
          "application/vnd.capparel.item.friendly.hateoas+json")]
        [HttpGet(Name = "GetItems")]
        [HttpHead]
        public async Task<IActionResult> GetItems([FromQuery]IEnumerable<int>? ids, int? categoryid, bool? isavailable, string? size, [FromQuery]Color? color, [FromQuery] ResourceParameters resourceParameters, [FromHeader(Name = "Accept")] string? mediaType)
        {          
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: 400,
                detail: $"Accept header media type value is not supported"));
            }
            if (!_fieldsValidationService.TypeHasProperties<ItemFullDto>(resourceParameters.Fields))
            {
                return BadRequest(
                    _problemDetailsFactory.CreateProblemDetails(HttpContext,
                    statusCode: 400,
                    detail: $"Not all provided data shaping fields exist" +
                    $" on the resource: {resourceParameters.Fields}"));
            }

            List<IFilter> filters = new List<IFilter>();

            if(categoryid is not null)
            {
                filters.Add(new NumericFilter("CategoryIds", await _categoryService.GetRelatedCategoriesIds((int)categoryid)));
            }
            if(isavailable is not null && isavailable is true)
            {
                filters.Add(new NumericFilter("HasPieces", 1));
            }
            if(size is not null)
            {
                filters.Add(new TextFilter("Size", size));
            }
            if(color is not null)
            {
                filters.Add(new TextFilter("Color", color));
            }
            if(ids is not null && ids.Count()>0)
            {
                filters.Add(new NumericFilter("Ids", ids));
            }
            

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix
                .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) :
                parsedMediaType.SubTypeWithoutSuffix;

            IEnumerable<ExpandoObject> shapedObjects = new List<ExpandoObject>();
            IEnumerable<IDictionary<string, object?>>? shapedObjectsToReturn = new List<IDictionary<string, object?>>();
            if (primaryMediaType == "vnd.capparel.item.full")
            {
                PagedList<ItemFullDto>? items = null;
                try
                {
                    Expression<Func<Item, object>>[] includeProperties = { c => c.Pieces, c => c.FileData};
                    items = await _itemService.GetFullAllWithEagerLoadingAsync(filters, resourceParameters, includeProperties);
                }
                catch (Exception ex)
                {
                    return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                        statusCode: 400,
                        detail: ex.Message));
                }
                var paginationMetadata = new
                {
                    totalCount = items.TotalCount,
                    pageSize = items.PageSize,
                    currentPage = items.CurrentPage,
                    totalPages = items.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                links = CreateLinksForCollection(
                resourceParameters,
                filters,
                items.HasNext,
                items.HasPrevious
                );
                shapedObjects = items.ShapeData(resourceParameters.Fields);
                int Ienumerator = 0;
                shapedObjectsToReturn = shapedObjects
                    .Select(selectedobj =>
                    {
                        var objectAsDictionary = selectedobj as IDictionary<string, object?>;

                        var Links = CreateLinks(
                        items[Ienumerator].Id,
                        resourceParameters.Fields);
                        Ienumerator++;
                        objectAsDictionary.Add("links", Links);

                        return objectAsDictionary;
                    }).ToArray();
            }
            else
            {
                PagedList<ItemDto>? items = null;
                try
                {

                        Expression<Func<Item, object>>[] includeProperties = { c => c.Pieces, c => c.FileData};
                        items = await _itemService.GetAllWithEagerLoadingAsync(filters, resourceParameters, includeProperties);
                                                        
                }
                catch (Exception ex)
                {
                    return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                        statusCode: 400,
                        detail: ex.Message));
                }
                var paginationMetadata = new
                {
                    totalCount = items.TotalCount,
                    pageSize = items.PageSize,
                    currentPage = items.CurrentPage,
                    totalPages = items.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                links = CreateLinksForCollection(
                resourceParameters,
                filters,
                items.HasNext,
                items.HasPrevious
                );
                shapedObjects = items.ShapeData(resourceParameters.Fields);
                int Ienumerator = 0;
                shapedObjectsToReturn = shapedObjects
                    .Select(selectedobj =>
                    {
                        var objectAsDictionary = selectedobj as IDictionary<string, object?>;
                        if (includeLinks)
                        {
                            var Links = CreateLinks(
                            items[Ienumerator].Id,
                            resourceParameters.Fields);
                            objectAsDictionary.Add("links", Links);
                        }
                        Ienumerator++;

                        return objectAsDictionary;
                    }).ToArray();

            }
            if (shapedObjectsToReturn.Count() == 0)
            {
                return NotFound();
            }

            if (includeLinks)
            {
                var linkedCollectionResource = new
                {
                    value = shapedObjectsToReturn,
                    links
                };
                return Ok(linkedCollectionResource);
            }
            else
            {
                var CollectionResource = new
                {
                    value = shapedObjectsToReturn
                };
                return Ok(CollectionResource);
            }

        }

        [Authorize(Policy = "MustBeAdmin")]
        [HttpPatch("{itemtoupdateid}", Name = "PartiallyUpdateItem")]
        public async Task<IActionResult> PartialUpdateItem(int itemtoupdateid, JsonPatchDocument<ItemForUpdateDto> patchDocument)
        {
            var operationResult = await _itemService.PartialUpdateAsync(itemtoupdateid, patchDocument);
            if (operationResult.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }


        private string? CreateResourceUri(
          ResourceParameters resourceParameters,
          List<IFilter> filters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetItems",
                     new
                     {
                         fields = resourceParameters.Fields,
                         PageNumber = resourceParameters.PageNumber - 1,
                         PageSize = resourceParameters.PageSize,
                         searchQuery = resourceParameters.SearchQuery,
                         OrderBy = resourceParameters.OrderBy
                     });
                case ResourceUriType.NextPage:
                    return Url.Link("GetItems",
                    new
                    {
                        fields = resourceParameters.Fields,
                        PageNumber = resourceParameters.PageNumber + 1,
                        PageSize = resourceParameters.PageSize,
                        searchQuery = resourceParameters.SearchQuery,
                        OrderBy = resourceParameters.OrderBy
                    });
                default:
                    return Url.Link("GetItems",
                    new
                    {
                        fields = resourceParameters.Fields,
                        PageNumber = resourceParameters.PageNumber,
                        PageSize = resourceParameters.PageSize,
                        searchQuery = resourceParameters.SearchQuery,
                        OrderBy = resourceParameters.OrderBy

                    });
            }

        }

        private IEnumerable<LinkDto> CreateLinks(
            int id,
            string? fields)
        {
            var links = new List<LinkDto>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new(Url.Link("GetItem", new { id }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new(Url.Link("GetItem", new { id, fields }),
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
            new(CreateResourceUri(
            resourceParameters,
            filters,
            ResourceUriType.Current),
            "self",
            "GET"));

            if (hasNext)
            {
                links.Add(
                    new(CreateResourceUri(
                        resourceParameters,
                        filters,
                        ResourceUriType.NextPage),
                        "nextPage",
                        "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new(CreateResourceUri(
                        resourceParameters,
                        filters,
                        ResourceUriType.PreviousPage),
                        "previousPage",
                        "GET"));
            }
            return links;
        }

    }
}
