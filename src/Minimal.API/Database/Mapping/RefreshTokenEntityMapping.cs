using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.API.Models;

namespace Minimal.API.Database.Mapping;

public class RefreshTokenEntityMapping : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(config => config.Id);

        builder.Property(config => config.Token)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(config => config.Username)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(config => config.ReplacedByToken)
            .HasMaxLength(200);
    }
}