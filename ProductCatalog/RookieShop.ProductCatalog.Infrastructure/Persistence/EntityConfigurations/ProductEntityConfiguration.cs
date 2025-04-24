using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RookieShop.ProductCatalog.Application.Entities;

namespace RookieShop.ProductCatalog.Infrastructure.Persistence.EntityConfigurations;

public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", schema: "ProductCatalog");

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
            .HasMaxLength(1000)
            .HasColumnName("Description");
        
        builder.Property(product => product.Price)
            .IsRequired()
            .HasColumnName("Price");
        
        builder.HasOne(product => product.Category)
            .WithMany()
            .HasForeignKey("CategoryId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(product => product.PrimaryImageId)
            .IsRequired()
            .HasColumnName("PrimaryImageId");

        builder.PrimitiveCollection(product => product.SupportingImageIds)
            .IsRequired()
            .HasColumnName("SupportingImageIds")
            .HasDefaultValueSql("array[]::uuid[]");
        
        builder.Property(product => product.IsFeatured)
            .IsRequired()
            .HasColumnName("IsFeatured");
        
        builder.Property(product => product.CreatedDate)
            .IsRequired()
            .HasColumnName("CreatedDate");
        
        builder.Property(product => product.UpdatedDate)
            .IsRequired()
            .HasColumnName("UpdatedDate");

        builder.HasOne(product => product.Rating)
            .WithOne()
            .HasForeignKey<Rating>(rating => rating.ProductSku)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex("CategoryId");
    }
}

public class GuidSetAndStringValueConverter : ValueConverter<ISet<Guid>, string>
{
    private static ISet<Guid> ConvertStringToGuidSet(string stringValue)
    {
        var guidSet = new HashSet<Guid>();

        foreach (var text in stringValue.Split(','))
        {
            if (Guid.TryParse(text, out var result))
            {
                guidSet.Add(result);
            }
        }

        return guidSet;
    }

    private static string ConvertGuidSetToString(ISet<Guid> guidSet)
    {
        return string.Join(',', guidSet);
    }
    
    public GuidSetAndStringValueConverter() : base((guidSet) => ConvertGuidSetToString(guidSet), (stringValue) => ConvertStringToGuidSet(stringValue)) {}
}