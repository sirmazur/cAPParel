using cAPParel.BlazorApp.Models;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Routing;
using System.Xml.XPath;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using cAPParel.BlazorApp.Helpers;
using Blazored.LocalStorage;

namespace cAPParel.BlazorApp.HttpClients
{
    public class cAPParelAPIClient
    {

        private HttpClient _client { get; }
        private readonly JsonSerializerOptionsWrapper _jsonSerializerOptionsWrapper;
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly ILocalStorageService _localStorageService;

        public cAPParelAPIClient(HttpClient client,
            JsonSerializerOptionsWrapper jsonSerializerOptionsWrapper,
            ProtectedSessionStorage sessionStorage,
            ILocalStorageService localStorageService)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://localhost:7003");
            _client.Timeout = new TimeSpan(0, 0, 60);
            _jsonSerializerOptionsWrapper = jsonSerializerOptionsWrapper;
            _sessionStorage=sessionStorage;
            _localStorageService = localStorageService;
        }

        public async Task DeleteResourceAsync(string route, string acceptHeader = "application/json")
        {
            var request = new HttpRequestMessage(
            HttpMethod.Delete,
            route);
            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(acceptHeader));
            var data = await _localStorageService.GetItemAsync<UserData>("userdata");
            if (data is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", data.Token);
            }

            var response = await _client.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode.HasValue)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();

                    throw new Exception(errorMessage, ex);
                }
                else
                {
                    throw new Exception(ex.Message, ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TDto> CreateResourceAsync<TCreationDto, TDto>(TCreationDto itemToCreate, string route, string acceptHeader = "application/json")
        {
            var request = new HttpRequestMessage(
            HttpMethod.Post,
            route);
            var serialized = JsonSerializer.Serialize(itemToCreate);
            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(acceptHeader));
            request.Content = new StringContent(
            JsonSerializer.Serialize(itemToCreate),
            Encoding.UTF8,
            "application/json");
            var data = await _localStorageService.GetItemAsync<UserData>("userdata");
            if (data is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", data.Token);
            }
            var response = await _client.SendAsync(request);
            try 
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode.HasValue)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();

                    throw new Exception(errorMessage, ex);
                }
                else
                {
                    throw new Exception(ex.Message, ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            var content = await response.Content.ReadAsStringAsync();
            var createdItemDto = JsonSerializer.Deserialize<TDto>(content, _jsonSerializerOptionsWrapper.Options);

            return createdItemDto;
        }

        public async Task<T> GetResourceAsync<T>(string route, string acceptHeader = "application/json")
        {
            var request = new HttpRequestMessage(
            HttpMethod.Get,
            route);
            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(acceptHeader));
            var data = await _localStorageService.GetItemAsync<UserData>("userdata");
            if (data is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", data.Token);
            }
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
            return JsonSerializer.Deserialize<T>(
                content, _jsonSerializerOptionsWrapper.Options);
        }

        public async Task<LinkedResourceList<T>?> GetResourcesAsync<T>(string route, string acceptHeader = "application/json")
        {
            var request = new HttpRequestMessage(
            HttpMethod.Get,
            route);
            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(acceptHeader));
            var data = await _localStorageService.GetItemAsync<UserData>("userdata");
            if(data is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", data.Token);
            }
            var response = await _client.SendAsync(request);
            if(!response.IsSuccessStatusCode)
            {
                return new LinkedResourceList<T>();
            }
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
            // var resultValue = JsonSerializer.Deserialize<IEnumerable<T>>(content, _jsonSerializerOptionsWrapper.Options);
            //return new LinkedResourceList<T>
            //{
            //    LinkedResources = resultValue
            //};

            return JsonSerializer.Deserialize<LinkedResourceList<T>>(
                content, _jsonSerializerOptionsWrapper.Options);
        }

        public async Task<string?> AuthenticateAsync(string username, string password)
        {
            var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/authentication");

            request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new StringContent(
            JsonSerializer.Serialize(new UserParams
            {
                Username = username,
                Password = password
            }),
            Encoding.UTF8,
            "application/json");

            request.Content = content;
            var response = await _client.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex) { return null; }
            var result = await response.Content.ReadAsStringAsync();
            var resultToReturn = JsonSerializer.Deserialize<string>(result, _jsonSerializerOptionsWrapper.Options);
            return resultToReturn;
        }

        public async Task<T> GetCurrentUserAsync<T>(string mediaType, string route)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, route);

            var data = await _localStorageService.GetItemAsync<UserData>("userdata");
            if (data is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", data.Token);
            }
            else
            {
                throw new Exception("User is not logged in.");
            }
            request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue(mediaType));

            var response = await _client.SendAsync(request);

                response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var currentUser = JsonSerializer.Deserialize<T>(content, _jsonSerializerOptionsWrapper.Options);

            return currentUser;
        }

        public async Task PatchResourceAsync<TUpdateDto>(Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<TUpdateDto> patchDoc, string route, string mediaType) where TUpdateDto : class
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, route);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

            var data = await _localStorageService.GetItemAsync<UserData>("userdata");
            if (data is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", data.Token);
            }

            // Serialize the JsonPatchDocument to a JSON string
            var serializedPatch = Newtonsoft.Json.JsonConvert.SerializeObject(patchDoc);
            request.Content = new StringContent(serializedPatch);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(("application/json-patch+json"));
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

        }

        public async Task DownloadFileAsync(string route, string destinationFilePath)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, route);

            var data = await _localStorageService.GetItemAsync<UserData>("userdata");
            if (data is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", data.Token);
            }
            var response = await _client.GetAsync(route);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode.HasValue)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();

                    throw new Exception(errorMessage, ex);
                }
                else
                {
                    throw new Exception(ex.Message, ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                {
                    using (var fileStream = new FileStream(Path.Combine(destinationFilePath,response.Content.Headers.ContentDisposition.FileNameStar), FileMode.Create, FileAccess.Write))
                    {
                        await contentStream.CopyToAsync(fileStream);
                    }
                }


        }


    }
}
