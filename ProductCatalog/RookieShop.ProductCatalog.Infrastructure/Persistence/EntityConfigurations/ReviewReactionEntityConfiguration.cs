using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RookieShop.ProductCatalog.Application.Entities;

namespace RookieShop.ProductCatalog.Infrastructure.Persistence.EntityConfigurations;

public class ReviewReactionEntityConfiguration : IEntityTypeConfiguration<ReviewReaction>
{
    public void Configure(EntityTypeBuilder<ReviewReaction> builder)
    {
        builder.ToTable("ReviewReactions", schema: "ProductCatalog");

        builder.HasKey(reviewReaction => new { reviewReaction.ReactorId, reviewReaction.WriterId, reviewReaction.ProductSku });

        builder.Property(reviewReaction => reviewReaction.ReactorId)
            .IsRequired()
            .HasColumnName("ReactorId");
        
        builder.Property(reviewReaction => reviewReaction.WriterId)
            .IsRequired()
            .HasColumnName("WriterId");

        builder.Property(reviewReaction => reviewReaction.ProductSku)
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnName("ProductSku");

        builder.Property(reviewReaction => reviewReaction.Type)
            .IsRequired()
            .HasConversion(new EnumToStringConverter<ReviewReactionType>())
            .HasColumnName("Type");

        builder.HasKey(reviewReaction => reviewReaction.Type);

        builder.HasIndex(reviewReaction => new { reviewReaction.WriterId, reviewReaction.ProductSku });
    }
}