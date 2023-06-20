namespace customer.data.Configuration;

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class NationEntityTypeConfiguration : IEntityTypeConfiguration<Nation>
{
    public void Configure(EntityTypeBuilder<Nation> builder)
    {
        // Ideal approach to name a table this way - this option is not available in in-memory EF
        // builder.ToTable(nameof(Nation));

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
                .IsRequired()
                .ValueGeneratedNever();

        builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50);
    }
}
