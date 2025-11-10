using Clinica_Herramientas_2.Application.Adapters.Input.Builders;
using Clinica_Herramientas_2.Application.UseCases;
using Clinica_Herramientas_2.Domain.Model;


namespace Clinica_Herramientas_2.Application.Adapters.Input
{
    public class AdminInputs
    {
        private PatientBuilder patientBuilder;
        private AppointmentBuilder appointmentBuilder;
        private InvoiceBuilder invoiceBuilder;
        private AdminUseCase adminUseCase;
        
        public AdminInputs(
            PatientBuilder patientBuilder,
            AppointmentBuilder appointmentBuilder,
            InvoiceBuilder invoiceBuilder,
            AdminUseCase adminUseCase)
        {
            this.patientBuilder = patientBuilder;
            this.appointmentBuilder = appointmentBuilder;
            this.invoiceBuilder = invoiceBuilder;
            this.adminUseCase = adminUseCase;
        }
        
        public void CreatePatient(string fullname, string dni, string email, string phonenumber, DateOnly birthdate, string address, Gender gender, string emergencyFirstName, string emergencyLastName, string emergencyRelationship, string emergencyPhone, string insuranceCompanyName, string insurancePolicyNumber, bool insuranceIsActive, DateTime insuranceExpirationDate)
        {
            // Usar builder para crear paciente (él maneja internamente EmergencyContact y HealthInsurance con validación)
            var patient = patientBuilder.Create(fullname, dni, email, phonenumber, birthdate, address, gender, emergencyFirstName, emergencyLastName, emergencyRelationship, emergencyPhone, insuranceCompanyName, insurancePolicyNumber, insuranceIsActive, insuranceExpirationDate);
            
            // Llamar a adminUseCase.CreateNewPatient() pasando el paciente ya creado
            adminUseCase.CreateNewPatient(patient);
        }
        
        public void UpdatePatient(Patient patient, string email, string phone, string address,
            string emergencyFirstName, string emergencyLastName, string emergencyRelationship, string emergencyPhone,
            string insuranceCompanyName, string insurancePolicyNumber, bool insuranceIsActive, DateTime insuranceExpirationDate)
        {
            adminUseCase.UpdateExistingPatient(patient, email, phone, address, 
                emergencyFirstName, emergencyLastName, emergencyRelationship, emergencyPhone,
                insuranceCompanyName, insurancePolicyNumber, insuranceIsActive, insuranceExpirationDate);
        }
        
        public void CreateAppointment(int id, string patientDni, DateTime date)
        {
            adminUseCase.CreateNewAppointment(id, patientDni, date);
        }
        
        public Invoice CreateInvoice(int invoiceNumber, string patientDni, string doctorDni, List<int> orderNumbers, DateTime invoiceDate)
        {
            return adminUseCase.CreateNewInvoice(invoiceNumber, patientDni, doctorDni, orderNumbers, invoiceDate);
        }
        
        public Patient GetPatientByDni(string dni)
        {
            return adminUseCase.GetPatientByDni(dni);
        }
        
        public List<Appointment> GetPatientAppointments(string patientDni)
        {
            return adminUseCase.GetPatientAppointments(patientDni);
        }
        
        public List<Order> GetPatientOrders(string patientDni)
        {
            return adminUseCase.GetPatientOrders(patientDni);
        }
        
        public List<Patient> GetAllPatients()
        {
            return adminUseCase.GetAllPatients();
        }
        
        public void SetCurrentUser(User user)
        {
            adminUseCase.SetCurrentUser(user);
        }
    }
}
