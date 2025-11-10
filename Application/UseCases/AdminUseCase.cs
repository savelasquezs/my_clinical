using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Domain.Services;
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
        private CreateInvoice createInvoice;

        internal CreatePatient CreatePatient { get => createPatient; set => createPatient = value; }
        internal UpdatePatient UpdatePatient { get => updatePatient; set => updatePatient = value; }
        internal CreateAppointment CreateAppointment { get => createAppointment; set => createAppointment = value; }
        internal CreateInvoice CreateInvoice { get => createInvoice; set => createInvoice = value; }
        internal User CurrentUser { get => currentUser; set => currentUser = value; }

        public AdminUseCase(CreatePatient createPatient, UpdatePatient updatePatient, CreateAppointment createAppointment, CreateInvoice createInvoice, ViewPatientInformation viewPatientInformation)
            : base(viewPatientInformation)
        {
            this.createPatient = createPatient;
            this.updatePatient = updatePatient;
            this.createAppointment = createAppointment;
            this.createInvoice = createInvoice;
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

            var appointment = new Appointment(id, null!, date); // El paciente se valida en el servicio
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

    }
}
