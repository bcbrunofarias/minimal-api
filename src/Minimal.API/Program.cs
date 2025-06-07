using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Minimal.API.Auth.Password;
using Minimal.API.Auth.Token;
using Minimal.API.Enums;
using Minimal.API.Models;
using Minimal.API.Requests;
using Minimal.API.Database.EF;
using Minimal.API.Extensions;
using Minimal.API.Responses;
using LoginRequest = Minimal.API.Requests.LoginRequest;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureDi();
builder.ConfigureAuthentication();
builder.ConfigureAuthorization();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

app.ConfigureDevelopmentEnv();

app.MapGet("/health", () => "Healthy");

app.MapGet("/auth/me", (AppDbContext context, ClaimsPrincipal authenticatedUser) =>
{
    var userEmail = authenticatedUser.FindFirst(ClaimTypes.Email)?.Value;
    if (string.IsNullOrWhiteSpace(userEmail))
    {
        return Results.Unauthorized();
    }
    
    var user = context.Users
        .Include(user => user.UserRoles)
        .ThenInclude(ur => ur.Role)
        .Include(user => user.UserClaims)
        .FirstOrDefault(user => user.Email.Equals(userEmail));

    if (user is null)
    {
        return Results.NotFound();
    }    
        
    var roles = user.UserRoles.Select(userRole => userRole.Role.Name).ToList();
    var claims = user.UserClaims.Select(userClaims => new GetMeClaimResponse(userClaims.Type, userClaims.Value)).ToList();
    
    return Results.Ok(new GetMeResponse(user.Name, user.Email, roles, claims));
}).RequireAuthorization();

app.MapPost("/auth/login", (AppDbContext dbContext, IToken tokenService, IPasswordService passwordService, LoginRequest request) =>
{
    var user = dbContext.Users
        .Include(user => user.UserRoles)
        .ThenInclude(user => user.Role)
        .Include(user => user.UserClaims)
        .FirstOrDefault(user => user.Email.Equals(request.Username));
   
    if (user is null || !passwordService.IsPasswordSuccessfulVerified(user.PasswordHash, request.Password))
    {
        return Results.Unauthorized();
    }
    
    var accessTokenAsString = tokenService.CreateAccessToken(user);
    var (refreshTokenAsString, newRefreshTokenExpiresDate) = tokenService.CreateRefreshToken(user);
    var refreshToken = RefreshToken.Create(refreshTokenAsString, user.Email, newRefreshTokenExpiresDate);
    
    dbContext.RefreshTokens.Add(refreshToken);
    dbContext.SaveChanges();
    
    return Results.Ok(new LoginResponse(accessTokenAsString, refreshTokenAsString));
});

app.MapPost("/auth/refresh-token", (AppDbContext dbContext, IToken tokenService, RefreshTokenRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.RefreshToken))
    {
        return Results.BadRequest();
    }
    
    var existingRefreshToken = dbContext.RefreshTokens.FirstOrDefault(refreshToken => 
        refreshToken.Token == request.RefreshToken && 
        refreshToken.Username == request.Username);

    if (existingRefreshToken is null || existingRefreshToken.IsRevoked || existingRefreshToken.IsExpired())
    {
        return Results.Unauthorized();
    }

    var user = dbContext.Users
        .Include(user => user.UserRoles)
        .ThenInclude(user => user.Role)
        .FirstOrDefault(user => user.Email == request.Username);

    if (user is null)
    {
        return Results.Unauthorized();
    }
    
    var newAccessTokenAsString = tokenService.CreateAccessToken(user);
    var (newRefreshTokenAsString, newRefreshTokenExpiresDate) = tokenService.CreateRefreshToken(user);
    
    existingRefreshToken.Revoke(newRefreshTokenAsString);
    
    var newRefreshToken = RefreshToken.Create(newRefreshTokenAsString, user.Email, newRefreshTokenExpiresDate);
    dbContext.RefreshTokens.Add(newRefreshToken);
    dbContext.SaveChanges();
    
    return Results.Ok(new RefreshTokenResponse(newAccessTokenAsString, newRefreshTokenAsString));
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
}).RequireAuthorization(nameof(TodoPolicies.CanDelete));

app.UseAuthentication();
app.UseAuthorization();

app.Run();