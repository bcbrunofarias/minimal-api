using Minimal.API.Auth.Password;
using Minimal.API.Database.EF;

namespace Minimal.API.Extensions;

public static class ConfigureDevelopmentEnvExtensions
{
    public static void ConfigureDevelopmentEnv(this WebApplication app)
    {
        if (app.Environment.IsProduction()) return;
        
        app.MapOpenApi();
    
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
        
        InMemoryDbSeeder.Seed(context, passwordService);
    }   
}