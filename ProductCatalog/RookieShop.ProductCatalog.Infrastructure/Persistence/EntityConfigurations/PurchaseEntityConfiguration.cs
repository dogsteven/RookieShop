using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RookieShop.ProductCatalog.Application.Entities;

namespace RookieShop.ProductCatalog.Infrastructure.Persistence.EntityConfigurations;

public class PurchaseEntityConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("Purchases", schema: "ProductCatalog");

        builder.HasKey(purchase => new { purchase.CustomerId, purchase.ProductSku });
        
        builder.Property(purchase => purchase.CustomerId)
            .IsRequired()
            .HasColumnName("CustomerId");
        
        builder.Property(purchase => purchase.ProductSku)
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnName("ProductSku");
    }
}