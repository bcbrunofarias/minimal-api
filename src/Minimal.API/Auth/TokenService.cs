using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Minimal.API.Auth;

public class TokenService(IConfiguration configuration) : IToken
{
    private readonly IConfiguration _jwtSettings = configuration.GetSection("JwtSettings");
    
    public string CreateAccessToken(string username)
    {
        var expirationTime = _jwtSettings.GetValue<int>("ExpirationTimeInMinutes");
        var token = TokenSettings(username, expirationTime);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateRefreshToken(string username)
    {
        var expirationTime = _jwtSettings.GetValue<int>("RefreshExpirationTimeInMinutes");
        var token = TokenSettings(username, expirationTime);
        return new JwtSecurityTokenHandler().WriteToken(token);
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

    private JwtSecurityToken TokenSettings(string username, int expirationTime)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.GetValue<string>("SecretKey") ?? string.Empty));
        var claims = new List<Claim>([
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ]);
        
        return new JwtSecurityToken
        (
            issuer: _jwtSettings.GetValue<string>("Issuer"),
            audience: _jwtSettings.GetValue<string>("Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationTime),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
        );
    }
}