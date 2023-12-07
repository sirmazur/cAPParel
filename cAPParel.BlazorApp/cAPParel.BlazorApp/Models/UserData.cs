using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace cAPParel.BlazorApp.Models
{
    public class UserData
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public Role Role { get; set; }
        public string Token { get; set; }
        public List<PieceDto> Cart { get; set; } = new List<PieceDto>();
        public DateTime Expires { get; set; } = DateTime.UtcNow.AddDays(30);

        public UserData(string token) 
        {
            var jwttoken = new JwtSecurityToken(token);
            Token = token;
            Username = jwttoken.Claims.First(c => c.Type =="username").Value;
            Id = int.Parse(jwttoken.Claims.First(c => c.Type == "sub").Value);           
            Role = (Role)Enum.Parse(typeof(Role),jwttoken.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        }
    }
}
