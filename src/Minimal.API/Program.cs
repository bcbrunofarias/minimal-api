using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Minimal.API.Auth;
using Minimal.API.Enums;
using Minimal.API.Models;
using Minimal.API.Requests;
using Minimal.API.Database;
using Minimal.API.Responses;
using LoginRequest = Minimal.API.Requests.LoginRequest;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    var configuration = builder.Configuration;
    options.TokenValidationParameters = TokenHelpers.GetTokenValidationParameters(configuration);
});

builder.Services.AddSingleton<IToken, TokenService>();
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TodosDb"));
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/health", () => "Healthy");

app.MapPost("/login", (IToken tokenService, LoginRequest request) =>
{
    if (!request.Username.Equals("ADMIN", StringComparison.InvariantCultureIgnoreCase) || request.Password != "123")
    {
        return Results.Unauthorized();
    }
    
    var accessToken = tokenService.CreateAccessToken(request.Username);
    var refreshToken = tokenService.CreateRefreshToken(request.Username);
    
    return Results.Ok(new LoginResponse(accessToken, refreshToken));
});

app.MapPost("/refresh-token", async (IToken tokenService, RefreshTokenRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.RefreshToken))
    {
        return Results.BadRequest();
    }
    
    var tokenValidationResult = await tokenService.ValidateTokenAsync(request.RefreshToken);
    if (!tokenValidationResult.IsValid || string.IsNullOrWhiteSpace(tokenValidationResult.Username))
    {
        return Results.Unauthorized();
    }
    
    var accessToken = tokenService.CreateAccessToken(tokenValidationResult.Username);
    var refreshToken = tokenService.CreateRefreshToken(tokenValidationResult.Username);
    
    return Results.Ok(new RefreshTokenResponse(accessToken, refreshToken));
});

app.MapGet("/todos", async (AppDbContext dbContext) =>
{
    var todos = await dbContext.Todos.ToListAsync();
    return Results.Ok(todos);
});

app.MapGet("/todos/{id:guid}", async (Guid id, AppDbContext dbContext) =>
{
    var selectedTodo = await dbContext.Todos.FirstOrDefaultAsync(todo => todo.Id == id);
    return selectedTodo is null ? Results.NotFound() : Results.Ok(selectedTodo);
});

app.MapPost("/todos", (AddTodoRequest request, AppDbContext dbContext) =>
{
    var todo = Todo.Create(request.Title, request.Description);
    dbContext.Add(todo);
    dbContext.SaveChanges();
    return Results.Created($"/todos/{todo.Id}", todo);
});

app.MapPut("/todos/{id:guid}", async (Guid id, UpdateTodoRequest request, AppDbContext dbContext) =>
{
    var selectedTodo = await dbContext.Todos.FirstOrDefaultAsync(todo => todo.Id == id);
    if (selectedTodo is null)
    {
        return Results.NotFound();
    }
    
    selectedTodo.Update(request.Title, request.Description);
    
    dbContext.Update(selectedTodo);
    dbContext.SaveChanges();
    
    return Results.Ok(selectedTodo);
});

app.MapPatch("/todos/{id:guid}", async (Guid id, AppDbContext dbContext) =>
{
    var selectedTodo = await dbContext.Todos.FirstOrDefaultAsync(todo => todo.Id == id);
    if (selectedTodo is null)
    {
        return Results.NotFound();
    }

    if (selectedTodo.Status is TodoStatus.Done or TodoStatus.Deleted)
    {
        return Results.BadRequest();
    }
    
    selectedTodo.MarkAsDone();
    
    dbContext.Update(selectedTodo);
    dbContext.SaveChanges();
    
    return Results.Ok(selectedTodo);
});

app.MapDelete("/todos/{id:guid}", async (Guid id, AppDbContext dbContext) =>
{
    var selectedTodo = await dbContext.Todos.FirstOrDefaultAsync(todo => todo.Id == id);
    if (selectedTodo is null)
    {
        return Results.NotFound();
    }
    
    selectedTodo.Delete();
    
    dbContext.Remove(selectedTodo);
    dbContext.SaveChanges();
    
    return Results.NoContent();
}).RequireAuthorization();

app.UseAuthentication();
app.UseAuthorization();

app.Run();