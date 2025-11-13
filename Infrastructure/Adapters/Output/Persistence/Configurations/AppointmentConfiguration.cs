using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinica_Herramientas_2.Domain.Model;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Output.Persistence.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("appointment");
            builder.HasKey(a => a.Id1);
            
            builder.Property(a => a.Id1).ValueGeneratedOnAdd();
            builder.Property(a => a.Date1).IsRequired();
            builder.Property(a => a.IsAccepted1)
                   .HasColumnName("is_accepted")
                   .IsRequired()
                   .HasDefaultValue(false);
            
            builder.HasOne(a => a.Patient1)
                   .WithMany(p => p.Appointments)
                   .HasForeignKey("patient_dni")
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
