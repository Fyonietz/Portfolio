using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Backend.Models;
namespace Backend.Services
{
    public interface IJWTService
    {
        string GenerateToken(User user);
    }

    public class JWTService : IJWTService
    {
        public string GenerateToken(User user)
        {
            var claims = new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };
        

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.Value["JWT:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Env.Value["JWT:Issuer"],
            audience: Env.Value["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
}
