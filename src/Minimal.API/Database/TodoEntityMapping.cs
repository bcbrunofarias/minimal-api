using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.API.Models;

namespace Minimal.API.Database;

public class TodoEntityMapping : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {
        builder.HasKey(todo => todo.Id);
        builder.Property(todo => todo.Title)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(todo => todo.Description)
            .IsRequired()
            .HasMaxLength(200);
    }
}