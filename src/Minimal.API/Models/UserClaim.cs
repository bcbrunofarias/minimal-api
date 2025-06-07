namespace Minimal.API.Models;

public class UserClaim
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string Type { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;

    public static UserClaim Create(Guid userId, string type, string value)
    {
        return new UserClaim()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = type,
            Value = value,
        };
    }
    
    private UserClaim() { }
}