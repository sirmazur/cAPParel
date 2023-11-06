using AutoMapper;
using cAPParel.API.Controllers;
using cAPParel.API.Entities;
using cAPParel.API.Models;
using cAPParel.API.Services.Basic;
using System.Data;

namespace cAPParel.API.Services.UserServices
{
    public class UserService : BasicService<UserDto, User, UserFullDto, UserForCreationDto, UserForUpdateDto>, IUserService
    {

        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public UserService(IMapper mapper, IConfiguration configuration, IBasicRepository<User> basicRepository, IUserRepository userRepository) : base(mapper, basicRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }
        public async Task<(int userId, bool credentialsCorrect)> AuthenticateUser(UserParams userParams)
        {
            var account = await _userRepository.GetUserByName(userParams.Username);
            if(account == null || account.Password!=userParams.Password)
            {
                return (-1, false);
            }
            else
            {
                return (account.Id, true);
            }

        }

        public async Task<Role> AuthorizeUser(int userId)
        {
            var user = await _basicRepository.GetByIdAsync(userId);
            return user.Role;
        }


        public async Task<UserDto> CreateUser(UserForClientCreation user)
        {
            var nameAvailable = await _userRepository.IsNameAvailable(user.Username);
            if(!nameAvailable)
            {
                throw new Exception("Username already taken");
            }

            var userToCreate = new UserForCreationDto
            {
                Username = user.Username,
                Password = user.Password,
                Role = Role.User
            };

            if (user.AdminCode == _configuration["AdminCodes:AdminRegisterCode"])
            {
                userToCreate.Role = Role.Admin;
            }

            var createdUser = _mapper.Map<User>(userToCreate);

            await _basicRepository.AddAsync(createdUser);
            
            return _mapper.Map<UserDto>(createdUser);
        }

        public async Task TopUp(string username, double amount)
        {
            var user = await _userRepository.GetUserByName(username);
            if(user is null)
            {
                throw new Exception("User not found");
            }
            else
            {
                user.Saldo += amount;
                await _basicRepository.SaveChangesAsync();
            }
            
        }
    }
}
