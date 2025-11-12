using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinica_Herramientas_2.Domain.Model;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Output.Persistence.Configurations
{
    public class PatientCareRecordConfiguration : IEntityTypeConfiguration<PatientCareRecord>
    {
        public void Configure(EntityTypeBuilder<PatientCareRecord> builder)
        {
            builder.ToTable("patient_care_record");
            builder.HasKey(pcr => pcr.Id);
            
            builder.Property(pcr => pcr.Id).ValueGeneratedOnAdd();
            
            // Mapear las propiedades usando sus backing fields automáticamente
            builder.Property(pcr => pcr.TestsPerformed)
                   .HasField("testsPerformed")
                   .UsePropertyAccessMode(PropertyAccessMode.Field)
                   .HasMaxLength(500);
            builder.Property(pcr => pcr.Notes)
                   .HasField("notes")
                   .UsePropertyAccessMode(PropertyAccessMode.Field)
                   .HasMaxLength(1000);
            builder.Property(pcr => pcr.PerformedAt)
                   .HasField("performedAt")
                   .UsePropertyAccessMode(PropertyAccessMode.Field)
                   .IsRequired();
            
            // Relación con OrderItem usando backing field
            builder.HasOne(pcr => pcr.OrderItem)
                   .WithMany()
                   .HasForeignKey("order_number", "item_number")
                   .OnDelete(DeleteBehavior.Restrict);
            
            builder.Navigation(pcr => pcr.OrderItem)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
            
            // TPT para subclases
            builder.UseTptMappingStrategy();
        }
    }
}
