using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Minimal.API.Models;

namespace Minimal.API.Auth.Token;

public class TokenService(IConfiguration configuration) : IToken
{
    private readonly IConfiguration _jwtSettings = configuration.GetSection("JwtSettings");
    
    public string CreateAccessToken(User user)
    {
        var expirationTime = _jwtSettings.GetValue<int>("ExpirationTimeInMinutes");
        return GenerateToken(user, expirationTime);
    }

    public (string, DateTime) CreateRefreshToken(User user)
    {
        var expirationTime = _jwtSettings.GetValue<int>("RefreshExpirationTimeInMinutes");
        var expiresAt = DateTime.UtcNow.AddMinutes(expirationTime);
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return (token, expiresAt);
    }

    public async Task<TokenValidationResult> ValidateTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return new TokenValidationResult(false);
        }

        var tokenParams = TokenHelpers.GetTokenValidationParameters(configuration);
        var tokenValidationResult = await new JwtSecurityTokenHandler().ValidateTokenAsync(token, tokenParams);

        if (!tokenValidationResult.IsValid)
        {
            return new TokenValidationResult(false);
        }
        
        var username = tokenValidationResult.Claims.FirstOrDefault(c => c.Key == ClaimTypes.NameIdentifier).Value as string;
        return new TokenValidationResult(true, username);
    }
    
    private JwtSecurityToken BuildJwt(User user, DateTime expiresAt)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.GetValue<string>("SecretKey") ?? string.Empty));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Name),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        claims.AddRange(user.UserRoles.Select(r => new Claim(ClaimTypes.Role, r.Role.Name)));
        claims.AddRange(user.UserClaims.Select(c => new Claim(c.Type, c.Value)));

        return new JwtSecurityToken(
            issuer: _jwtSettings.GetValue<string>("Issuer"),
            audience: _jwtSettings.GetValue<string>("Audience"),
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
        );
    }
    
    private string GenerateToken(User user, int expirationInMinutes)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(expirationInMinutes);
        var jwtSecurityToken = BuildJwt(user, expiresAt);
        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}