﻿using DiamondLuxurySolution.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.ProductId);
            builder.Property(p => p.ProductId).IsRequired().HasMaxLength(15);
            builder.Property(p => p.ProductName).IsRequired().HasMaxLength(250);
            builder.Property(p => p.Description);
            builder.Property(p => p.ProductThumbnail).IsRequired().HasMaxLength(50);
            builder.Property(p => p.IsHome).IsRequired();
            builder.Property(p => p.IsSale).IsRequired();
            builder.Property(p => p.DateCreate).IsRequired();
            builder.Property(p => p.DateModified).IsRequired();
            builder.Property(p => p.ProcessingPrice).IsRequired().HasColumnType("decimal(10, 2)");
            builder.Property(p => p.OriginalPrice).IsRequired().HasColumnType("decimal(10, 2)");
            builder.Property(p => p.SellingPrice).IsRequired().HasColumnType("decimal(10, 2)");
            builder.Property(p => p.SellingCount).IsRequired();
            builder.Property(p => p.PercentSale).IsRequired();

            builder.HasOne(p => p.Material).WithMany(x => x.Products).IsRequired();
            builder.HasOne(p => p.Gem).WithMany(x => x.Products).IsRequired();
            builder.HasOne(p => p.Category)
              .WithMany(c => c.Products)
              .HasForeignKey(p => p.CategoryId);

            builder.HasOne(p => p.InspectionCertificate)
              .WithMany(i => i.Products)
              .HasForeignKey(p => p.InspectionCertificateId);
        }
    }

}
