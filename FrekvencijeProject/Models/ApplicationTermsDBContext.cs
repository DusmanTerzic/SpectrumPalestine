using FrekvencijeProject.Models.ApplicationTerms;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models
{
    public class ApplicationTermsDBContext : DbContext
    {

        public ApplicationTermsDBContext(DbContextOptions<ApplicationTermsDBContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<ApplicationTermsDB> RootApplicationTermsDB { get; set; }
    }
}
