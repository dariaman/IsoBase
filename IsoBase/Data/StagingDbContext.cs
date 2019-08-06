using System;
using System.Collections.Generic;
using System.Text;
using IsoBase.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IsoBase.Data
{
    public class StagingDbContext : IdentityDbContext
    {
        public StagingDbContext(DbContextOptions<StagingDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<PlanUploadModel>(b =>
            //{
            //    b.HasKey(u => u.ID);
            //    b.Property(u => u.RecType).HasDefaultValue("1");
            //    b.Property(u => u.UploadDate).HasDefaultValueSql(" getdate() ");
            //});
            //builder.Entity<CoverageUploadModel>(b =>
            //{
            //    b.HasKey(u => u.ID);
            //    b.Property(u => u.RecType).HasDefaultValue("2");
            //    b.Property(u => u.UploadDate).HasDefaultValueSql(" getdate() ");
            //});
            //builder.Entity<BenefitUploadModel>(b =>
            //{
            //    b.HasKey(u => u.ID);
            //    b.Property(u => u.RecType).HasDefaultValue("3");
            //    b.Property(u => u.UploadDate).HasDefaultValueSql(" getdate() ");
            //});
            base.OnModelCreating(builder);
        }

        public DbSet<PlanUploadModel> PlanUploadModel { get; set; }
        public DbSet<CoverageUploadModel> CoverageUploadModel { get; set; }
        public DbSet<BenefitUploadModel> BenefitUploadModel { get; set; }
    }
}
