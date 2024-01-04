using Microsoft.EntityFrameworkCore;
using ModernRecrut.Postulations.API.Models;

namespace ModernRecrut.Postulations.API.Data
{
    public class PostulationContext : DbContext
    {
        public PostulationContext(DbContextOptions<PostulationContext> options) : base(options)
        {
        }

        public DbSet<Postulation> Postulations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Postulation>().ToTable("Postulation");
        }
    }
}
