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

        public DbQuery<ClientListVM> ClientListVM { get; set; }

        public DbSet<ClientMasterModel> ClientMasterModel { get; set; }
        public DbSet<ClientTypeModel> ClientTypeModel { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ClientMasterModel>(b => {
                b.HasKey(u => u.ClientID);
                b.Property(u => u.IsActive).HasDefaultValueSql("1");
                b.Property(u => u.DateCreate).HasDefaultValueSql("getdate()");
            });
            builder.Entity<ClientTypeModel>(b => {
                b.HasKey(u => u.ID);
                b.Property(u => u.IsActive).HasDefaultValueSql("1");
                b.Property(u => u.DateCreate).HasDefaultValueSql("getdate()");
            });
            base.OnModelCreating(builder);
        }
    }
}
