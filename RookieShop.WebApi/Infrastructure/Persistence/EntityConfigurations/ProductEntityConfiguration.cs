using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RookieShop.Application.Entities;

namespace RookieShop.WebApi.Infrastructure.Persistence.EntityConfigurations;

public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(product => product.Sku);
        
        builder.Property(product => product.Sku)
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnName("Sku");
        
        builder.Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("Name");
        
        builder.Property(product => product.Description)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("Description");
        
        builder.Property(product => product.Price)
            .IsRequired()
            .HasColumnName("Price");
        
        builder.HasOne(product => product.Category)
            .WithMany()
            .HasForeignKey("CategoryId")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(product => product.ImageUrl)
            .IsRequired()
            .HasMaxLength(250)
            .HasColumnName("ImageUrl");
        
        builder.Property(product => product.IsFeatured)
            .IsRequired()
            .HasColumnName("IsFeatured");
        
        builder.Property(product => product.CreatedDate)
            .IsRequired()
            .HasColumnName("CreatedDate");
        
        builder.Property(product => product.UpdatedDate)
            .IsRequired()
            .HasColumnName("UpdatedDate");
        
        builder.HasIndex("CategoryId");
    }
}