using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RookieShop.ImageGallery.Application.Entities;

namespace RookieShop.ImageGallery.Infrastructure.Persistence.EntityConfigurations;

public class ImageEntityConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.ToTable("Images", schema: "ImageGallery");
        
        builder.HasKey(image => image.Id);
        
        builder.Property(image => image.Id)
            .IsRequired()
            .HasColumnName("Id");
        
        builder.Property(image => image.ContentType)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("ContentType");
        
        builder.Property(image => image.TemporaryEntryId)
            .IsRequired()
            .HasMaxLength(250)
            .HasColumnName("TemporaryEntryId");
        
        builder.Property(image => image.CreatedDate)
            .IsRequired()
            .HasColumnName("CreatedDate");
        
        builder.Property(image => image.IsSynced)
            .IsRequired()
            .HasColumnName("IsSynced");
    }
}