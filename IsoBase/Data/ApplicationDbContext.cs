using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IsoBase.Models;
using IsoBase.ViewModels;

namespace IsoBase.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        ///  ViewModel Mapping
        /// </summary>
        public DbSet<PlanUploadVM> PlanUploadVM { get; set; }
        public DbSet<CoverageUploadVM> CoverageUploadVM { get; set; }
        public DbSet<BenefitUploadVM> BenefitUploadVM { get; set; }
        public DbSet<ClientListVM> ClientListVM { get; set; }
        public DbSet<EnrollmentVM> EnrollmentVM { get; set; }
        public DbSet<KalenderVM> KalenderVM { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ClientModel>(b =>
            {
                b.HasKey(u => u.ID);
                b.Property(u => u.IsActive).HasDefaultValueSql("1");
                b.Property(u => u.DateCreate).HasDefaultValueSql("getdate()");
            });
            builder.Entity<ClientTypeModel>(b =>
            {
                b.HasKey(u => u.ID);
                b.Property(u => u.IsActive).HasDefaultValueSql("1");
                b.Property(u => u.DateCreate).HasDefaultValueSql("getdate()");
            });
            builder.Entity<PlanUploadModel>(b =>
            {
                b.HasKey(u => u.ID);
                b.Property(u => u.UploadDate).HasDefaultValueSql(" getdate() ");
            });
            builder.Entity<CoverageUploadModel>(b =>
            {
                b.HasKey(u => u.ID);
                b.Property(u => u.UploadDate).HasDefaultValueSql(" getdate() ");
            });
            builder.Entity<BenefitUploadModel>(b =>
            {
                b.HasKey(u => u.ID);
                b.Property(u => u.UploadDate).HasDefaultValueSql(" getdate() ");
            });
            builder.Entity<PicCodeModel>(b =>
            {
                b.HasKey(u => u.ID);
                b.HasIndex(u => u.PicDesc).IsUnique();
                b.Property(u => u.DateCreate).HasDefaultValueSql(" getdate() ");
            });
            builder.Entity<EnrollmentHdrModel>(b =>
            {
                b.HasKey(u => u.ID);
                b.Property(u => u.EnrollDate).HasDefaultValueSql(" getdate() ");
            });
            base.OnModelCreating(builder);
        }

        /// <summary>
        /// Table Staging Upload
        /// </summary>
        public DbSet<PlanUploadModel> PlanUploadModel { get; set; }
        public DbSet<CoverageUploadModel> CoverageUploadModel { get; set; }
        public DbSet<BenefitUploadModel> BenefitUploadModel { get; set; }


        /// <summary>
        /// Table Production Non Staging
        /// </summary>
        public DbSet<BenefitCodesModel> BenefitCodesModel { get; set; }
        public DbSet<ClientModel> ClientModel { get; set; }
        public DbSet<ClientTypeModel> ClientTypeModel { get; set; }
        public DbSet<KalenderOperationalModel> KalenderOperationalModel { get; set; }
        public DbSet<LimitCodesModel> LimitCodesModel { get; set; }
        public DbSet<CoverageCodesModel> CoverageCodesModel { get; set; }
        public DbSet<FrequencyCodesModel> FrequencyCodesModel { get; set; }
        public DbSet<PlanLimitModel> PlanLimitModel { get; set; }
        public DbSet<CoverageLimitModel> CoverageLimitModel { get; set; }
        public DbSet<BenefitModel> BenefitModel { get; set; }
        public DbSet<BenefitLimitValueModel> BenefitLimitValueModel { get; set; }
        public DbSet<MemberModel> MemberModel { get; set; }
        public DbSet<PicCodeModel> PicCodeModel { get; set; }
        public DbSet<ClientPicModel> ClientPicModel { get; set; }
        public DbSet<ClientAccBankModel> ClientAccBankModel { get; set; }
        public DbSet<EnrollmentHdrModel> EnrollmentHdrModel { get; set; }
    }
}
