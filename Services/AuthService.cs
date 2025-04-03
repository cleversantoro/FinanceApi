using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using FinanceApi.Models;

namespace FinanceApi.Services;

public class AuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GerarToken(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email!)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
