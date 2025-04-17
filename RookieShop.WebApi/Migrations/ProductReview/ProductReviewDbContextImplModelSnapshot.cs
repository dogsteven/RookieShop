﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RookieShop.ProductReview.Infrastructure.Persistence;

#nullable disable

namespace RookieShop.WebApi.Migrations.ProductReview
{
    [DbContext(typeof(ProductReviewDbContextImpl))]
    partial class ProductReviewDbContextImplModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("RookieShop.ProductReview.Application.Entities.Reaction", b =>
                {
                    b.Property<Guid>("ReactorId")
                        .HasColumnType("uuid")
                        .HasColumnName("ReactorId");

                    b.Property<Guid>("WriterId")
                        .HasColumnType("uuid")
                        .HasColumnName("WriterId");

                    b.Property<string>("ProductSku")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)")
                        .HasColumnName("ProductSku");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Type");

                    b.HasKey("ReactorId", "WriterId", "ProductSku");

                    b.HasIndex("WriterId", "ProductSku");

                    b.ToTable("Reactions", "ProductReview");
                });

            modelBuilder.Entity("RookieShop.ProductReview.Application.Entities.Review", b =>
                {
                    b.Property<Guid>("WriterId")
                        .HasColumnType("uuid")
                        .HasColumnName("WriterId");

                    b.Property<string>("ProductSku")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)")
                        .HasColumnName("ProductSku");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("Comment");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedDate");

                    b.Property<int>("Score")
                        .HasColumnType("integer")
                        .HasColumnName("Score");

                    b.HasKey("WriterId", "ProductSku");

                    b.HasIndex("ProductSku");

                    b.ToTable("Reviews", "ProductReview");
                });

            modelBuilder.Entity("RookieShop.ProductReview.Application.Entities.Reaction", b =>
                {
                    b.HasOne("RookieShop.ProductReview.Application.Entities.Review", null)
                        .WithMany("Reactions")
                        .HasForeignKey("WriterId", "ProductSku")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RookieShop.ProductReview.Application.Entities.Review", b =>
                {
                    b.Navigation("Reactions");
                });
#pragma warning restore 612, 618
        }
    }
}
