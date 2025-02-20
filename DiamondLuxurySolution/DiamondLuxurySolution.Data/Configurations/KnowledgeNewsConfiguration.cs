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
    public class KnowledgeNewsConfiguration : IEntityTypeConfiguration<KnowledgeNews>
    {
        public void Configure(EntityTypeBuilder<KnowledgeNews> builder)
        {
            builder.ToTable("KnowledgeNews");

            builder.HasKey(k => k.KnowledgeNewsId);
            builder.Property(k => k.KnowledgeNewsId).IsRequired().ValueGeneratedOnAdd();
            builder.Property(k => k.KnowledgeNewsName).HasMaxLength(250);
            builder.Property(k => k.Thumnail).HasMaxLength(int.MaxValue);
            builder.Property(k => k.Description).HasMaxLength(int.MaxValue);
            builder.Property(k => k.DateCreated).IsRequired();
            builder.Property(k => k.DateModified).IsRequired();
            builder.HasOne(k => k.KnowledgeNewCatagory).WithMany(k => k.KnowledgeNews)
                .HasForeignKey(k => k.KnowledgeNewCatagoryId).IsRequired();
            builder.HasOne(k => k.Writer)
                   .WithMany(x => x.KnowledgeNews)
                   .HasForeignKey(k => k.WriterId).IsRequired();
        }
    }

}
