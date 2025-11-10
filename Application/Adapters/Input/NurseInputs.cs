using Clinica_Herramientas_2.Application.Adapters.Input.Builders;
using Clinica_Herramientas_2.Application.UseCases;
using Clinica_Herramientas_2.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinica_Herramientas_2.Application.Adapters.Input
{
    public class NurseInputs
    {
        private NurseVisitBuilder nurseVisitBuilder;
        private NurseUseCase nurseUseCase;
        
        public NurseInputs(
            NurseVisitBuilder nurseVisitBuilder,
            NurseUseCase nurseUseCase)
        {
            this.nurseVisitBuilder = nurseVisitBuilder;
            this.nurseUseCase = nurseUseCase;
        }
        
        public void CreateNurseVisit(OrderItem orderItem, string testsPerformed, string notes, DateTime performedAt, string bloodPressure, double temperature, int pulse, int oxygenLevel, List<AdministeredMedication> administeredMedications, DateTime visitTime, Patient patient)
        {
            nurseUseCase.CreateNewNurseVisit(orderItem, testsPerformed, notes, performedAt, bloodPressure, temperature, pulse, oxygenLevel, administeredMedications, visitTime, patient);
        }
        
        public void CreateAdministeredMedication(OrderItem orderItem, string testsPerformed, string notes, DateTime performedAt, Medication medication, string dose, string administrationRoute)
        {
            nurseUseCase.CreateAdministeredMedication(orderItem, testsPerformed, notes, performedAt, medication, dose, administrationRoute);
        }
        
        public VitalData CreateVitalData(string bloodPressure, double temperature, int pulse, int oxygenLevel)
        {
            return nurseUseCase.CreateVitalData(bloodPressure, temperature, pulse, oxygenLevel);
        }
        
        public Patient GetPatientByDni(string dni)
        {
            return nurseUseCase.GetPatientByDni(dni);
        }
        
        public List<Appointment> GetPatientAppointments(string patientDni)
        {
            return nurseUseCase.GetPatientAppointments(patientDni);
        }
        
        public List<Order> GetPatientOrders(string patientDni)
        {
            return nurseUseCase.GetPatientOrders(patientDni);
        }
        
        public void SetCurrentUser(User user)
        {
            nurseUseCase.SetCurrentUser(user);
        }
    }
}
