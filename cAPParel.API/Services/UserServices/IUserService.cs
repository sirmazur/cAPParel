using cAPParel.API.Controllers;
using cAPParel.API.Entities;
using cAPParel.API.Models;
using cAPParel.API.Services.Basic;

namespace cAPParel.API.Services.UserServices
{
    public interface IUserService : IBasicService<UserDto, User, UserFullDto, UserForCreationDto, UserForUpdateDto>
    {
        public Task<(int userId, bool credentialsCorrect)> AuthenticateUser(UserParams userParams);  
        public Task<Role> AuthorizeUser(int userId);
        public Task<UserDto> CreateUser(UserForClientCreation user);
        public Task TopUp(string username, double amount);
        public string GenerateToken(int userId);
    }
}
