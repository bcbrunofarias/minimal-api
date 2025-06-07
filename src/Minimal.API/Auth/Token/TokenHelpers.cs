using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Minimal.API.Auth.Token;

public class TokenHelpers
{
    public static TokenValidationParameters GetTokenValidationParameters(IConfiguration configuration)
    {
        var tokenKey = Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"] ?? string.Empty);

        return new TokenValidationParameters()
        {
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = configuration["JwtSettings:Audience"],
            ValidIssuer = configuration["JwtSettings:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
        };
    }
}