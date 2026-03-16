using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Persistence.Configurations;

public class CustomIdElementConfiguration
    : IEntityTypeConfiguration<CustomIdElement>
{
    public void Configure(EntityTypeBuilder<CustomIdElement> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
           .ValueGeneratedNever();

        builder.Property(x => x.Type)
               .IsRequired()
               .HasConversion<int>();

        builder.Property(x => x.Order)
               .IsRequired();
    }
}
