using cAPParel.API.Services.UserServices;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Authenticate")]
        public async Task<ActionResult<string>> Authenticate(UserParams param)
        {
            var result = await _userService.AuthenticateUser(param);
            if(result.credentialsCorrect)
            {
                var token = _userService.GenerateToken(result.userId);
                return Ok(token);
            }
            else
            {
                return Unauthorized();
            }
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
