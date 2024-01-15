using cAPParel.BlazorApp.Helpers;
using cAPParel.BlazorApp.Models;

namespace cAPParel.BlazorApp.Services.UserServices
{
    public interface IUserService
    {
        Task<OperationResult> Authenticate(string username, string password);
        Task<UserDto> GetSelfFriendly();
        Task<UserFullDto> GetSelfFull();
        Task<LinkedResourceList<UserFullDto>?> GetUsersFullByIdAsync(List<int>? ids, bool? includeLinks = false);
        Task<LinkedResourceList<UserFullDto>?> GetUsersFullAsync(bool? includeLinks = false);
        Task Register(UserForClientCreation userToCreate);
        Task TopUpAccountAsync(int id, double amount);
    }
}