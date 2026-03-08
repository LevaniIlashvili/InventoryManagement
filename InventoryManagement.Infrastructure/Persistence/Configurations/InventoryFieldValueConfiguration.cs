using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Persistence.Configurations;

public class ItemFieldValueConfiguration : IEntityTypeConfiguration<ItemFieldValue>
{
    public void Configure(EntityTypeBuilder<ItemFieldValue> builder)
    {
        builder.HasKey(fv => fv.Id);

        builder.Property(i => i.Id)
            .ValueGeneratedNever();

        builder.Property(fv => fv.Value)
               .HasMaxLength(1000);
    }
}
