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
    public class GemConfiguration : IEntityTypeConfiguration<Gem>
    {
        public void Configure(EntityTypeBuilder<Gem> builder)
        {
            builder.ToTable("Gems");

            builder.HasKey(g => g.GemId);
            builder.Property(c => c.GemName).HasMaxLength(250);
            builder.Property(g => g.ProportionImage);
            builder.Property(g => g.Symetry).HasMaxLength(250);
            builder.Property(g => g.Polish).HasMaxLength(250);
            builder.Property(g => g.IsOrigin).IsRequired();
            builder.Property(g => g.GemImage).HasMaxLength(int.MaxValue);
            builder.Property(g => g.Fluoresence).IsRequired();
            builder.Property(g => g.Active).IsRequired();

            builder.HasOne(x => x.InspectionCertificate).WithOne(x => x.Gem).HasForeignKey<Gem>(g => g.InspectionCertificateId);
			builder.HasOne(x => x.GemPriceList).WithMany(x => x.Gems).HasForeignKey(g => g.GemPriceListId);
		}
    }

}
