using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
