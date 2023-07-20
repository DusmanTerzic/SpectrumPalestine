using FrekvencijeProject.Models.RightOfUse;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models
{
    public class ImportTempRightOfUseDBContext : DbContext
    {
        public ImportTempRightOfUseDBContext(DbContextOptions<ImportTempRightOfUseDBContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }


        public DbSet<ImportTempRightOfUse> ImportTempRightOfUse { get; set; }
    }
}
