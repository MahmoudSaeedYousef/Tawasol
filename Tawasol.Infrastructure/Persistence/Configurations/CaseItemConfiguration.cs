using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawasol.Domain.Entities;

namespace Tawasol.Infrastructure.Persistence.Configurations;

public class CaseItemConfiguration : IEntityTypeConfiguration<CaseItem>
{
    public void Configure(EntityTypeBuilder<CaseItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Type)
            .HasConversion<string>();

        builder.Property(x => x.TargetAmount)
            .HasColumnType("decimal(18,2)");
    }
}
