using Minimal.API.Auth;
using Minimal.API.Auth.Password;
using Minimal.API.Models;

namespace Minimal.API.Database.EF;

public static class InMemoryDbSeeder 
{
    public static void Seed(AppDbContext context, IPasswordService passwordService)
    {
        var adminRole = Role.Create("admin");
        var commonRole = Role.Create("common");
        
        var adminUser = User.Create("Bruno", "admin@dotnet.com", passwordService.HashPassword("admin"));
        var commonUser = User.Create("César", "common@dotnet.com", passwordService.HashPassword("common"));
        
        var adminUserRole = UserRole.Create(adminUser.Id, adminRole.Id);
        var commonUserRole = UserRole.Create(commonUser.Id, commonRole.Id);

        var adminReadClaim = UserClaim.Create(adminUser.Id, "permission", "CanRead");
        var adminWriteClaim = UserClaim.Create(adminUser.Id, "permission", "CanWrite");
        var adminDeleteClaim = UserClaim.Create(adminUser.Id, "permission", "CanDelete");
        var commonReadClaim = UserClaim.Create(commonUser.Id, "permission", "CanRead");
        
        adminUser.AddUserClaim(adminReadClaim, adminDeleteClaim, adminWriteClaim);
        commonUser.AddUserClaim(commonReadClaim);
        
        context.Roles.AddRange(adminRole, commonRole);
        context.Users.AddRange(adminUser, commonUser);
        context.UserRoles.AddRange(adminUserRole, commonUserRole);
        context.UserClaims.AddRange(adminWriteClaim, adminDeleteClaim, adminReadClaim, commonReadClaim);
        
        context.SaveChanges();
    }
}