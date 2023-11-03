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
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public UserService(IMapper mapper, IConfiguration configuration, IBasicRepository<User> basicRepository, IUserRepository userRepository) : base(mapper, basicRepository)
        {
            _mapper = mapper;
            _configuration = configuration;
            _userRepository = userRepository;
        }
        public Task<(int userId, bool credentialsCorrect)> AuthenticateUser(UserParams userParams)
        {
            throw new NotImplementedException();
        }

        public Task<Role> AuthorizeUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckIfNameAvailable(string username)
        {
            throw new NotImplementedException();
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

        public Task TopUp(string username, double amount)
        {
            throw new NotImplementedException();
        }
    }
}
