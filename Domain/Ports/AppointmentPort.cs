using Clinica_Herramientas_2.Domain.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinica_Herramientas_2.Domain.Ports
{
    public interface IAppointmentPort
    {

        public void SaveAppointment(Appointment appointment);
        public List<Appointment> FindByPatientDni(string dni);
        public List<Appointment> FindAll();
        public Appointment? FindById(int id);
        public void UpdateAppointment(Appointment appointment);
        public void DeleteAppointment(Appointment appointment);
        public List<Appointment> FindAvailableAppointments();

    }
}
