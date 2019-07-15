using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IsoBase.Models;

namespace IsoBase.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ClientMasterModels>(b => {
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

        public DbSet<IsoBase.Models.ClientMasterModels> ClientMasterModels { get; set; }
        public DbSet<IsoBase.Models.ClientTypeModel> ClientTypeModel { get; set; }
    }
}
