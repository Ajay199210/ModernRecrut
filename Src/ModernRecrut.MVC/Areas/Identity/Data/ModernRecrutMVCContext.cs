using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModernRecrut.MVC.Models.User;

namespace ModernRecrut.MVC.Areas.Identity.Data;

public class ModernRecrutMVCContext : IdentityDbContext<Utilisateur>
{
    public ModernRecrutMVCContext(DbContextOptions<ModernRecrutMVCContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations afeter calling base.OnModelCreating(builder);
    }

    public DbSet<ModernRecrut.MVC.Models.Postulations.Postulation>? Postulation { get; set; }

    public DbSet<ModernRecrut.MVC.Models.Postulations.Note>? Note { get; set; }
}
