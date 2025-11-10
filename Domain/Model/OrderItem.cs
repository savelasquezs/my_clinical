using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace Clinica_Herramientas_2.Domain.Model
{
    public class OrderItem
    {
        private int orderNumber;
        private int itemNumber;
        private decimal cost;

        public OrderItem(int orderNumber, int itemNumber, decimal cost)
        {
            this.orderNumber = orderNumber;
            this.itemNumber = itemNumber;
            this.cost = cost;
        }

        // Constructor protegido para EF Core
        protected OrderItem() { }

        public int OrderNumber { get => orderNumber; private set => orderNumber = value; }
        public int ItemNumber { get => itemNumber; private set => itemNumber = value; }
        public decimal Cost { get => cost; private set => cost = value; }

        internal void Update(decimal cost)
        {
            this.cost = cost;
        }
    }
    public class MedicationOrderItem : OrderItem
    {
        private Medication medication;
        private string dose;
        private int treatmentDuration;

        public MedicationOrderItem(int orderNumber, int itemNumber, decimal cost, Medication medication, string dose, int treatmentDuration)
            : base(orderNumber, itemNumber, cost)
        {
            ArgumentNullException.ThrowIfNull(medication);
            this.medication = medication;
            this.dose = dose.Trim();
            this.treatmentDuration = treatmentDuration;
        }

        // Constructor protegido para EF Core
        protected MedicationOrderItem() { }

        public Medication Medication { get => medication; private set => medication = value; }
        public string Dose { get => dose; private set => dose = value; }
        public int TreatmentDuration { get => treatmentDuration; private set => treatmentDuration = value; }

        internal void Update(decimal cost, Medication medication, string dose, int treatmentDuration)
        {
            ArgumentNullException.ThrowIfNull(medication);
            base.Update(cost);
            this.medication = medication;
            this.dose = dose.Trim();
            this.treatmentDuration = treatmentDuration;
        }
    }

    public class ProcedureOrderItem : OrderItem
    {
        private Procedure procedure;
        private int frequency;
        private bool requiresSpecialist;
        private int? specialistTypeId;

        public ProcedureOrderItem(int orderNumber, int itemNumber, decimal cost, Procedure procedure, int frequency, bool requiresSpecialist, int? specialistTypeId)
            : base(orderNumber, itemNumber, cost)
        {
         ArgumentNullException.ThrowIfNull(procedure);
            this.procedure = procedure;
            this.frequency = frequency;
            this.requiresSpecialist = requiresSpecialist;
            this.specialistTypeId = specialistTypeId;
        }

        // Constructor protegido para EF Core
        protected ProcedureOrderItem() { }

        public Procedure Procedure { get => procedure; private set => procedure = value; }
        public int Frequency { get => frequency; private set => frequency = value; }
        public bool RequiresSpecialist { get => requiresSpecialist; private set => requiresSpecialist = value; }
        public int? SpecialistTypeId { get => specialistTypeId; private set => specialistTypeId = value; }

        internal void Update(decimal cost, Procedure procedure, int frequency, bool requiresSpecialist, int? specialistTypeId)
        {
            ArgumentNullException.ThrowIfNull(procedure);
            base.Update(cost);
            this.procedure = procedure;
            this.frequency = frequency;
            this.requiresSpecialist = requiresSpecialist;
            this.specialistTypeId = specialistTypeId;
        }
    }

    public class DiagnosticAidOrderItem : OrderItem
    {
        private DiagnosticAid diagnosticAid;
        private int quantity;
        private bool requiresSpecialist;
        private int? specialistTypeId;

        public DiagnosticAidOrderItem(int orderNumber, int itemNumber, decimal cost, DiagnosticAid diagnosticAid, int quantity, bool requiresSpecialist, int? specialistTypeId)
            : base(orderNumber, itemNumber, cost)
        {
         ArgumentNullException.ThrowIfNull(diagnosticAid);
            this.diagnosticAid = diagnosticAid;
            this.quantity = quantity;
            this.requiresSpecialist = requiresSpecialist;
            this.specialistTypeId = specialistTypeId;
        }

        // Constructor protegido para EF Core
        protected DiagnosticAidOrderItem() { }

        public DiagnosticAid DiagnosticAid { get => diagnosticAid; private set => diagnosticAid = value; }
        public int Quantity { get => quantity; private set => quantity = value; }
        public bool RequiresSpecialist { get => requiresSpecialist; private set => requiresSpecialist = value; }
        public int? SpecialistTypeId { get => specialistTypeId; private set => specialistTypeId = value; }

        internal void Update(decimal cost, DiagnosticAid diagnosticAid, int quantity, bool requiresSpecialist, int? specialistTypeId)
        {
            ArgumentNullException.ThrowIfNull(diagnosticAid);
            base.Update(cost);
            this.diagnosticAid = diagnosticAid;
            this.quantity = quantity;
            this.requiresSpecialist = requiresSpecialist;
            this.specialistTypeId = specialistTypeId;
        }
    }
}
