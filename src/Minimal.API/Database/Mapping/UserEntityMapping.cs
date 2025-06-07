using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.API.Models;

namespace Minimal.API.Database.Mapping;

public class UserEntityMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(config => config.Id);
        
        builder.Property(config => config.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(config => config.Email)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(config => config.PasswordHash)
            .HasMaxLength(200)
            .IsRequired();
    }
}