using Microsoft.EntityFrameworkCore;

namespace VirtualClinic.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options)
        {
        }

        // User Table
        public DbSet<User> Users { get; set; }

        // Provider Table
        public DbSet<Provider> Providers { get; set; }

        // Patient Table
        public DbSet<Patient> Patients { get; set; }

        // Appointment Table
        public DbSet<Appointment> Appointments { get; set; }
        
        // Medical Notes
        public DbSet<MedicalNote> MedicalNotes { get; set; }
        
        // Medications
        public DbSet<Medication> Medications { get; set; }
        public DbSet<ReportedMedication> ReportedMedications { get; set; }
        
        // Allergies
        public DbSet<Allergy> Allergies { get; set; }
        
        // Medical History
        public DbSet<MedicalHistory> MedicalHistories { get; set; }
        
    }
}