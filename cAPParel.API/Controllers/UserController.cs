﻿using cAPParel.API.Filters;
using cAPParel.API.Helpers;
using cAPParel.API.Models;
using cAPParel.API.Services.FieldsValidationServices;
using cAPParel.API.Services.UserServices;
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
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IFieldsValidationService _fieldsValidationService;
        public UserController(IUserService userService, ProblemDetailsFactory problemDetailsFactory, IFieldsValidationService fieldsValidationService)
        {
            _userService = userService;
            _problemDetailsFactory = problemDetailsFactory;
            _fieldsValidationService = fieldsValidationService;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(UserForClientCreation user)
        {
            try
            {
                var result = await _userService.CreateUser(user);
                return CreatedAtRoute("GetUser", result.Id, result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Produces("application/json",
          "application/vnd.capparel.hateoas+json",
          "application/vnd.capparel.user.full+json",
          "application/vnd.capparel.user.full.hateoas+json",
          "application/vnd.capparel.user.friendly+json",
          "application/vnd.capparel.user.friendly.hateoas+json")]
        [HttpGet(Name = "GetUsers")]
        [Authorize(Policy = "MustBeAdmin")]
        [HttpHead]
        public async Task<IActionResult> GetUsers([FromQuery] ResourceParameters resourceParameters, [FromHeader(Name = "Accept")] string? mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: 400,
                detail: $"Accept header media type value is not supported"));
            }
            if (!_fieldsValidationService.TypeHasProperties<UserFullDto>(resourceParameters.Fields))
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

                        var userLinks = CreateLinks(
                        users[usersIenumerator].Id,
                        resourceParameters.Fields);
                        usersIenumerator++;
                        userAsDictionary.Add("links", userLinks);

                        return userAsDictionary;
                    }).ToArray();
            }
            else
            {
                PagedList<UserDto>? users = null;
                try
                {
                    users = await _userService.GetAllAsync(filters, resourceParameters);
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
                            var userLinks = CreateLinks(
                            users[usersIenumerator].Id,
                            resourceParameters.Fields);
                            userAsDictionary.Add("links", userLinks);
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
                var CollectionResource = new
                {
                    value = shapedUsersToReturn
                };
                return Ok(CollectionResource);
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


        [Produces("application/json",
            "application/vnd.capparel.hateoas+json",
            "application/vnd.capparel.user.full+json",
            "application/vnd.capparel.user.full.hateoas+json",
            "application/vnd.capparel.user.friendly+json",
            "application/vnd.capparel.user.friendly.hateoas+json")]
        [Authorize(Policy = "MustBeAdmin")]
        [HttpGet("{userid}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(
            int userid,
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

            if (!_fieldsValidationService.TypeHasProperties<UserFullDto>(fields))
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
                links = CreateLinks(userid, fields);
            }

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix.Substring(
                0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) :
                parsedMediaType.SubTypeWithoutSuffix;

            if (primaryMediaType == "vnd.capparel.user.full")
            {
                var fullItem = await _userService.GetExtendedByIdWithEagerLoadingAsync(userid);
                var fullResourceToReturn = fullItem.ShapeDataForObject(fields) as IDictionary<string, object>;
                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }
                return Ok(fullResourceToReturn);
            }
            var item = await _userService.GetExtendedByIdWithEagerLoadingAsync(userid);

            var lightResourceToReturn = item.ShapeDataForObject(fields) as IDictionary<string, object>;
            if (includeLinks)
            {
                lightResourceToReturn.Add("links", links);
            }
            return Ok(lightResourceToReturn);
        }

        [Produces("application/json",
            "application/vnd.capparel.hateoas+json",
            "application/vnd.capparel.user.full+json",
            "application/vnd.capparel.user.full.hateoas+json",
            "application/vnd.capparel.user.friendly+json",
            "application/vnd.capparel.user.friendly.hateoas+json")]
        [Authorize(Policy = "MustBeLoggedIn")]
        [HttpGet("self",Name = "GetSelf")]
        public async Task<IActionResult> GetSelf(
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

            if (!_fieldsValidationService.TypeHasProperties<UserFullDto>(fields))
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
                links = CreateLinks(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value), fields);
            }

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix.Substring(
                0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) :
                parsedMediaType.SubTypeWithoutSuffix;

            if (primaryMediaType == "vnd.capparel.user.full")
            {
                var fullItem = await _userService.GetExtendedByIdWithEagerLoadingAsync(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
                var fullResourceToReturn = fullItem.ShapeDataForObject(fields) as IDictionary<string, object>;
                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }
                return Ok(fullResourceToReturn);
            }
            var item = await _userService.GetExtendedByIdWithEagerLoadingAsync(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));

            var lightResourceToReturn = item.ShapeDataForObject(fields) as IDictionary<string, object>;
            if (includeLinks)
            {
                lightResourceToReturn.Add("links", links);
            }
            return Ok(lightResourceToReturn);
        }

        [Authorize(Policy = "MustBeAdmin")]
        [HttpDelete("{todeleteid}", Name = "DeleteUser")]
        public async Task<ActionResult> DeleteCategory(int todeleteid)
        {
            var operationResult = await _userService.DeleteByIdAsync(todeleteid);
            if (operationResult.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }

        [Authorize(Policy = "MustBeAdmin")]
        [HttpPut("{toupdateid}", Name = "UpdateUser")]
        public async Task<IActionResult> UpdateUser(int toupdateid, UserForUpdateDto item)
        {
            var operationResult = await _userService.UpdateAsync(toupdateid, item);
            if (operationResult.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }

        [Authorize(Policy = "MustBeAdmin")]
        [HttpPatch("{toupdateid}", Name = "PartialUpdateUser")]
        public async Task<IActionResult> PartialUpdateUser(int toupdateid, JsonPatchDocument<UserForUpdateDto> patchDocument)
        {
            var operationResult = await _userService.PartialUpdateAsync(toupdateid, patchDocument);
            if (operationResult.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }

        [Authorize(Policy = "MustBeAdmin")]
        [HttpPost("{userid}/balance/{amount}")]
        public async Task<UserDto> TopUp(int userid, double amount)
        {
            try
            {
                var userToReturn = await _userService.TopUp(userid, amount);
                return userToReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpOptions()]
        public IActionResult GetUsersOptions()
        {
            Response.Headers.Add("Allow", "GET,HEAD,POST,PUT,PATCH,DELETE,OPTIONS");
            return Ok();
        }
    }
}
