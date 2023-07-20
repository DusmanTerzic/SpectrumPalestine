using FrekvencijeProject.Models.Document;
using FrekvencijeProject.Models.SRD;
using FrekvencijeProject.Models.Standard;
using Microsoft.EntityFrameworkCore;

namespace FrekvencijeProject.Models
{
    public class SRDContext : DbContext
    {
        public SRDContext(DbContextOptions<SRDContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<SRDDb>().ToTable("SRD");
        }

        public DbSet<DocumentsDb> DocumentsDb { get; set; }

        public DbSet<StandardsDb> StandardsDb { get; set; }
        public DbSet<SRDDb> SRDDb { get; set; }

    }
}
