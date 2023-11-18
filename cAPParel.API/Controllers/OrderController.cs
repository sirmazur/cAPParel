using cAPParel.API.Entities;
using cAPParel.API.Filters;
using cAPParel.API.Helpers;
using cAPParel.API.Models;
using cAPParel.API.Services.FieldsValidationServices;
using cAPParel.API.Services.OrderServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Dynamic;
using System.Security.Claims;

namespace cAPParel.API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IFieldsValidationService _fieldsValidationService;
        public OrderController(IOrderService orderService, ProblemDetailsFactory problemDetailsFactory, IFieldsValidationService fieldsValidationService)
        {
            _orderService = orderService;
            _problemDetailsFactory=problemDetailsFactory;
            _fieldsValidationService = fieldsValidationService;
        }

        [HttpPost(Name = "PlaceOrder")]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<ActionResult<OrderDto>> PlaceOrder(List<int> pieceIds)
        {
            try
            {
                var createdOrder = await _orderService.CreateOrder(new OrderForCreationDto
                {
                    PiecesIds = pieceIds,
                    UserId=int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value)
                });

                return CreatedAtRoute("GetOrder", new { orderid = createdOrder.Id }, createdOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = "MustBeAdmin")]
        [HttpPatch("{ordertoupdateid}", Name = "PartiallyUpdateOrder")]
        public async Task<IActionResult> PartialUpdateOrder(int ordertoupdateid, JsonPatchDocument<OrderForUpdateDto> patchDocument)
        {
            var operationResult = await _orderService.PartialUpdateAsync(ordertoupdateid, patchDocument);
            if (operationResult.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }
        [HttpDelete("{orderid}", Name = "CancelOrder")]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<IActionResult> CancelOrder(int orderid)
        {
            try
            {
                await _orderService.CancelOrderAsync(orderid,
                    int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value),
                    (Role)Enum.Parse(typeof(Role), User.Claims.FirstOrDefault(d => d.Type == ClaimTypes.Role)?.Value));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();

        }

        [Produces("application/json",
           "application/vnd.capparel.hateoas+json",
           "application/vnd.capparel.order.full+json",
           "application/vnd.capparel.order.full.hateoas+json",
           "application/vnd.capparel.order.friendly+json",
           "application/vnd.capparel.order.friendly.hateoas+json")]
        [HttpGet(Name = "GetOrders")]
        [Authorize(Policy = "MustBeAdmin")]
        [HttpHead]
        public async Task<IActionResult> GetOrders( [FromQuery] ResourceParameters resourceParameters, 
            [FromHeader(Name = "Accept")] string? mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: 400,
                detail: $"Accept header media type value is not supported"));
            }
            if (!_fieldsValidationService.TypeHasProperties<CategoryDto>(resourceParameters.Fields))
            {
                return BadRequest(
                    _problemDetailsFactory.CreateProblemDetails(HttpContext,
                    statusCode: 400,
                    detail: $"Not all provided data shaping fields exist" +
                    $" on the resource: {resourceParameters.Fields}"));
            }
            List<IFilter> filters = new List<IFilter>();


            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
            IEnumerable<LinkDto> links = new List<LinkDto>();

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix
                .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) :
                parsedMediaType.SubTypeWithoutSuffix;

            IEnumerable<ExpandoObject> shapedOrders = new List<ExpandoObject>();
            IEnumerable<IDictionary<string, object?>>? shapedOrdersToReturn = new List<IDictionary<string, object?>>();
            if (primaryMediaType == "vnd.capparel.order.full")
            {
                PagedList<OrderFullDto>? orders = null;
                try
                {
                    orders = await _orderService.GetFullAllAsync(filters, resourceParameters);
                }
                catch (Exception ex)
                {
                    return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                        statusCode: 400,
                        detail: ex.Message));
                }
                var paginationMetadata = new
                {
                    totalCount = orders.TotalCount,
                    pageSize = orders.PageSize,
                    currentPage = orders.CurrentPage,
                    totalPages = orders.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                links = CreateLinksForCollection(
                resourceParameters,
                filters,
                orders.HasNext,
                orders.HasPrevious
                );
                shapedOrders = orders.ShapeData(resourceParameters.Fields);
                int Ienumerator = 0;
                shapedOrdersToReturn = shapedOrders
                    .Select(order =>
                    {
                        var categoryAsDictionary = order as IDictionary<string, object?>;

                        var categoryLinks = CreateLinks(
                        orders[Ienumerator].Id,
                        resourceParameters.Fields);
                        Ienumerator++;
                        categoryAsDictionary.Add("links", categoryLinks);

                        return categoryAsDictionary;
                    }).ToArray();
            }
            else
            {
                PagedList<OrderDto>? orders = null;
                try
                {
                    orders = await _orderService.GetAllAsync(filters, resourceParameters);
                }
                catch (Exception ex)
                {
                    return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                        statusCode: 400,
                        detail: ex.Message));
                }
                var paginationMetadata = new
                {
                    totalCount = orders.TotalCount,
                    pageSize = orders.PageSize,
                    currentPage = orders.CurrentPage,
                    totalPages = orders.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                links = CreateLinksForCollection(
                resourceParameters,
                filters,
                orders.HasNext,
                orders.HasPrevious
                );
                shapedOrders = orders.ShapeData(resourceParameters.Fields);
                int Ienumerator = 0;
                shapedOrdersToReturn = shapedOrders
                    .Select(order =>
                    {
                        var orderAsDictionary = order as IDictionary<string, object?>;
                        if (includeLinks)
                        {
                            var orderLinks = CreateLinks(
                            orders[Ienumerator].Id,
                            resourceParameters.Fields);
                            orderAsDictionary.Add("links", orderLinks);
                        }
                        Ienumerator++;

                        return orderAsDictionary;
                    }).ToArray();

            }


            if (shapedOrdersToReturn.Count() == 0)
            {
                return NotFound();
            }

            if (includeLinks)
            {
                var linkedCollectionResource = new
                {
                    value = shapedOrdersToReturn,
                    links
                };
                return Ok(linkedCollectionResource);
            }
            else
            {
                var CollectionResource = new
                {
                    value = shapedOrdersToReturn
                };
                return Ok(CollectionResource);
            }




        }

        [Produces("application/json",
            "application/vnd.capparel.hateoas+json",
            "application/vnd.capparel.order.full+json",
            "application/vnd.capparel.order.full.hateoas+json",
            "application/vnd.capparel.order.friendly+json",
            "application/vnd.capparel.order.friendly.hateoas+json")]
        [HttpGet("{orderid}", Name = "GetOrder")]
        public async Task<IActionResult> GetOrder(
            int orderid,
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

            if (!_fieldsValidationService.TypeHasProperties<OrderDto>(fields))
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
                links = CreateLinks(orderid, fields);
            }

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix.Substring(
                0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) :
                parsedMediaType.SubTypeWithoutSuffix;
            if (primaryMediaType == "vnd.capparel.order.full")
            {
                var fullItem = await _orderService.GetExtendedByIdWithEagerLoadingAsync(orderid, c=> c.Pieces);
                var fullResourceToReturn = fullItem.ShapeDataForObject(fields) as IDictionary<string, object>;
                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }
                return Ok(fullResourceToReturn);
            }
            var item = await _orderService.GetExtendedByIdWithEagerLoadingAsync(orderid, c => c.Pieces);

            var lightResourceToReturn = item.ShapeDataForObject(fields) as IDictionary<string, object>;
            if (includeLinks)
            {
                lightResourceToReturn.Add("links", links);
            }
            return Ok(lightResourceToReturn);
        }

        private string? CreateOrdersResourceUri(
            ResourceParameters resourceParameters,
            List<IFilter> filters,
            ResourceUriType type)
        {
            int? parentId = (filters.Count()>1) ? Convert.ToInt32(filters[0].Value) : null;
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetOrders",
                     new
                     {
                         fields = resourceParameters.Fields,
                         pageNumber = resourceParameters.PageNumber - 1,
                         pageSize = resourceParameters.PageSize,
                         searchQuery = resourceParameters.SearchQuery,
                         orderBy = resourceParameters.OrderBy
                     });
                case ResourceUriType.NextPage:
                    return Url.Link("GetOrders",
                    new
                    {
                        fields = resourceParameters.Fields,
                        pageNumber = resourceParameters.PageNumber + 1,
                        pageSize = resourceParameters.PageSize,
                        searchQuery = resourceParameters.SearchQuery,
                        orderBy = resourceParameters.OrderBy
                    });
                default:
                    return Url.Link("GetOrders",
                    new
                    {
                        fields = resourceParameters.Fields,
                        pageNumber = resourceParameters.PageNumber,
                        pageSize = resourceParameters.PageSize,
                        searchQuery = resourceParameters.SearchQuery,
                        orderBy = resourceParameters.OrderBy

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
                    new(Url.Link("GetOrder", new { id }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new(Url.Link("GetOrder", new { id, fields }),
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
            new(CreateOrdersResourceUri(
            resourceParameters,
            filters,
            ResourceUriType.Current),
            "self",
            "GET"));

            if (hasNext)
            {
                links.Add(
                    new(CreateOrdersResourceUri(
                        resourceParameters,
                        filters,
                        ResourceUriType.NextPage),
                        "nextPage",
                        "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new(CreateOrdersResourceUri(
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
