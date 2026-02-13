using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using task.Model;

namespace task.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Cource> Cources => Set<Cource>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Cource)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.CourceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

