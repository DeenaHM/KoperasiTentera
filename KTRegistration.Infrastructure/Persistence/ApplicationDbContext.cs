using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;

namespace KTRegistration.Infrastructure.Persistence;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);

    }
    public DbSet<VerificationCode> VerificationCodes { get; set; }
}
