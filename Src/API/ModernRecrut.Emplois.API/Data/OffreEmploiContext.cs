using ModernRecrut.Emplois.API.Entites;
using Microsoft.EntityFrameworkCore;

namespace ModernRecrut.Emplois.API.Data
{
    public class OffreEmploiContext : DbContext
    {
        public OffreEmploiContext(DbContextOptions<OffreEmploiContext> options) : base(options)
        {
        }

        public DbSet<OffreEmploi> OffresEmplois { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OffreEmploi>().ToTable("OffreEmploi");
        }
    }
}
