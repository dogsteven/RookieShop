using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RookieShop.Application.Entities;

namespace RookieShop.WebApi.Infrastructure.Persistence.EntityConfigurations;

public class RatingEntityConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.ToTable("Ratings");

        builder.HasKey(rating => new { rating.CustomerId, rating.Sku });
        
        builder.Property(rating => rating.CustomerId)
            .IsRequired()
            .HasColumnName("CustomerId");
        
        builder.Property(rating => rating.Sku)
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnName("Sku");
        
        builder.Property(rating => rating.Score)
            .IsRequired()
            .HasColumnName("Score");
        
        builder.Property(rating => rating.Comment)
            .IsRequired()
            .HasMaxLength(250)
            .HasColumnName("Comment");
        
        builder.Property(rating => rating.CreatedDate)
            .IsRequired()
            .HasColumnName("CreatedDate");

        builder.HasIndex(rating => rating.Sku);
    }
}