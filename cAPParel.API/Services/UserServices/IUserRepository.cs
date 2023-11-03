namespace cAPParel.API.Services.UserServices
{
    public interface IUserRepository
    {
        Task<bool> IsNameAvailable(string name);
    }
}
