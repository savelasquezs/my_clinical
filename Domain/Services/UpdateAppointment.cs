using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Domain.Ports;
using System;

namespace Clinica_Herramientas_2.Domain.Services
{
    public class UpdateAppointment(IAppointmentPort appointmentPort)
    {
        private readonly IAppointmentPort appointmentPort = appointmentPort;

        public void Update(Appointment appointment, DateTime newDate)
        {
            if (newDate < DateTime.Now)
            {
                throw new Exception("La fecha de la cita no puede ser en el pasado");
            }
            appointment.UpdateDate(newDate);
            appointmentPort.UpdateAppointment(appointment);
        }
    }
}

