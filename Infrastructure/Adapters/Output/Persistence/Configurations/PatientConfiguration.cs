using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinica_Herramientas_2.Domain.Model;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Output.Persistence.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("patient");
            
            builder.Property(p => p.Gender).HasConversion<string>().IsRequired();
            
            // Owned Entity: EmergencyContact
            builder.OwnsOne(p => p.EmergencyContact, ec =>
            {
                ec.Property(e => e.Firtname).HasColumnName("emergency_contact_first_name").HasMaxLength(100).IsRequired();
                ec.Property(e => e.Lastname).HasColumnName("emergency_contact_last_name").HasMaxLength(100).IsRequired();
                ec.Property(e => e.Relationship).HasColumnName("emergency_contact_relationship").HasMaxLength(50).IsRequired();
                ec.Property(e => e.PhoneNumber).HasColumnName("emergency_contact_phone").HasMaxLength(20).IsRequired();
            });
            
            // Owned Entity: HealthInsurance
            builder.OwnsOne(p => p.Insurance, hi =>
            {
                hi.Property(h => h.CompanyName).HasColumnName("health_insurance_company").HasMaxLength(200).IsRequired();
                hi.Property(h => h.PolicyNumber).HasColumnName("health_insurance_policy").HasMaxLength(100).IsRequired();
                // IsActive es una propiedad calculada basada en ExpirationDate, no se almacena en BD
                hi.Ignore(h => h.IsActive);
                hi.Property(h => h.ExpirationDate).HasColumnName("health_insurance_expiration").IsRequired();
            });
            
            // Relaciones
            builder.HasMany(p => p.Appointments)
                   .WithOne(a => a.Patient1)
                   .HasForeignKey("patient_dni")
                   .OnDelete(DeleteBehavior.Restrict);
                   
            builder.HasMany(p => p.MedicalRecords)
                   .WithOne(mr => mr.Patient)
                   .HasForeignKey("patient_dni")
                   .OnDelete(DeleteBehavior.Restrict);
                   
            builder.HasMany(p => p.NurseVisits)
                   .WithOne(nv => nv.Patient)
                   .HasForeignKey("patient_dni")
                   .OnDelete(DeleteBehavior.Restrict);
                   
            builder.HasMany(p => p.Invoices)
                   .WithOne(i => i.Patient)
                   .HasForeignKey("patient_dni")
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
