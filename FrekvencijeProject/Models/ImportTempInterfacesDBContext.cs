using FrekvencijeProject.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models
{
    public class ImportTempInterfacesDBContext : DbContext
    {
        public ImportTempInterfacesDBContext(DbContextOptions<ImportTempInterfacesDBContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<ImportTempInterfaces> ImportTempInterfaces { get; set; }
    }
}
