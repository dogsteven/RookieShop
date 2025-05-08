using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RookieShop.Ordering.Domain.Orders;

namespace RookieShop.Ordering.Infrastructure.Persistence.EntityConfigurations;

public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders", schema: "Ordering");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.Id)
            .IsRequired()
            .HasColumnName("Id");
        
        builder.Property(order => order.CustomerId)
            .IsRequired()
            .HasColumnName("CustomerId");
        
        builder.Property(order => order.PlacedTime)
            .IsRequired()
            .HasColumnName("PlacedTime");

        builder.OwnsOne(order => order.BillingAddress, addressBuilder =>
        {
            addressBuilder.ToTable("OrderBillingAddresses", schema: "Ordering");

            addressBuilder.WithOwner()
                .HasForeignKey("OrderId");

            addressBuilder.HasKey("OrderId");

            addressBuilder.Property(address => address.Street)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Street");

            addressBuilder.Property(address => address.City)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("City");

            addressBuilder.Property(address => address.State)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("State");
        });

        builder.OwnsOne(order => order.ShippingAddress, addressBuilder =>
        {
            addressBuilder.ToTable("OrderShippingAddresses", schema: "Ordering");

            addressBuilder.WithOwner()
                .HasForeignKey("OrderId");

            addressBuilder.HasKey("OrderId");

            addressBuilder.Property(address => address.Street)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Street");

            addressBuilder.Property(address => address.City)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("City");

            addressBuilder.Property(address => address.State)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("State");
        });
        
        builder.Property(order => order.Status)
            .IsRequired()
            .HasConversion(new EnumToStringConverter<OrderStatus>())
            .HasColumnName("Status");

        builder.OwnsMany<OrderItem>("_items", itemBuilder =>
        {
            itemBuilder.ToTable("OrderItems", schema: "Ordering");
            
            itemBuilder.WithOwner()
                .HasForeignKey("OrderId");
            
            itemBuilder.HasKey("OrderId", "Sku");
            
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
            
            itemBuilder.Property(item => item.Quantity)
                .IsRequired()
                .HasColumnName("Quantity");

            itemBuilder.HasIndex("OrderId");
        });

        builder.Ignore(order => order.Items);

        builder.Property<DateTime>("Version")
            .IsRequired()
            .IsConcurrencyToken();
        
        builder.HasIndex(order => order.CustomerId);
    }
}