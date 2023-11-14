using cAPParel.ConsoleClient.Models;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cAPParel.ConsoleClient.Helpers;
using System.Text.Json;

namespace cAPParel.ConsoleClient
{
    public class cAPParelAPIClient
    {

        private HttpClient _client { get; }
        private readonly JsonSerializerOptionsWrapper _jsonSerializerOptionsWrapper;

        public cAPParelAPIClient(HttpClient client,
            JsonSerializerOptionsWrapper jsonSerializerOptionsWrapper)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://localhost:7003");
            _client.Timeout = new TimeSpan(0, 0, 30);
            _jsonSerializerOptionsWrapper = jsonSerializerOptionsWrapper;
        }


        public async Task<LinkedResourceList<T>?> GetItemsAsync<T>(string acceptHeader = "application/json")
        {
            var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/items");
            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(acceptHeader));
            
            var response = await _client.SendAsync(request);       
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
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async Task<T> GetCurrentUserAsync<T>(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/users/self");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var currentUser = JsonSerializer.Deserialize<T>(content, _jsonSerializerOptionsWrapper.Options);

            return currentUser;
        }

    }
}
