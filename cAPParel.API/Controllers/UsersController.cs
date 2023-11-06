using cAPParel.API.Models;
using cAPParel.API.Services.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace cAPParel.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(UserForClientCreation user)
        {
            try
            {
                var result = await _userService.CreateUser(user);
                return Ok(result);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }                      
        }
    }
}
