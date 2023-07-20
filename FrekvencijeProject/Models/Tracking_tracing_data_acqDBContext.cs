using Microsoft.EntityFrameworkCore;
using FrekvencijeProject.Models.Tracking_tracing_data_acq;
namespace FrekvencijeProject.Models
{
    public class Tracking_tracing_data_acqDBContext : DbContext
    {

        public Tracking_tracing_data_acqDBContext(DbContextOptions<Tracking_tracing_data_acqDBContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            base.OnModelCreating(builder);

        }

        public DbSet<Tracking_tracing_data_acq.Tracking_tracing_data_acq> Tracking_tracing_data_acq { get; set; }


        public DbSet<Tracking_tracing_data_acq.TTT> TTT { get; set; }
    }
}
