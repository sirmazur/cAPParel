using cAPParel.ConsoleClient.Models;

namespace cAPParel.ConsoleClient.Services.UserServices
{
    public interface IUserService
    {
        Task Authenticate(string username, string password);
        Task<UserDto> GetSelfFriendly();
        Task<UserFullDto> GetSelfFull();
        Task<LinkedResourceList<UserFullDto>?> GetUsersFullAsync(List<int>? ids, bool? includeLinks = false);
        Task Register(UserForClientCreation userToCreate);
        Task TopUpAccountAsync(int id, double amount);
    }
}