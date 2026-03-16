using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Persistence.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Description)
            .HasMaxLength(500);


        builder.HasMany(i => i.CustomFields)
            .WithOne(cf => cf.Inventory)
            .HasForeignKey(cf => cf.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.Tags)
            .WithMany(t => t.Inventories)
            .UsingEntity(j => j.ToTable("InventoryInventoryTag"));

        builder.HasMany(i => i.Items)
            .WithOne(i => i.Inventory)
            .HasForeignKey(i => i.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.CustomIdElements)
            .WithOne(i => i.Inventory)
            .HasForeignKey(i => i.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
