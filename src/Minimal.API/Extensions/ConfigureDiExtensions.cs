using Microsoft.EntityFrameworkCore;
using Minimal.API.Auth.Password;
using Minimal.API.Auth.Token;
using Minimal.API.Database.EF;

namespace Minimal.API.Extensions;

public static class ConfigureDiExtensions
{
    public static void ConfigureDi(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IToken, TokenService>();
        builder.Services.AddSingleton<IPasswordService, PasswordService>();
        builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TodosDb"));
    }
}