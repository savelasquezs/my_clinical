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
    public class NurseUseCase : BaseUseCase
    {
        private CreateNurseVisit createNurseVisit;
        private IOrderPort orderPort;
        private INurseVisit nurseVisitPort;

        internal CreateNurseVisit CreateNurseVisit { get => createNurseVisit; set => createNurseVisit = value; }
        internal User CurrentUser { get => currentUser; set => currentUser = value; }

        public NurseUseCase(CreateNurseVisit createNurseVisit, ViewPatientInformation viewPatientInformation, IOrderPort orderPort, INurseVisit nurseVisitPort)
            : base(viewPatientInformation)
        {
            this.createNurseVisit = createNurseVisit;
            this.orderPort = orderPort;
            this.nurseVisitPort = nurseVisitPort;
        }

        public void SetCurrentUser(User user)
        {
            if (user.Role != Role.Nurse)
            {
                throw new Exception("Solo enfermeras pueden acceder a esta funcionalidad");
            }
            this.CurrentUser = user;
        }

        public void CreateNewNurseVisit(OrderItem orderItem, string testsPerformed, string notes, DateTime performedAt, string bloodPressure, double temperature, int pulse, int oxygenLevel, List<AdministeredMedication> administeredMedications, DateTime visitTime, Patient patient)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer una enfermera válida");
            }

            var vitalData = new VitalData(bloodPressure, temperature, pulse, oxygenLevel);
            var nurseVisit = new NurseVisit(orderItem, testsPerformed, notes, performedAt, this.CurrentUser, vitalData, administeredMedications, visitTime, patient);

            createNurseVisit.Create(nurseVisit);
        }

        public void CreateAdministeredMedication(OrderItem orderItem, string testsPerformed, string notes, DateTime performedAt, Medication medication, string dose, string administrationRoute)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer una enfermera válida");
            }

            var administeredMedication = new AdministeredMedication(orderItem, testsPerformed, notes, performedAt, medication, dose, administrationRoute);
            // El AdministeredMedication se puede agregar a la lista de medicamentos administrados
        }


        public VitalData CreateVitalData(string bloodPressure, double temperature, int pulse, int oxygenLevel)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer una enfermera válida");
            }

            return new VitalData(bloodPressure, temperature, pulse, oxygenLevel);
        }

        public List<Order> GetAvailableOrdersForNurse()
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer una enfermera válida");
            }

            return orderPort.FindOrdersWithNurseVisitProcedure();
        }

        public Order GetOrderDetailsForNurse(int orderNumber)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer una enfermera válida");
            }

            var order = orderPort.FindByNumber(orderNumber);
            if (order == null)
            {
                throw new Exception($"La orden {orderNumber} no existe.");
            }

            // Obtener PerformedProcedures y AdministeredMedications para cada item
            foreach (var item in order.Items)
            {
                var performedProcedures = nurseVisitPort.GetPerformedProceduresByOrderItem(item.OrderNumber, item.ItemNumber);
                var administeredMedications = nurseVisitPort.GetAdministeredMedicationsByOrderItem(item.OrderNumber, item.ItemNumber);
                
                // Esta información se puede usar para calcular progreso en el frontend
                // Por ahora solo cargamos los datos, el cálculo se hará en el frontend
            }

            return order;
        }
    }
}
