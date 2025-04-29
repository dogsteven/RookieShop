using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RookieShop.ProductCatalog.Application.Entities;

namespace RookieShop.ProductCatalog.Infrastructure.Persistence.EntityConfigurations;

public class ProductSemanticVectorEntityConfiguration : IEntityTypeConfiguration<ProductSemanticVector>
{
    public void Configure(EntityTypeBuilder<ProductSemanticVector> builder)
    {
        builder.ToTable("ProductSemanticVectors", schema: "ProductCatalog");

        builder.HasKey(productSemanticVector => productSemanticVector.ProductSku);
        
        builder.Property(productSemanticVector => productSemanticVector.ProductSku)
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnName("ProductSku");

        builder.Property(productSemanticVector => productSemanticVector.SemanticVector)
            .IsRequired()
            .HasColumnName("SemanticVector");

        builder.Property<DateTime>("Version")
            .IsRequired()
            .IsConcurrencyToken();
    }
}