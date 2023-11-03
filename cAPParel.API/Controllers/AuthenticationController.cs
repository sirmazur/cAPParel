using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        [HttpPost("Authenticate")]
        public IActionResult Authenticate(UserParams param)
        {
            throw new NotImplementedException();
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
