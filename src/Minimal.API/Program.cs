using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Minimal.API.Enums;
using Minimal.API.Models;
using Minimal.API.Requests;
using Minimal.API.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TodosDb"));
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

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
});

app.Run();