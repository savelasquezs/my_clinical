using Clinica_Herramientas_2.Domain.Ports;
using Clinica_Herramientas_2.Domain.Model;


namespace Clinica_Herramientas_2.Domain.Services
{
    public class CreatePatient(IPatientPort patientPort, IUserPort userPort)
    {
        private readonly IPatientPort patientPort = patientPort;
        private readonly IUserPort userPort = userPort;

        public void Create(User user, Patient patient)
        {
            if (user.Role != Role.Admin)
            {
                throw new Exception("Solo el administrador puede crear pacientes");
            }
            
            // Verificar si ya existe un paciente con ese DNI
            if (patientPort.FindByDocument(patient.Dni) != null)
            {
                throw new Exception("El paciente ya existe");
            }
            
            // Verificar si ya existe un usuario con ese DNI (comparten la misma PK en person)
            if (userPort.FindByDocument(patient.Dni) != null)
            {
                throw new Exception("Ya existe un usuario con este número de identificación. El DNI debe ser único en todo el sistema.");
            }
               
            patientPort.Save(patient);
        }
    }
}
