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

namespace cAPParel.BlazorApp.Services.VisitServices
{
    public class VisitService : IVisitService
    {
        private readonly cAPParelAPIClient _client;
        private readonly ILocalStorageService _localStorageService;
        public VisitService(cAPParelAPIClient client, ILocalStorageService localStorageService)
        {
            _client = client;
            _localStorageService = localStorageService;
        }


        public async Task<Visit> GetVisitAsync()
        {
            var route = "api/visits";
                return await _client.GetResourceAsync<Visit>(route);
        }
        public async Task AddVisitAsync()
        {
            var route = "api/visits";
            await _client.PostEmptyAsync(route);
        }
    }
}
