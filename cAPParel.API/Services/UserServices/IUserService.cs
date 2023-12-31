﻿using cAPParel.API.Controllers;
using cAPParel.API.Entities;
using cAPParel.API.Models;
using cAPParel.API.Services.Basic;

namespace cAPParel.API.Services.UserServices
{
    public interface IUserService : IBasicService<UserDto, User, UserFullDto, UserForCreationDto, UserForUpdateDto>
    {
        public Task<UserFullDto> AuthenticateUser(UserParams userParams);  
        public Task<Role> AuthorizeUser(int userId);
        public Task<UserDto> CreateUser(UserForClientCreation user);
        Task<UserDto> TopUp(int userid, double amount);
        public string GenerateToken(UserFullDto user);
    }
}
