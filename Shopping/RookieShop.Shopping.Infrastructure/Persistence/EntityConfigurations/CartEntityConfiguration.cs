using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RookieShop.Shopping.Domain;
using RookieShop.Shopping.Domain.Carts;

namespace RookieShop.Shopping.Infrastructure.Persistence.EntityConfigurations;

public class CartEntityConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts", schema: "Shopping");

        builder.HasKey(cart => cart.Id);

        builder.Property(cart => cart.Id)
            .IsRequired()
            .HasColumnName("Id");
        
        builder.Property(cart => cart.ExpirationTime)
            .IsRequired()
            .HasColumnName("ExpirationTime");

        builder.OwnsMany<CartItem>("_items", itemBuilder =>
            {
                itemBuilder.ToTable("CartItems", schema: "Shopping");
                
                itemBuilder.WithOwner()
                    .HasForeignKey("CartId");
                
                itemBuilder.HasKey("CartId", "Sku");
                
                itemBuilder.Property(item => item.Sku)
                    .IsRequired()
                    .HasMaxLength(16)
                    .HasColumnName("Sku");

                itemBuilder.Property(item => item.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("Name");
            
                itemBuilder.Property(item => item.Price)
                    .IsRequired()
                    .HasColumnName("Price");
            
                itemBuilder.Property(item => item.ImageId)
                    .IsRequired()
                    .HasColumnName("ImageId");

                itemBuilder.Property(item => item.Quantity)
                    .IsRequired()
                    .HasColumnName("Quantity");

                itemBuilder.HasIndex("CartId");
            })
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.Ignore(cart => cart.Items);
        
        builder.Property<DateTime>("Version")
            .IsRequired()
            .IsConcurrencyToken();
    }
}