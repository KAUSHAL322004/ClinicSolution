using Microsoft.EntityFrameworkCore;
using ClinicApp.API.Models;
using ClinicApp.Data.Entities;

namespace ClinicApp.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {

        }

        public DbSet<Models.Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<User> Users { get; set; }
    }

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }

}
