using ClinicApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.Entity<Patient>().Property(p => p.FirstName).HasMaxLength(100).IsRequired();
        b.Entity<Patient>().Property(p => p.LastName).HasMaxLength(100).IsRequired();
        b.Entity<Patient>().HasIndex(p => new { p.FirstName, p.LastName });
    }
}