using Microsoft.EntityFrameworkCore;
using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Domain.Ports;

namespace Clinica_Herramientas_2.Infrastructure.Adapters.Output.Persistence
{
    internal class PostgresAppointmentPort : IAppointmentPort
    {
        private readonly ClinicaDbContext context;
        
        public PostgresAppointmentPort(ClinicaDbContext context)
        {
            this.context = context;
        }
        
        public Appointment? FindById(int id)
        {
            return context.Appointments
                .Include(a => a.Patient1)
                .FirstOrDefault(a => a.Id1 == id);
        }
        
        public void Save(Appointment appointment)
        {
            context.Appointments.Add(appointment);
            context.SaveChanges();
        }
        
        public void Update(Appointment appointment)
        {
            context.Appointments.Update(appointment);
            context.SaveChanges();
        }
        
        public void Delete(Appointment appointment)
        {
            context.Appointments.Remove(appointment);
            context.SaveChanges();
        }
        
        public List<Appointment> FindAll()
        {
            return context.Appointments
                .Include(a => a.Patient1)
                .ToList();
        }
        
        public List<Appointment> FindByPatient(string patientDni)
        {
            return context.Appointments
                .Include(a => a.Patient1)
                .Where(a => a.Patient1.Dni == patientDni)
                .ToList();
        }
        
        public List<Appointment> FindByDateRange(DateTime startDate, DateTime endDate)
        {
            return context.Appointments
                .Include(a => a.Patient1)
                .Where(a => a.Date1 >= startDate && a.Date1 <= endDate)
                .ToList();
        }
        
        public void SaveAppointment(Appointment appointment)
        {
            Save(appointment);
        }
        
        public List<Appointment> FindByPatientDni(string dni)
        {
            return FindByPatient(dni);
        }
        
        public void UpdateAppointment(Appointment appointment)
        {
            Update(appointment);
        }
        
        public void DeleteAppointment(Appointment appointment)
        {
            Delete(appointment);
        }

        public List<Appointment> FindAvailableAppointments()
        {
            return context.Appointments
                .Include(a => a.Patient1)
                .Where(a => !a.IsAccepted1)
                .OrderBy(a => a.Date1)
                .ToList();
        }
    }
}
