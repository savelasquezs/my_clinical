using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Domain.Ports;

namespace Clinica_Herramientas_2.Domain.Services
{
    public class DeleteAppointment(IAppointmentPort appointmentPort)
    {
        private readonly IAppointmentPort appointmentPort = appointmentPort;

        public void Delete(Appointment appointment)
        {
            appointmentPort.DeleteAppointment(appointment);
        }
    }
}

