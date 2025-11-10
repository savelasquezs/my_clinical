using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Domain.Services;
using Clinica_Herramientas_2.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinica_Herramientas_2.Application.UseCases
{
    public class AdminUseCase : BaseUseCase
    {
        private CreatePatient createPatient;
        private UpdatePatient updatePatient;
        private CreateAppointment createAppointment;
        private UpdateAppointment updateAppointment;
        private DeleteAppointment deleteAppointment;
        private CreateInvoice createInvoice;
        private IAppointmentPort appointmentPort;

        internal CreatePatient CreatePatient { get => createPatient; set => createPatient = value; }
        internal UpdatePatient UpdatePatient { get => updatePatient; set => updatePatient = value; }
        internal CreateAppointment CreateAppointment { get => createAppointment; set => createAppointment = value; }
        internal UpdateAppointment UpdateAppointment { get => updateAppointment; set => updateAppointment = value; }
        internal DeleteAppointment DeleteAppointment { get => deleteAppointment; set => deleteAppointment = value; }
        internal CreateInvoice CreateInvoice { get => createInvoice; set => createInvoice = value; }
        internal User CurrentUser { get => currentUser; set => currentUser = value; }

        public AdminUseCase(CreatePatient createPatient, UpdatePatient updatePatient, CreateAppointment createAppointment, UpdateAppointment updateAppointment, DeleteAppointment deleteAppointment, CreateInvoice createInvoice, ViewPatientInformation viewPatientInformation, IAppointmentPort appointmentPort)
            : base(viewPatientInformation)
        {
            this.createPatient = createPatient;
            this.updatePatient = updatePatient;
            this.createAppointment = createAppointment;
            this.updateAppointment = updateAppointment;
            this.deleteAppointment = deleteAppointment;
            this.createInvoice = createInvoice;
            this.appointmentPort = appointmentPort;
        }

        public void SetCurrentUser(User user)
        {
            if (user.Role != Role.Admin)
            {
                throw new Exception("Solo usuarios administrativos pueden acceder a esta funcionalidad");
            }
            this.CurrentUser = user;
        }

        public void CreateNewPatient(Patient patient)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario administrativo válido");
            }

            createPatient.Create(this.CurrentUser, patient);
        }

        public void UpdateExistingPatient(Patient patient, string email, string phone, string address, 
            string emergencyFirstName, string emergencyLastName, string emergencyRelationship, string emergencyPhone,
            string insuranceCompanyName, string insurancePolicyNumber, bool insuranceIsActive, DateTime insuranceExpirationDate)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario administrativo válido");
            }

            // Actualizar los campos del paciente
            patient.SetEmail(email);
            patient.SetPhone(phone);
            patient.SetAddress(address);
            patient.UpdateEmergencyContact(emergencyFirstName, emergencyLastName, emergencyRelationship, emergencyPhone);
            patient.UpdateInsurance(insuranceCompanyName, insurancePolicyNumber, insuranceIsActive, insuranceExpirationDate);

            updatePatient.Update(this.CurrentUser, patient);
        }

        public void CreateNewAppointment(int id, string patientDni, DateTime date)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario administrativo válido");
            }

            // Obtener el paciente primero (GetPatientByDni lanza excepción si no existe)
            var patient = GetPatientByDni(patientDni);

            // Crear la cita con el paciente real
            var appointment = new Appointment(id, patient, date);
            createAppointment.Create(appointment, patientDni);
        }

        public Invoice CreateNewInvoice(int invoiceNumber, string patientDni, string doctorDni, List<int> orderNumbers, DateTime invoiceDate)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario administrativo válido");
            }

            return createInvoice.Create(invoiceNumber, patientDni, doctorDni, orderNumbers, invoiceDate);
        }

        public List<Appointment> GetAllAppointments()
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario administrativo válido");
            }

            return appointmentPort.FindAll();
        }

        public void UpdateExistingAppointment(int appointmentId, DateTime newDate)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario administrativo válido");
            }
            
            var appointment = appointmentPort.FindById(appointmentId);
            if (appointment == null)
            {
                throw new Exception("La cita no existe");
            }
            
            updateAppointment.Update(appointment, newDate);
        }

        public void CancelAppointment(int appointmentId)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario administrativo válido");
            }
            
            var appointment = appointmentPort.FindById(appointmentId);
            if (appointment == null)
            {
                throw new Exception("La cita no existe");
            }
            
            deleteAppointment.Delete(appointment);
        }

    }
}
