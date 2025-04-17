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

        builder.HasKey(reaction => new { reaction.ReactorId, reaction.WriterId, reaction.ProductSku });

        builder.Property(reaction => reaction.ReactorId)
            .IsRequired()
            .HasColumnName("ReactorId");
        
        builder.Property(id => id.WriterId)
            .IsRequired()
            .HasColumnName("WriterId");

        builder.Property(id => id.ProductSku)
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnName("ProductSku");

        builder.Property(reaction => reaction.Type)
            .IsRequired()
            .HasConversion(new EnumToStringConverter<ReactionType>())
            .HasColumnName("Type");

        builder.HasIndex(reaction => new { reaction.WriterId, reaction.ProductSku });
    }
}