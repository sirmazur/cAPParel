using Blazored.LocalStorage;
using cAPParel.BlazorApp.Helpers;
using cAPParel.BlazorApp.HttpClients;
using cAPParel.BlazorApp.Models;
using cAPParel.BlazorApp.Services.ItemServices;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.BlazorApp.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly cAPParelAPIClient _client;
        private readonly ILocalStorageService _localStorageService;
        public UserService(cAPParelAPIClient client, ILocalStorageService localStorageService)
        {
            _client = client;
            _localStorageService = localStorageService;
        }

        public async Task<OperationResult> Authenticate(string username, string password)
        {
            var token = await _client.AuthenticateAsync(username, password);
            if (token == null)
                return new OperationResult(false, "Incorrect credentials.");
            var userData = new UserData(token);
            await _localStorageService.SetItemAsync("userdata", userData);
            return new OperationResult(true);
        }

        public async Task Register(UserForClientCreation userToCreate)
        {
            try
            { 
                var user = await _client.CreateResourceAsync<UserForClientCreation, UserDto>(userToCreate, "api/users");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task TopUpAccountAsync(int id, double amount)
        {
            var patchDocument = new JsonPatchDocument<UserForUpdateDto>();
            patchDocument.Replace(o => o.Saldo, amount);
            await _client.PatchResourceAsync<UserForUpdateDto>(patchDocument, $"api/users/{id}", "application/json");
        }

        public async Task<LinkedResourceList<UserFullDto>?> GetUsersFullByIdAsync(List<int>? ids, bool? includeLinks = false)
        {
            var route = "api/users";

            var queryString = QueryStringBuilder.BuildQueryString(
                ("ids", ids)
            );

            if (!string.IsNullOrEmpty(queryString))
            {
                route = $"{route}?{queryString}";
            }

            if (includeLinks is not null && includeLinks is true)
                return await _client.GetResourcesAsync<UserFullDto>(route, "application/vnd.capparel.user.full.hateoas+json");
            else
                return await _client.GetResourcesAsync<UserFullDto>(route, "application/vnd.capparel.user.full+json");


        }

        public async Task<LinkedResourceList<UserFullDto>?> GetUsersFullAsync(bool? includeLinks = false)
        {
            var route = "api/users";

            if (includeLinks is not null && includeLinks is true)
                return await _client.GetResourcesAsync<UserFullDto>(route, "application/vnd.capparel.user.full.hateoas+json");
            else
                return await _client.GetResourcesAsync<UserFullDto>(route, "application/vnd.capparel.user.full+json");


        }

        public async Task<UserDto> GetSelfFriendly()
        {
            return await _client.GetCurrentUserAsync<UserDto>("application/vnd.capparel.user.friendly+json", "/api/users/self");
        }
        public async Task<UserFullDto> GetSelfFull()
        {
            return await _client.GetCurrentUserAsync<UserFullDto>("application/vnd.capparel.user.full+json", "/api/users/self");
        }
    }
}
