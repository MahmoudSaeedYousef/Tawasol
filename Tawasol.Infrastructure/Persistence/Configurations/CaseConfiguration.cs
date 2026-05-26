using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawasol.Domain.Entities;
using Tawasol.Domain.ValueObjects;

namespace Tawasol.Infrastructure.Persistence.Configurations;

public class CaseConfiguration : IEntityTypeConfiguration<Case>
{
    public void Configure(EntityTypeBuilder<Case> builder)
    {
        builder.HasKey(c => c.Id);

        // Configure Location as an owned type (value object)
        builder.OwnsOne(c => c.Location, locationBuilder =>
        {
            locationBuilder.Property(l => l.Latitude).HasColumnName("Location_Latitude");
            locationBuilder.Property(l => l.Longitude).HasColumnName("Location_Longitude");
        });
        
        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .IsRequired();

        builder.Property(c => c.TargetAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.CollectedAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.Status)
            .HasConversion<string>();

        builder.Property(c => c.CaseType)
            .IsRequired()
            .HasMaxLength(50);

        // More robust Value Converter
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        builder.Property(c => c.ExtraDetails)
            .HasConversion(
                v => JsonSerializer.Serialize(v, options),
                v => string.IsNullOrEmpty(v) 
                    ? new Dictionary<string, string>() 
                    : JsonSerializer.Deserialize<Dictionary<string, string>>(v, options) ?? new Dictionary<string, string>(),
                new ValueComparer<IReadOnlyDictionary<string, string>>(
                    (c1, c2) => JsonSerializer.Serialize(c1, options) == JsonSerializer.Serialize(c2, options),
                    c => JsonSerializer.Serialize(c, options).GetHashCode(),
                    c => JsonSerializer.Deserialize<Dictionary<string, string>>(JsonSerializer.Serialize(c, options), options)!
                )
            )
            .HasColumnType("nvarchar(max)");
            
        builder.HasMany(c => c.Attachments)
            .WithOne()
            .HasForeignKey(a => a.CaseId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Metadata.FindNavigation(nameof(Case.Attachments))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
