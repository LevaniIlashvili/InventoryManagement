using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Persistence.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.CustomId)
               .HasMaxLength(100);

        builder.Property(i => i.CreatedAt)
               .IsRequired();

        builder.HasMany(i => i.Values)
               .WithOne(v => v.InventoryItem)
               .HasForeignKey(v => v.InventoryItemId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
