using cAPParel.API.Entities;

namespace cAPParel.API.Services.UserServices
{
    public interface IUserRepository
    {
        Task<bool> IsNameAvailable(string name);
        Task<User?> GetUserByName(string name);
        Task<User?> GetUserById(int id);
    }
}
