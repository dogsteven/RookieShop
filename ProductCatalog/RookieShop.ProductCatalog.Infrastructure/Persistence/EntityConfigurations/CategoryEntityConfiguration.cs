using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RookieShop.ProductCatalog.Application.Entities;

namespace RookieShop.ProductCatalog.Infrastructure.Persistence.EntityConfigurations;

public class CategoryEntityConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories", schema: "ProductCatalog");
        
        builder.HasKey(category => category.Id);

        builder.Property(category => category.Id)
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasColumnName("Id");
        
        builder.Property(category => category.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("Name");
        
        builder.Property(category => category.Description)
            .IsRequired()
            .HasMaxLength(250)
            .HasColumnName("Description");
    }
}