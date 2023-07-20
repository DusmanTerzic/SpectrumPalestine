using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models
{
    public class UsersDBContext : DbContext
    {

        public UsersDBContext(DbContextOptions<UsersDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AspNetUserRoles>()
           .HasKey(e => new { e.UserId, e.RoleId });
            
            base.OnModelCreating(builder);

        }

        public DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public DbSet<AspNetRoles> AspNetRoles { get; set; }
        public DbSet<AspNetUsers> AspNetUsers { get; set; }
    }
}
