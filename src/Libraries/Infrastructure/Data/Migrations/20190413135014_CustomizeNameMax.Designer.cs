﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Migrations
{
    [DbContext(typeof(DamaContext))]
    [Migration("20190413135014_CustomizeNameMax")]
    partial class CustomizeNameMax
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ApplicationCore.Entities.BasketAggregate.Basket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BuyerId");

                    b.Property<DateTime?>("CreatedDate");

                    b.Property<DateTime?>("UpdatedDate");

                    b.HasKey("Id");

                    b.ToTable("Baskets");
                });

            modelBuilder.Entity("ApplicationCore.Entities.BasketAggregate.BasketItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BasketId");

                    b.Property<int?>("CatalogAttribute1");

                    b.Property<int?>("CatalogAttribute2");

                    b.Property<int?>("CatalogAttribute3");

                    b.Property<int>("CatalogItemId");

                    b.Property<int?>("CatalogTypeId");

                    b.Property<DateTime?>("CreatedDate");

                    b.Property<string>("CustomizeColors");

                    b.Property<string>("CustomizeDescription");

                    b.Property<string>("CustomizeName");

                    b.Property<string>("CustomizeSide")
                        .HasMaxLength(100);

                    b.Property<int>("Quantity");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("UpdatedDate");

                    b.HasKey("Id");

                    b.HasIndex("BasketId");

                    b.ToTable("BasketItem");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogAttribute", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CatalogItemId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int>("Stock");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("CatalogItemId");

                    b.ToTable("CatalogAttribute");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CatalogItemId");

                    b.Property<int>("CategoryId");

                    b.HasKey("Id");

                    b.HasIndex("CatalogItemId");

                    b.HasIndex("CategoryId");

                    b.ToTable("CatalogCategory");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogIllustration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<int>("IllustrationTypeId");

                    b.Property<byte[]>("Image");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("PictureUri")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("IllustrationTypeId");

                    b.ToTable("CatalogIllustration");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("CanCustomize");

                    b.Property<int>("CatalogIllustrationId");

                    b.Property<int>("CatalogTypeId");

                    b.Property<string>("Description");

                    b.Property<bool>("IsFeatured");

                    b.Property<bool>("IsNew");

                    b.Property<string>("MetaDescription")
                        .HasMaxLength(161);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("PictureUri");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("ShowOnShop");

                    b.Property<string>("Sku")
                        .HasMaxLength(255);

                    b.Property<string>("Slug")
                        .HasMaxLength(100);

                    b.Property<int>("Stock");

                    b.Property<string>("Title")
                        .HasMaxLength(61);

                    b.HasKey("Id");

                    b.HasIndex("CatalogIllustrationId");

                    b.HasIndex("CatalogTypeId");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("Catalog");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogPicture", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CatalogItemId");

                    b.Property<bool>("IsActive");

                    b.Property<int>("Order");

                    b.Property<string>("PictureUri")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CatalogItemId");

                    b.ToTable("CatalogPicture");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogReference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CatalogItemId");

                    b.Property<string>("LabelDescription")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(20)
                        .HasDefaultValue("Tamanho");

                    b.Property<int>("ReferenceCatalogItemId");

                    b.HasKey("Id");

                    b.HasIndex("CatalogItemId");

                    b.HasIndex("ReferenceCatalogItemId");

                    b.ToTable("CatalogReference");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal?>("AdditionalTextPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<int>("DeliveryTimeMax")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(3);

                    b.Property<int>("DeliveryTimeMin")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(2);

                    b.Property<int>("DeliveryTimeUnit")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("MetaDescription")
                        .HasMaxLength(161);

                    b.Property<string>("PictureUri")
                        .HasMaxLength(255);

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Slug")
                        .HasMaxLength(100);

                    b.Property<string>("Title")
                        .HasMaxLength(61);

                    b.Property<int?>("Weight");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("CatalogType");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogTypeCategory", b =>
                {
                    b.Property<int>("CatalogTypeId");

                    b.Property<int>("CategoryId");

                    b.HasKey("CatalogTypeId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("CatalogTypeCategory");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("MetaDescription")
                        .HasMaxLength(161);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int>("Order")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.Property<int?>("ParentId");

                    b.Property<string>("Slug")
                        .HasMaxLength(100);

                    b.Property<string>("Title")
                        .HasMaxLength(61);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("ParentId");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("Category");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CustomizeOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AttachFileName");

                    b.Property<string>("BuyerContact")
                        .IsRequired();

                    b.Property<string>("BuyerId")
                        .IsRequired();

                    b.Property<string>("BuyerName")
                        .IsRequired();

                    b.Property<string>("Colors");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<DateTimeOffset>("OrderDate");

                    b.Property<int>("OrderState")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.ToTable("CustomizeOrder");
                });

            modelBuilder.Entity("ApplicationCore.Entities.FileDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CatalogTypeId");

                    b.Property<string>("Extension")
                        .HasMaxLength(10);

                    b.Property<string>("FileName")
                        .HasMaxLength(100);

                    b.Property<bool?>("IsActive");

                    b.Property<string>("Location")
                        .HasMaxLength(255);

                    b.Property<int?>("Order");

                    b.Property<string>("PictureUri")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("CatalogTypeId");

                    b.ToTable("FileDetail");
                });

            modelBuilder.Entity("ApplicationCore.Entities.IllustrationType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("IllustrationType");
                });

            modelBuilder.Entity("ApplicationCore.Entities.OrderAggregate.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BuyerId");

                    b.Property<DateTimeOffset>("OrderDate");

                    b.Property<int>("OrderState")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(100);

                    b.Property<long?>("SalesInvoiceId");

                    b.Property<string>("SalesInvoiceNumber")
                        .HasMaxLength(255);

                    b.Property<long?>("SalesPaymentId");

                    b.Property<decimal>("ShippingCost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("TaxNumber");

                    b.Property<bool>("UseBillingSameAsShipping");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("ApplicationCore.Entities.OrderAggregate.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CatalogAttribute1");

                    b.Property<int?>("CatalogAttribute2");

                    b.Property<int?>("CatalogAttribute3");

                    b.Property<string>("CustomizeName");

                    b.Property<string>("CustomizeSide")
                        .HasMaxLength(100);

                    b.Property<int?>("OrderId");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Units");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ShippingPriceWeight", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Country");

                    b.Property<int?>("MaxWeight");

                    b.Property<int>("MinWeight");

                    b.Property<decimal>("Price");

                    b.HasKey("Id");

                    b.ToTable("ShippingPriceWeights");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ShopConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsActive");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<int>("Type");

                    b.Property<string>("Value")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("ShopConfig");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ShopConfigDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContentText");

                    b.Property<string>("HeadingText");

                    b.Property<bool>("IsActive");

                    b.Property<string>("LinkButtonText");

                    b.Property<string>("LinkButtonUri");

                    b.Property<string>("PictureMobileUri")
                        .HasMaxLength(255);

                    b.Property<string>("PictureUri")
                        .HasMaxLength(255);

                    b.Property<string>("PictureWebpUri")
                        .HasMaxLength(255);

                    b.Property<int>("ShopConfigId");

                    b.HasKey("Id");

                    b.HasIndex("ShopConfigId");

                    b.ToTable("ShopConfigDetail");
                });

            modelBuilder.Entity("ApplicationCore.Entities.BasketAggregate.BasketItem", b =>
                {
                    b.HasOne("ApplicationCore.Entities.BasketAggregate.Basket", "Basket")
                        .WithMany("Items")
                        .HasForeignKey("BasketId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogAttribute", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogItem", "CatalogItem")
                        .WithMany("CatalogAttributes")
                        .HasForeignKey("CatalogItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogCategory", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogItem", "CatalogItem")
                        .WithMany("CatalogCategories")
                        .HasForeignKey("CatalogItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ApplicationCore.Entities.Category", "Category")
                        .WithMany("CatalogCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogIllustration", b =>
                {
                    b.HasOne("ApplicationCore.Entities.IllustrationType", "IllustrationType")
                        .WithMany()
                        .HasForeignKey("IllustrationTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogItem", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogIllustration", "CatalogIllustration")
                        .WithMany()
                        .HasForeignKey("CatalogIllustrationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ApplicationCore.Entities.CatalogType", "CatalogType")
                        .WithMany("CatalogItems")
                        .HasForeignKey("CatalogTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogPicture", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogItem", "CatalogItem")
                        .WithMany("CatalogPictures")
                        .HasForeignKey("CatalogItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogReference", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogItem", "CatalogItem")
                        .WithMany("CatalogReferences")
                        .HasForeignKey("CatalogItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ApplicationCore.Entities.CatalogItem", "ReferenceCatalogItem")
                        .WithMany()
                        .HasForeignKey("ReferenceCatalogItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogTypeCategory", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogType", "CatalogType")
                        .WithMany("Categories")
                        .HasForeignKey("CatalogTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ApplicationCore.Entities.Category", "Category")
                        .WithMany("CatalogTypes")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ApplicationCore.Entities.Category", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Category", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CustomizeOrder", b =>
                {
                    b.OwnsOne("ApplicationCore.Entities.OrderAggregate.CatalogItemOrdered", "ItemOrdered", b1 =>
                        {
                            b1.Property<int>("CustomizeOrderId");

                            b1.Property<int>("CatalogItemId");

                            b1.Property<string>("PictureUri");

                            b1.Property<string>("ProductName");

                            b1.HasKey("CustomizeOrderId");

                            b1.ToTable("CustomizeOrder");

                            b1.HasOne("ApplicationCore.Entities.CustomizeOrder")
                                .WithOne("ItemOrdered")
                                .HasForeignKey("ApplicationCore.Entities.OrderAggregate.CatalogItemOrdered", "CustomizeOrderId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("ApplicationCore.Entities.FileDetail", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogType", "CatalogType")
                        .WithMany("PictureTextHelpers")
                        .HasForeignKey("CatalogTypeId");
                });

            modelBuilder.Entity("ApplicationCore.Entities.OrderAggregate.Order", b =>
                {
                    b.OwnsOne("ApplicationCore.Entities.OrderAggregate.Address", "BillingToAddress", b1 =>
                        {
                            b1.Property<int>("OrderId");

                            b1.Property<string>("City");

                            b1.Property<string>("Country");

                            b1.Property<string>("Name");

                            b1.Property<string>("PostalCode");

                            b1.Property<string>("Street");

                            b1.HasKey("OrderId");

                            b1.ToTable("Orders");

                            b1.HasOne("ApplicationCore.Entities.OrderAggregate.Order")
                                .WithOne("BillingToAddress")
                                .HasForeignKey("ApplicationCore.Entities.OrderAggregate.Address", "OrderId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });

                    b.OwnsOne("ApplicationCore.Entities.OrderAggregate.Address", "ShipToAddress", b1 =>
                        {
                            b1.Property<int>("OrderId");

                            b1.Property<string>("City");

                            b1.Property<string>("Country");

                            b1.Property<string>("Name");

                            b1.Property<string>("PostalCode");

                            b1.Property<string>("Street");

                            b1.HasKey("OrderId");

                            b1.ToTable("Orders");

                            b1.HasOne("ApplicationCore.Entities.OrderAggregate.Order")
                                .WithOne("ShipToAddress")
                                .HasForeignKey("ApplicationCore.Entities.OrderAggregate.Address", "OrderId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("ApplicationCore.Entities.OrderAggregate.OrderItem", b =>
                {
                    b.HasOne("ApplicationCore.Entities.OrderAggregate.Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId");

                    b.OwnsOne("ApplicationCore.Entities.OrderAggregate.CustomizeItemOrdered", "CustomizeItem", b1 =>
                        {
                            b1.Property<int>("OrderItemId");

                            b1.Property<int?>("CatalogTypeId");

                            b1.Property<string>("Colors");

                            b1.Property<string>("Description");

                            b1.Property<string>("Name");

                            b1.Property<string>("PictureUri");

                            b1.Property<string>("ProductName");

                            b1.HasKey("OrderItemId");

                            b1.ToTable("OrderItems");

                            b1.HasOne("ApplicationCore.Entities.OrderAggregate.OrderItem")
                                .WithOne("CustomizeItem")
                                .HasForeignKey("ApplicationCore.Entities.OrderAggregate.CustomizeItemOrdered", "OrderItemId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });

                    b.OwnsOne("ApplicationCore.Entities.OrderAggregate.CatalogItemOrdered", "ItemOrdered", b1 =>
                        {
                            b1.Property<int>("OrderItemId");

                            b1.Property<int>("CatalogItemId");

                            b1.Property<string>("PictureUri");

                            b1.Property<string>("ProductName");

                            b1.HasKey("OrderItemId");

                            b1.ToTable("OrderItems");

                            b1.HasOne("ApplicationCore.Entities.OrderAggregate.OrderItem")
                                .WithOne("ItemOrdered")
                                .HasForeignKey("ApplicationCore.Entities.OrderAggregate.CatalogItemOrdered", "OrderItemId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("ApplicationCore.Entities.ShopConfigDetail", b =>
                {
                    b.HasOne("ApplicationCore.Entities.ShopConfig", "ShopConfig")
                        .WithMany("Details")
                        .HasForeignKey("ShopConfigId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
