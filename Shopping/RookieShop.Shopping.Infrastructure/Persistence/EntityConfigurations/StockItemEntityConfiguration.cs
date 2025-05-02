using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RookieShop.Shopping.Domain;
using RookieShop.Shopping.Domain.StockItems;

namespace RookieShop.Shopping.Infrastructure.Persistence.EntityConfigurations;

public class StockItemEntityConfiguration : IEntityTypeConfiguration<StockItem>
{
    public void Configure(EntityTypeBuilder<StockItem> builder)
    {
        builder.ToTable("StockItems", schema: "Shopping");

        builder.HasKey(stockItem => stockItem.Sku);

        builder.Property(stockItem => stockItem.Sku)
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnName("Sku");
        
        builder.Property(stockItem => stockItem.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("Name");
        
        builder.Property(stockItem => stockItem.Price)
            .IsRequired()
            .HasColumnName("Price");
        
        builder.Property(stockItem => stockItem.ImageId)
            .IsRequired()
            .HasColumnName("ImageId");
        
        builder.Property(stockItem => stockItem.AvailableQuantity)
            .IsRequired()
            .HasColumnName("AvailableQuantity");
        
        builder.Property(stockItem => stockItem.ReservedQuantity)
            .IsRequired()
            .HasColumnName("ReservedQuantity");
        
        builder.Property<DateTime>("Version")
            .IsRequired()
            .IsConcurrencyToken();
    }
}