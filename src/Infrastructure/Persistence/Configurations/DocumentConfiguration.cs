using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.Property(t => t.DocumentType)
               .HasConversion<string>();
        builder.Ignore(e => e.DomainEvents);
        builder.HasOne(x => x.Owner)
               .WithMany()
               .HasForeignKey(x => x.CreatedBy)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Editor)
               .WithMany()
               .HasForeignKey(x => x.LastModifiedBy)
               .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(e => e.Owner)
               .AutoInclude();
        builder.Navigation(e => e.Editor)
               .AutoInclude();
    }
}