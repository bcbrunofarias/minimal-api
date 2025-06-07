using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.API.Models;

namespace Minimal.API.Database.Mapping;

public class UserClaimEntityMapping : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.HasKey(config => config.Id);
        
        builder.Property(config => config.UserId)
            .IsRequired();
        
        builder.Property(config => config.Type)
            .HasMaxLength(128)
            .IsRequired();
        
        builder.Property(config => config.Value)
            .HasMaxLength(128)
            .IsRequired();
    }
}