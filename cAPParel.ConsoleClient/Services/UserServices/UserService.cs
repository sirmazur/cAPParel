using cAPParel.ConsoleClient.Helpers;
using cAPParel.ConsoleClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.ConsoleClient.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly cAPParelAPIClient _client;
        private CurrentUserData _currentUserData = CurrentUserData.Instance;
        public UserService(cAPParelAPIClient client)
        {
            _client = client ??
                throw new ArgumentNullException(nameof(client));
        }

        public async Task Authenticate(string username, string password)
        {
            var token = await _client.AuthenticateAsync(username, password);
            if (token != null)
            {
                _currentUserData.SetToken(token);
            }


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
