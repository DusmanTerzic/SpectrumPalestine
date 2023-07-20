using FrekvencijeProject.Models.AllocationTerms;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models
{
    public class AllocationTermsDBContext : DbContext
    {

        public AllocationTermsDBContext(DbContextOptions<AllocationTermsDBContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //   builder.Entity<AllocationDb>()
            //.HasKey(e => new { e.Id}).ValueGeneratedOnAdd(); ;
            base.OnModelCreating(builder);

        }

        public DbSet<AllocationTermDb> AllocationTermDb { get; set; }

        
    }
}
