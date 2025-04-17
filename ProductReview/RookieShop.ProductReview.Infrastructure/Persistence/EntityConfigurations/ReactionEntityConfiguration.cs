using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RookieShop.ProductReview.Application.Entities;

namespace RookieShop.ProductReview.Infrastructure.Persistence.EntityConfigurations;

public class ReactionEntityConfiguration : IEntityTypeConfiguration<Reaction>
{
    public void Configure(EntityTypeBuilder<Reaction> builder)
    {
        builder.ToTable("Reactions", schema: "ProductReview");

        builder.HasKey(reaction => new { reaction.ReactorId, reaction.ReviewId });

        builder.ComplexProperty(reaction => reaction.ReviewId, reviewId =>
        {
            reviewId.Property(id => id.WriterId)
                .IsRequired()
                .HasColumnName("WriterId");
            
            reviewId.Property(id => id.ProductSku)
                .IsRequired()
                .HasMaxLength(16)
                .HasColumnName("ProductSku");
            
            reviewId.IsRequired();
        });
        
        builder.Property(reaction => reaction.ReactorId)
            .IsRequired()
            .HasColumnName("ReactorId");

        builder.Property(reaction => reaction.Type)
            .IsRequired()
            .HasConversion(new EnumToStringConverter<ReactionType>())
            .HasColumnName("Type");

        builder.HasIndex(reaction => reaction.ReviewId);
    }
}