using cAPParel.API.Filters;
using cAPParel.API.Helpers;
using cAPParel.API.Models;
using cAPParel.API.Services.FieldsValidationServices;
using cAPParel.API.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Dynamic;

namespace cAPParel.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IFieldsValidationService _fieldsValidationService;
        public UsersController(IUserService userService, ProblemDetailsFactory problemDetailsFactory, IFieldsValidationService fieldsValidationService)
        {
            _userService = userService;
            _problemDetailsFactory = problemDetailsFactory;
            _fieldsValidationService = fieldsValidationService;
        }


        [Produces("application/json",
          "application/vnd.capparel.hateoas+json",
          "application/vnd.capparel.user.full+json",
          "application/vnd.capparel.user.full.hateoas+json",
          "application/vnd.capparel.user.friendly+json",
          "application/vnd.capparel.user.friendly.hateoas+json")]
        [HttpGet(Name = "GetUsers")]
        [Authorize(Roles = "Admin")]
        [HttpHead]
        public async Task<IActionResult> GetUsers([FromQuery] ResourceParameters resourceParameters, [FromHeader(Name = "Accept")] string? mediaType)
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

            IEnumerable<ExpandoObject> shapedUsers = new List<ExpandoObject>();
            IEnumerable<IDictionary<string, object?>>? shapedUsersToReturn = new List<IDictionary<string, object?>>();
            if (primaryMediaType == "vnd.capparel.user.full")
            {
                PagedList<UserFullDto>? categories = null;
                try
                {
                    categories = await _userService.GetFullAllAsync(filters, resourceParameters);
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
                shapedUsers = categories.ShapeData(resourceParameters.Fields);
                int categoriesIenumerator = 0;
                shapedUsersToReturn = shapedUsers
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
                PagedList<UserFullDto>? users = null;
                try
                {
                    users = await _userService.GetFullAllAsync(filters, resourceParameters);
                }
                catch (Exception ex)
                {
                    return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                        statusCode: 400,
                        detail: ex.Message));
                }
                var paginationMetadata = new
                {
                    totalCount = users.TotalCount,
                    pageSize = users.PageSize,
                    currentPage = users.CurrentPage,
                    totalPages = users.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                links = CreateLinksForCollection(
                resourceParameters,
                filters,
                users.HasNext,
                users.HasPrevious
                );
                shapedUsers = users.ShapeData(resourceParameters.Fields);
                int usersIenumerator = 0;
                shapedUsersToReturn = shapedUsers
                    .Select(user =>
                    {
                        var userAsDictionary = user as IDictionary<string, object?>;
                        if (includeLinks)
                        {
                            var categoryLinks = CreateLinks(
                            users[usersIenumerator].Id,
                            resourceParameters.Fields);
                            userAsDictionary.Add("links", categoryLinks);
                        }
                        usersIenumerator++;

                        return userAsDictionary;
                    }).ToArray();

            }
            if (shapedUsersToReturn.Count() == 0)
            {
                return NotFound();
            }

            if (includeLinks)
            {
                var linkedCollectionResource = new
                {
                    value = shapedUsersToReturn,
                    links
                };
                return Ok(linkedCollectionResource);
            }
            else
            {
                return Ok(shapedUsersToReturn);
            }

        }

        private string? CreateUsersResourceUri(
          ResourceParameters resourceParameters,
          List<IFilter> filters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetUsers",
                     new
                     {
                         fields = resourceParameters.Fields,
                         pageNumber = resourceParameters.PageNumber - 1,
                         pageSize = resourceParameters.PageSize,
                         searchQuery = resourceParameters.SearchQuery,
                         orderBy = resourceParameters.OrderBy
                     });
                case ResourceUriType.NextPage:
                    return Url.Link("GetUsers",
                    new
                    {
                        fields = resourceParameters.Fields,
                        pageNumber = resourceParameters.PageNumber + 1,
                        pageSize = resourceParameters.PageSize,
                        searchQuery = resourceParameters.SearchQuery,
                        orderBy = resourceParameters.OrderBy
                    });
                default:
                    return Url.Link("GetUsers",
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
            int userid,
            string? fields)
        {
            var links = new List<LinkDto>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new(Url.Link("GetUser", new { userid }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new(Url.Link("GetUser", new { userid, fields }),
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
            new(CreateUsersResourceUri(
            resourceParameters,
            filters,
            ResourceUriType.Current),
            "self",
            "GET"));

            if (hasNext)
            {
                links.Add(
                    new(CreateUsersResourceUri(
                        resourceParameters,
                        filters,
                        ResourceUriType.NextPage),
                        "nextPage",
                        "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new(CreateUsersResourceUri(
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
