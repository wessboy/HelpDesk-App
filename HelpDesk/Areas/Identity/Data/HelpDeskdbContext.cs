using HelpDesk.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace HelpDesk.Areas.Identity.Data;

public class HelpDeskdbContext : IdentityDbContext<User>
{
    public HelpDeskdbContext(DbContextOptions<HelpDeskdbContext> options)
        : base(options)
    {
    }  
    public DbSet<Complaint> Complaints { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<User>()
           .HasMany(u => u.Complaints)
           .WithOne(c => c.User)
           .HasForeignKey(c => c.UserId)
           .OnDelete(DeleteBehavior.Cascade);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
