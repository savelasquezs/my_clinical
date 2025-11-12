
#nullable disable

namespace Clinica_Herramientas_2.Domain.Model
{
    public class NurseVisit:PerformedProcedure
    {
        private User nurse;
        private Patient patient;
        private VitalData vitalData;
        private List<AdministeredMedication> administeredMedications;
        private DateTime visitTime;

        public NurseVisit(OrderItem orderItem, string testsPerformed, string notes, DateTime performedAt,
            User nurse, VitalData vitalData, List<AdministeredMedication> administeredMedications, DateTime visitTime, Patient patient)
            : base(orderItem, testsPerformed, notes, performedAt)
        {
            ArgumentNullException.ThrowIfNull(nurse);
            if (nurse.Role != Role.Nurse)
            {
                throw new ArgumentException("El usuario asignado no tiene rol de enfermera.");
            }
            ArgumentNullException.ThrowIfNull(vitalData);

            this.nurse = nurse;
            this.patient = patient;
            this.vitalData = vitalData;
            this.administeredMedications = administeredMedications ?? [];
            if (this.administeredMedications.Any(m => m == null))
            {
                throw new ArgumentException("La lista de medicamentos administrados contiene elementos nulos.");
            }
            this.visitTime = visitTime;
            this.OrderItem = orderItem; 
        }

        // Constructor protegido para EF Core
        protected NurseVisit() : base()
        {
            // No llamar al constructor base con null para evitar validación durante materialización
            this.administeredMedications = new List<AdministeredMedication>();
            this.visitTime = DateTime.MinValue;
        }

        public DateTime VisitTime { get => visitTime; private set => visitTime = value; }
        public new OrderItem OrderItem { get; internal set; }
        internal Patient Patient { get => patient; private set => patient = value; }
        internal User Nurse { get => nurse; private set => nurse = value; }
        internal VitalData VitalData { get => vitalData; private set => vitalData = value; }
        internal List<AdministeredMedication> AdministeredMedications { get => administeredMedications; private set => administeredMedications = value; }
    }
    public class VitalData
    {
        private string _bloodPressure;
        private double _temperature;
        private int _pulse;
        private int _oxygenLevel;
        
        public VitalData(string bloodPressure, double temperature, int pulse, int oxygenLevel)
        {
            _bloodPressure = bloodPressure;
            _temperature = temperature;
            _pulse = pulse;
            _oxygenLevel = oxygenLevel;
        }

        // Constructor protegido para EF Core
        protected VitalData() { }

        public string BloodPressure { get => _bloodPressure; private set => _bloodPressure = value; }
        public double Temperature { get => _temperature; private set => _temperature = value; }
        public int Pulse { get => _pulse; private set => _pulse = value; }
        public int OxygenLevel { get => _oxygenLevel; private set => _oxygenLevel = value; }
    }
}
