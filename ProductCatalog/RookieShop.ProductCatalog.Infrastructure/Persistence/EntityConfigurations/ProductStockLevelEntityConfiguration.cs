using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RookieShop.ProductCatalog.Application.Entities;

namespace RookieShop.ProductCatalog.Infrastructure.Persistence.EntityConfigurations;

public class ProductStockLevelEntityConfiguration : IEntityTypeConfiguration<ProductStockLevel>
{
    public void Configure(EntityTypeBuilder<ProductStockLevel> builder)
    {
        builder.ToTable("ProductStockLevels", schema: "ProductCatalog");

        builder.HasKey(productStockLevel => productStockLevel.ProductSku);

        builder.Property(productStockLevel => productStockLevel.ProductSku)
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnName("ProductSku");
        
        builder.Property(productStockLevel => productStockLevel.AvailableQuantity)
            .IsRequired()
            .HasColumnName("AvailableQuantity");

        builder.Property<DateTime>("Version")
            .IsRequired()
            .IsConcurrencyToken();
    }
}