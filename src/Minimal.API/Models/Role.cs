namespace Minimal.API.Models;

public class Role
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } =  string.Empty;
    public ICollection<UserRole> UserRoles { get; private set; }  = [];

    public static Role Create(string name)
    {
        return new Role()
        {
            Id = Guid.NewGuid(),
            Name = name
        };
    }
    
    private Role() {}
}