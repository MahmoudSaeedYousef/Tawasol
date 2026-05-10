using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawasol.Domain.Entities;

namespace Tawasol.Infrastructure.Persistence.Configurations;

public class CaseAttachmentConfiguration : IEntityTypeConfiguration<CaseAttachment>
{
    public void Configure(EntityTypeBuilder<CaseAttachment> builder)
    {
        builder.HasKey(x => x.Id);
        
        // Standard ID generation
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.FileType)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne<Case>()
            .WithMany(c => c.Attachments)
            .HasForeignKey(x => x.CaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
