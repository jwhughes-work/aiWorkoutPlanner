using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace aiWorkoutPlanner.Framework
{
    public class NativeLogin
    {
        public class LoginDto
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public async Task<string> LoginUser(LoginDto loginDto)
        {
            if (loginDto.Username == "admin" && loginDto.Password == "password")
            {
                var token = GenerateJwtToken(loginDto.Username);
                return token;
            }
            
            return "";
        }

        private string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("2600611A-9AAF-4A52-8FE9-72A6FD8CE0F9")); // Replace with your secret key this need to be long
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, "loginclaim")
            };


            var token = new JwtSecurityToken(
                issuer: "com.jwhsolutions.fitpa", // Replace with your issuer
                audience: "clientapp", // Replace with your audience
                claims: claims,
                expires: DateTime.Now.AddDays(30), // Token expiration time
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
