using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinica_Herramientas_2.Application.UseCases
{
    public class DoctorUseCase : BaseUseCase
    {
        private CreateOrder createOrder;
        private CreateOrderItem createOrderItem;
        private AddOrderItem addOrderItem;
        private UpdateOrderItem updateOrderItem;
        private CreateMedicalRecord createMedicalRecord;
        private ViewMedicalHistory viewMedicalHistory;
        private Domain.Ports.IOrderPort orderPort;

        internal CreateOrder CreateOrder { get => createOrder; set => createOrder = value; }
        internal CreateOrderItem CreateOrderItem { get => createOrderItem; set => createOrderItem = value; }
        internal AddOrderItem AddOrderItem { get => addOrderItem; set => addOrderItem = value; }
        internal UpdateOrderItem UpdateOrderItemService { get => updateOrderItem; set => updateOrderItem = value; }
        internal CreateMedicalRecord CreateMedicalRecord { get => createMedicalRecord; set => createMedicalRecord = value; }
        internal ViewMedicalHistory ViewMedicalHistory { get => viewMedicalHistory; set => viewMedicalHistory = value; }
        internal Domain.Ports.IOrderPort OrderPort { get => orderPort; set => orderPort = value; }
        internal User CurrentUser { get => currentUser; set => currentUser = value; }

        public DoctorUseCase(CreateOrder createOrder, CreateOrderItem createOrderItem, AddOrderItem addOrderItem, UpdateOrderItem updateOrderItem, CreateMedicalRecord createMedicalRecord, ViewMedicalHistory viewMedicalHistory, ViewPatientInformation viewPatientInformation, Domain.Ports.IOrderPort orderPort)
            : base(viewPatientInformation)
        {
            this.createOrder = createOrder;
            this.createOrderItem = createOrderItem;
            this.addOrderItem = addOrderItem;
            this.updateOrderItem = updateOrderItem;
            this.createMedicalRecord = createMedicalRecord;
            this.viewMedicalHistory = viewMedicalHistory;
            this.orderPort = orderPort;
        }

        public void SetCurrentUser(User user)
        {
            if (user.Role != Role.Doctor && user.Role != Role.Admin)
            {
                throw new Exception("Solo médicos y administradores pueden acceder a esta funcionalidad");
            }
            this.CurrentUser = user;
        }

        public Order CreateNewOrder(int orderNumber, DateTime creationDate)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un médico válido");
            }

            return createOrder.Create(orderNumber, creationDate);
        }

        public void AddMedicationToOrder(Order order, int itemNumber, decimal cost, Medication medication, string dose, int treatmentDuration)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un médico válido");
            }

            var dto = new CreateOrderItemDTO
            {
                OrderNumber = order.OrderNumber,
                Cost = cost,
                ItemType = OrderItemType.Medication,
                MedicationId = medication.Id,
                Dose = dose,
                TreatmentDuration = treatmentDuration
            };
            addOrderItem.AddItem(dto);
        }

        public void AddProcedureToOrder(Order order, int itemNumber, decimal cost, Procedure procedure, int frequency, bool requiresSpecialist, int? specialistTypeId)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un médico válido");
            }

            var dto = new CreateOrderItemDTO
            {
                OrderNumber = order.OrderNumber,
                Cost = cost,
                ItemType = OrderItemType.Procedure,
                ProcedureId = procedure.Id,
                Frequency = frequency,
                RequiresSpecialist = requiresSpecialist,
                SpecialistTypeId = specialistTypeId
            };
            addOrderItem.AddItem(dto);
        }

        public void AddDiagnosticAidToOrder(Order order, int itemNumber, decimal cost, DiagnosticAid diagnosticAid, int quantity, bool requiresSpecialist, int? specialistTypeId)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un médico válido");
            }

            var dto = new CreateOrderItemDTO
            {
                OrderNumber = order.OrderNumber,
                Cost = cost,
                ItemType = OrderItemType.DiagnosticAid,
                DiagnosticAidId = diagnosticAid.Id,
                Quantity = quantity,
                RequiresSpecialist = requiresSpecialist,
                SpecialistTypeId = specialistTypeId
            };
            addOrderItem.AddItem(dto);
        }

        public void CreateNewMedicalRecord(DateTime date, Patient patient, string consultationReason, string symptoms, string diagnosis, Order? order = null)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un médico válido");
            }

            var medicalRecord = new MedicalRecord(date, patient, this.CurrentUser, consultationReason, symptoms, diagnosis, order ?? null!);
            createMedicalRecord.Create(medicalRecord);
        }

        public List<MedicalRecord> GetMedicalHistory(string patientDni)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un médico válido");
            }

            return viewMedicalHistory.GetMedicalHistory(patientDni, this.CurrentUser);
        }

        public Order GetOrderByNumber(int orderNumber)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un médico válido");
            }

            var order = orderPort.FindByNumber(orderNumber);
            if (order == null)
            {
                throw new Exception($"La orden {orderNumber} no existe.");
            }

            return order;
        }

        public void UpdateOrderItem(CreateOrderItemDTO dto, int itemNumber)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un médico válido");
            }

            updateOrderItem.Update(dto, itemNumber);
        }

        public void RemoveOrderItem(int orderNumber, int itemNumber)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un médico válido");
            }

            orderPort.RemoveItem(orderNumber, itemNumber);
        }

    }
}
