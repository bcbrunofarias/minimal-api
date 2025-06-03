namespace Minimal.API.Auth;

public interface IToken
{
    string CreateAccessToken(string username);
    string CreateRefreshToken(string username);
    Task<TokenValidationResult> ValidateTokenAsync(string token);
}