using Minimal.API.Enums;

namespace Minimal.API.Extensions;

public static class ConfigureAuthorizationExtensions
{
    public static void ConfigureAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            foreach (var policyClaim in Enum.GetValues<TodoPolicies>())
            {
                options.AddPolicy(policyClaim.ToString(), policy => 
                    policy.RequireClaim("permission", policyClaim.ToString()));
            }
        });
    }
}