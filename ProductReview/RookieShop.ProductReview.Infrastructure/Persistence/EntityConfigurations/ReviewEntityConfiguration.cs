using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RookieShop.ProductReview.Application.Entities;

namespace RookieShop.ProductReview.Infrastructure.Persistence.EntityConfigurations;

public class ReviewEntityConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews", schema: "ProductReview");

        builder.HasKey(review => new { review.WriterId, review.ProductSku });

        builder.Property(id => id.WriterId)
            .IsRequired()
            .HasColumnName("WriterId");

        builder.Property(id => id.ProductSku)
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnName("ProductSku");
        
        builder.Property(review => review.Score)
            .IsRequired()
            .HasColumnName("Score");
        
        builder.Property(review => review.Comment)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("Comment");
        
        builder.Property(review => review.CreatedDate)
            .IsRequired()
            .HasColumnName("CreatedDate");

        builder.HasMany(review => review.Reactions)
            .WithOne()
            .HasForeignKey(reaction => new { reaction.WriterId, reaction.ProductSku });

        builder.HasIndex(review => review.ProductSku);
    }
}