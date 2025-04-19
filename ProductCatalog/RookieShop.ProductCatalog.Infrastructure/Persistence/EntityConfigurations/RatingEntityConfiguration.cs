using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RookieShop.ProductCatalog.Application.Entities;

namespace RookieShop.ProductCatalog.Infrastructure.Persistence.EntityConfigurations;

public class RatingEntityConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.ToTable("Ratings", schema: "ProductCatalog");

        builder.HasKey(rating => rating.ProductSku);
        
        builder.Property(rating => rating.ProductSku)
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnName("Sku");
        
        builder.Property(rating => rating.Score)
            .IsRequired()
            .HasColumnName("Score");
        
        builder.Property(rating => rating.OneCount)
            .IsRequired()
            .HasColumnName("OneCount");
        
        builder.Property(rating => rating.TwoCount)
            .IsRequired()
            .HasColumnName("TwoCount");
        
        builder.Property(rating => rating.ThreeCount)
            .IsRequired()
            .HasColumnName("ThreeCount");
        
        builder.Property(rating => rating.FourCount)
            .IsRequired()
            .HasColumnName("FourCount");
        
        builder.Property(rating => rating.FiveCount)
            .IsRequired()
            .HasColumnName("FiveCount");

        builder.Property<DateTime>("Version")
            .IsRequired()
            .IsConcurrencyToken();
    }
}