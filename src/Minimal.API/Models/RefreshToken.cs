namespace Minimal.API.Models;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public string Username { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; private init; }
    public bool IsRevoked { get; private set; }
    public string? ReplacedByToken { get; private set; }
    
    public static RefreshToken Create(string token,  string username, DateTime expiresAt)
    {
        return new RefreshToken()
        {
            Id = Guid.NewGuid(),
            Token = token,
            Username = username,
            ExpiresAt = expiresAt,
        };
    }

    public bool IsExpired()
    {
        return ExpiresAt < DateTime.UtcNow;
    }

    public void Revoke(string newRefreshToken)
    {
        IsRevoked = true;
        ReplacedByToken = newRefreshToken;
    }
    
    private RefreshToken() { }
}