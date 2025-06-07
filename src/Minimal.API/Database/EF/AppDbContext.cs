using Microsoft.EntityFrameworkCore;
using Minimal.API.Database.Mapping;
using Minimal.API.Models;

namespace Minimal.API.Database.EF;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<UserClaim> UserClaims => Set<UserClaim>();
    public DbSet<Todo> Todos => Set<Todo>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new TodoEntityMapping());
        modelBuilder.ApplyConfiguration(new RefreshTokenEntityMapping());
        modelBuilder.ApplyConfiguration(new UserEntityMapping());
        modelBuilder.ApplyConfiguration(new RoleEntityMapping());
        modelBuilder.ApplyConfiguration(new UserRoleEntityMapping());
        modelBuilder.ApplyConfiguration(new UserClaimEntityMapping());
    }
}