using FrekvencijeProject.Models.Standard;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models
{
    public class StandardsDbContext : DbContext
    {
        public StandardsDbContext(DbContextOptions<StandardsDbContext> options)
             : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //   builder.Entity<AllocationDb>()
            //.HasKey(e => new { e.Id}).ValueGeneratedOnAdd(); ;
            base.OnModelCreating(builder);

        }

        public DbSet<StandardsDb> StandardsDb { get; set; }
    }
}
