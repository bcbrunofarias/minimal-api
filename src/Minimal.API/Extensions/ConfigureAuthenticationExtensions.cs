using Microsoft.AspNetCore.Authentication.JwtBearer;
using Minimal.API.Auth.Token;

namespace Minimal.API.Extensions;

public static class ConfigureAuthenticationExtensions
{
    public static void ConfigureAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => {
            var configuration = builder.Configuration;
            options.TokenValidationParameters = TokenHelpers.GetTokenValidationParameters(configuration);
        });
    }
}