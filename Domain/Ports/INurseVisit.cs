using Clinica_Herramientas_2.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinica_Herramientas_2.Domain.Ports
{
    public interface INurseVisit
    {
        public bool IsValidVitalData(VitalData vitalData);
        public void Save(NurseVisit nurseVisit);
        public NurseVisit? FindById(NurseVisit nurseVisit);
        public void DeleteById(NurseVisit nurseVisit);
        public bool OrderItemExists(OrderItem orderItem);
        public List<PerformedProcedure> GetPerformedProceduresByOrderItem(int orderNumber, int itemNumber);
        public List<AdministeredMedication> GetAdministeredMedicationsByOrderItem(int orderNumber, int itemNumber);
        public List<NurseVisit> FindByOrderNumber(int orderNumber);
    }
}

