using Minimal.API.Enums;

namespace Minimal.API.Models;

public class Todo
{
    private Todo() {}
    
    public static Todo Create(string title, string description)
    {
        return new Todo
        {
            Title = title,
            Description = description
        };
    }

    public void Update(string title, string description)
    {
        Title = title;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }
  
    public void Delete()
    {
        Status = TodoStatus.Deleted;
        DeletedAt = DateTime.UtcNow;
    }
    
    public void MarkAsDone()
    {
        Status = TodoStatus.Done;
        UpdatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public TodoStatus Status { get; private set; } = TodoStatus.Open;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
