using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RookieShop.ProductCatalog.Application.Entities;

namespace RookieShop.ProductCatalog.Infrastructure.Persistence.EntityConfigurations;

public class ProductRatingEntityConfiguration : IEntityTypeConfiguration<ProductRating>
{
    public void Configure(EntityTypeBuilder<ProductRating> builder)
    {
        builder.ToTable("ProductRatings", schema: "ProductCatalog");

        builder.HasKey(productRating => productRating.ProductSku);
        
        builder.Property(productRating => productRating.ProductSku)
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnName("Sku");
        
        builder.Property(productRating => productRating.Score)
            .IsRequired()
            .HasColumnName("Score");
        
        builder.Property(productRating => productRating.OneCount)
            .IsRequired()
            .HasColumnName("OneCount");
        
        builder.Property(productRating => productRating.TwoCount)
            .IsRequired()
            .HasColumnName("TwoCount");
        
        builder.Property(productRating => productRating.ThreeCount)
            .IsRequired()
            .HasColumnName("ThreeCount");
        
        builder.Property(productRating => productRating.FourCount)
            .IsRequired()
            .HasColumnName("FourCount");
        
        builder.Property(productRating => productRating.FiveCount)
            .IsRequired()
            .HasColumnName("FiveCount");

        builder.Property<DateTime>("Version")
            .IsRequired()
            .IsConcurrencyToken();
    }
}