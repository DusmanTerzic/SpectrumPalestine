using FrekvencijeProject.JSON.Allocations;
using FrekvencijeProject.Models.Allocation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models
{
    public class AllocationDBContext : DbContext
    {

        public AllocationDBContext(DbContextOptions<AllocationDBContext> options)
             : base(options)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
         //   builder.Entity<AllocationDb>()
         //.HasKey(e => new { e.Id}).ValueGeneratedOnAdd(); ;
            base.OnModelCreating(builder);

        }

        public DbSet<RootAllocationDB> RootAllocationDB { get; set; }
        public DbSet<AllocationRangeDb> AllocationRangeDb { get; set; }
        public DbSet<AllocationDb> AllocationDb { get; set; }
        public DbSet<AllocationTermDb> AllocationTermDb { get; set; }

        public DbSet<FootnoteAllocation> FootnoteAllocation { get; set; }

        public DbSet<Footnote_description> Footnote_description { get; set; }

        public DbSet<AllocationTermDbInsert> AllocationTermDbInsert { get; set; }
        

    }
}
