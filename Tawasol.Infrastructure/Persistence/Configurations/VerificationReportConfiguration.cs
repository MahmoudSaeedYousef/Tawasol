using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawasol.Domain.Entities;

namespace Tawasol.Infrastructure.Persistence.Configurations;

public class VerificationReportConfiguration : IEntityTypeConfiguration<VerificationReport>
{
    public void Configure(EntityTypeBuilder<VerificationReport> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FieldNotes)
            .IsRequired();

        builder.HasOne<Case>()
            .WithOne(c => c.ResearchReport)
            .HasForeignKey<VerificationReport>(x => x.CaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
