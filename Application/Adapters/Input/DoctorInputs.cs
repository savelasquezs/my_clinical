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
    public class DoctorInputs
    {
        private OrderBuilder orderBuilder;
        private MedicalRecordBuilder medicalRecordBuilder;
        private CreateOrderItemDTOBuilder createOrderItemDTOBuilder;
        private DoctorUseCase doctorUseCase;
        
        public DoctorInputs(
            OrderBuilder orderBuilder,
            MedicalRecordBuilder medicalRecordBuilder,
            CreateOrderItemDTOBuilder createOrderItemDTOBuilder,
            DoctorUseCase doctorUseCase)
        {
            this.orderBuilder = orderBuilder;
            this.medicalRecordBuilder = medicalRecordBuilder;
            this.createOrderItemDTOBuilder = createOrderItemDTOBuilder;
            this.doctorUseCase = doctorUseCase;
        }
        
        public Order CreateOrder(int orderNumber, DateTime creationDate)
        {
            return doctorUseCase.CreateNewOrder(orderNumber, creationDate);
        }
        
        public void AddMedicationToOrder(Order order, int itemNumber, decimal cost, Medication medication, string dose, int treatmentDuration)
        {
            doctorUseCase.AddMedicationToOrder(order, itemNumber, cost, medication, dose, treatmentDuration);
        }
        
        public void AddProcedureToOrder(Order order, int itemNumber, decimal cost, Procedure procedure, int frequency, bool requiresSpecialist, int? specialistTypeId)
        {
            doctorUseCase.AddProcedureToOrder(order, itemNumber, cost, procedure, frequency, requiresSpecialist, specialistTypeId);
        }
        
        public void AddDiagnosticAidToOrder(Order order, int itemNumber, decimal cost, DiagnosticAid diagnosticAid, int quantity, bool requiresSpecialist, int? specialistTypeId)
        {
            doctorUseCase.AddDiagnosticAidToOrder(order, itemNumber, cost, diagnosticAid, quantity, requiresSpecialist, specialistTypeId);
        }
        
        public void CreateMedicalRecord(DateTime date, Patient patient, string consultationReason, string symptoms, string diagnosis, Order? order = null)
        {
            doctorUseCase.CreateNewMedicalRecord(date, patient, consultationReason, symptoms, diagnosis, order);
        }
        
        public List<MedicalRecord> GetMedicalHistory(string patientDni)
        {
            return doctorUseCase.GetMedicalHistory(patientDni);
        }
        
        public Patient GetPatientByDni(string dni)
        {
            return doctorUseCase.GetPatientByDni(dni);
        }
        
        public List<Appointment> GetPatientAppointments(string patientDni)
        {
            return doctorUseCase.GetPatientAppointments(patientDni);
        }
        
        public List<Order> GetPatientOrders(string patientDni)
        {
            return doctorUseCase.GetPatientOrders(patientDni);
        }
        
        public List<Patient> GetAllPatients()
        {
            return doctorUseCase.GetAllPatients();
        }
        
        public void SetCurrentUser(User user)
        {
            doctorUseCase.SetCurrentUser(user);
        }

        public Order GetOrderByNumber(int orderNumber)
        {
            return doctorUseCase.GetOrderByNumber(orderNumber);
        }

        public void UpdateOrderItem(CreateOrderItemDTO dto, int itemNumber)
        {
            doctorUseCase.UpdateOrderItem(dto, itemNumber);
        }

        public void RemoveOrderItem(int orderNumber, int itemNumber)
        {
            doctorUseCase.RemoveOrderItem(orderNumber, itemNumber);
        }

        public void UpdateMedicalRecord(int id, DateTime date, string consultationReason, string symptoms, string diagnosis)
        {
            doctorUseCase.UpdateMedicalRecord(id, date, consultationReason, symptoms, diagnosis);
        }
    }
}
