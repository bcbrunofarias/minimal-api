namespace Minimal.API.Models;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; }  = string.Empty;
    public string PasswordHash { get; private set; } =  string.Empty;
    public List<UserRole> UserRoles { get; private init; } = [];
    public List<UserClaim> UserClaims { get; private init; } =  [];
    
    public static User Create(string name, string email, string password)
    {
        return new User()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            PasswordHash = password,
            UserRoles = [],
            UserClaims = []
        };
    }

    public void AddUserRoles(params UserRole[] userRoles)
    {
        UserRoles.AddRange(userRoles);
    }

    public void AddUserClaim(params UserClaim[] userClaims)
    {
        UserClaims.AddRange(userClaims);
    }
    
    private User() {}
}