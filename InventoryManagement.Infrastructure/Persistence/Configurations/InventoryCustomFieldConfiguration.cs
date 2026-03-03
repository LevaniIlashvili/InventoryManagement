using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Persistence.Configurations;

public class InventoryCustomFieldConfiguration
    : IEntityTypeConfiguration<InventoryCustomField>
{
    public void Configure(EntityTypeBuilder<InventoryCustomField> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
           .ValueGeneratedNever();

        builder.Property(x => x.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Description)
               .HasMaxLength(1000);

        builder.Property(x => x.ShouldBeDisplayed)
               .IsRequired();

        builder.Property(x => x.Type)
               .IsRequired()
               .HasConversion<int>();

        builder.Property(x => x.Order)
               .IsRequired();
    }
}