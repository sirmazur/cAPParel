using cAPParel.ConsoleClient.Models;

namespace cAPParel.ConsoleClient.Services.AuthenticationServices
{
    public interface IAuthenticationService
    {
        Task Authenticate(string username, string password);
        Task<UserDto> GetSelfFriendly();
        Task<UserFullDto> GetSelfFull();
    }
}