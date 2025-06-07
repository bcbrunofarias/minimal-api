using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.API.Models;

namespace Minimal.API.Database.Mapping;

public class RoleEntityMapping : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(config => config.Id);
        
        builder.Property(config => config.Name)
            .HasMaxLength(100)
            .IsRequired();
    }
}