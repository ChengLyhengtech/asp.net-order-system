using aps.net_order_system.Interface;
using aps.net_order_system.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace aps.net_order_system.Services
{

   
        public class TokenService : ITokenService
        {
            private readonly IConfiguration _config;
            private readonly SymmetricSecurityKey _key;
            private readonly UserManager<UserModel> _userManager;

            public TokenService(IConfiguration config, UserManager<UserModel> userManager)
            {
                _config = config;
                _userManager = userManager;
                // The Key from appsettings.json
                _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            }

        public async Task<string> CreateToken(UserModel user)
        {
            // 1. Define User Claims (Using standard JWT mapping keys)
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.GivenName, user.UserName),
        new Claim(JwtRegisteredClaimNames.NameId, user.Id)
    };

            // 2. Add Roles using the literal short string "role"
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role)); // <-- FIX HERE: Match your Program.cs RoleClaimType
            }

            // 3. Create Signing Credentials
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            // 4. Create Token Descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme),
                Expires = DateTime.UtcNow.AddDays(7), // Best practice: Use UtcNow
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
