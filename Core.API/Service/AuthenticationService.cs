using Core.API.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.API.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        public AuthenticationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateJwtToken(IdentityUser user, bool RememberME)
        {
            var jwtSetting = _configuration.GetSection("jwt");
            var key = Encoding.ASCII.GetBytes(jwtSetting["key"]);

            DateTime date;

            if(!RememberME)
            {
                date = DateTime.UtcNow.AddMonths(1);
            }
            else
            {
                date = DateTime.UtcNow.AddDays(1);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = date,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtSetting["Issuer"],
                Audience = jwtSetting["Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
