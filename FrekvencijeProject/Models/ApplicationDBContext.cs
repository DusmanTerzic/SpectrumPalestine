using FrekvencijeProject.Models.Application;
using FrekvencijeProject.Models.Document;
using FrekvencijeProject.Models.Standard;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models
{
    public class ApplicationDBContext : DbContext
    {

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<RootApplicationDB> RootApplicationDB { get; set; }

        public DbSet<RootApplicationTermsDB> RootApplicationTermsDB { get; set; }
        
        public DbSet<ApplicationRangeDB> ApplicationRange { get; set; }

        public DbSet<ApplicationDB> Application { get; set; }

        public DbSet<DocumentsDb> DocumentsDb { get; set; }

        public DbSet<StandardsDb> StandardsDb { get; set; }


        public DbSet<StandardsDbCopy> StandardsDbCopy { get; set; }

        public DbSet<StandardsDbEN> StandardsDbEN { get; set; }
        
        public DbSet<DocumentsDbCopy> DocumentsDbCopy { get; set; }

    }
}
