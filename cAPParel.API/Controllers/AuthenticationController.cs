using cAPParel.API.Models;
using cAPParel.API.Services.UserServices;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Controllers
{
    
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost(Name = "Authenticate")]
        public async Task<ActionResult<string>> Authenticate(UserParams param)
        {
            UserFullDto result;
            try
            {
                result = await _userService.AuthenticateUser(param);
            }
            catch(Exception e)
            {
                return Unauthorized(e.Message);
            }
            
            var token = _userService.GenerateToken(result);
            return Ok(token);

        }
    }


    public class UserParams
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
