using FrekvencijeProject.Models.ImportTempTables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models
{
    public class ImportTempTableContext : DbContext
    {
        
        public ImportTempTableContext(DbContextOptions<ImportTempTableContext> options)
             : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            base.OnModelCreating(builder);

            builder.Entity<storedTableApplicationProcedure>()
           .HasKey("ApplicationId");

        }

        public DbSet<ImportTempTable> ImportTempTable { get; set; }

        public DbSet<storedTableApplicationProcedure> storedTableApplicationProcedure { get; set; }
    }
}
