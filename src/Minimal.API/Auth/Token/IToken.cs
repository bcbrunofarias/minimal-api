using Minimal.API.Models;

namespace Minimal.API.Auth.Token;

public interface IToken
{
    string CreateAccessToken(User user);
    (string, DateTime) CreateRefreshToken(User user);
    Task<TokenValidationResult> ValidateTokenAsync(string token);
}