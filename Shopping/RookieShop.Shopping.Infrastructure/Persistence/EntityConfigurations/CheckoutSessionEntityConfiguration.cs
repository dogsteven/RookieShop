using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RookieShop.Shopping.Domain.CheckoutSessions;
using RookieShop.Shopping.Domain.Shared;

namespace RookieShop.Shopping.Infrastructure.Persistence.EntityConfigurations;

public class CheckoutSessionEntityConfiguration : IEntityTypeConfiguration<CheckoutSession>
{
    public void Configure(EntityTypeBuilder<CheckoutSession> builder)
    {
        builder.ToTable("CheckoutSessions", schema: "Shopping");

        builder.HasKey(session => session.Id);
        
        builder.Property(session => session.Id)
            .IsRequired()
            .HasColumnName("Id");
        
        builder.Property(session => session.SessionId)
            .IsRequired()
            .HasColumnName("SessionId");
        
        builder.Property(session => session.IsActive)
            .IsRequired()
            .HasColumnName("IsActive");

        builder.OwnsOne(session => session.BillingAddress, addressBuilder =>
        {
            addressBuilder.ToTable("CheckoutSessionBillingAddresses", schema: "Shopping");
            
            addressBuilder.WithOwner()
                .HasForeignKey("CheckoutSessionId");

            addressBuilder.HasKey("CheckoutSessionId");
            
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
        
        builder.OwnsOne(session => session.ShippingAddress, addressBuilder =>
        {
            addressBuilder.ToTable("CheckoutSessionShippingAddresses", schema: "Shopping");
            
            addressBuilder.WithOwner()
                .HasForeignKey("CheckoutSessionId");

            addressBuilder.HasKey("CheckoutSessionId");
            
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

        builder.OwnsMany<CheckoutItem>("_items", itemBuilder =>
        {
            itemBuilder.ToTable("CheckoutSessionCheckoutItems", schema: "Shopping");
            
            itemBuilder.WithOwner()
                .HasForeignKey("CheckoutSessionId");
            
            itemBuilder.HasKey("CheckoutSessionId", "Sku");
            
            itemBuilder.Property(item=> item.Sku)
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

            itemBuilder.HasIndex("CheckoutSessionId");
        })
        .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(session => session.Items);

        builder.Property<DateTime>("Version")
            .IsRequired()
            .IsConcurrencyToken();
    }
}