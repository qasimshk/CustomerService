namespace customer.data.Configuration;

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // Ideal approach to name a table this way - this option is not available in in-memory EF 
        // builder.ToTable(nameof(Customer));

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(4);

        builder.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(20);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Dob)
                .IsRequired();

        builder.Property(c => c.ContactNumber)
                .IsRequired()
                .HasMaxLength(11);

        builder.Property(c => c.ReferenceNumber)
                .IsRequired();

        builder.HasIndex(c => c.ReferenceNumber)
                .IsUnique();

        builder.Property(c => c.EmailAddress)
                .IsRequired();

        builder.HasIndex(c => c.EmailAddress)
                .IsUnique();

        builder.HasOne(cus => cus.Address)
            .WithOne(add => add.Customer)
            .HasForeignKey<Address>("CustomerId")
            .IsRequired();
    }
}
