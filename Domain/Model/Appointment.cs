#nullable disable

namespace Clinica_Herramientas_2.Domain.Model
{
    public class Appointment
    {
        private int Id;
        private Patient Patient;
        private DateTime Date;
        private bool IsAccepted;
        
        public Appointment(int id, Patient patient, DateTime date, bool isAccepted = false)
        {
            ArgumentNullException.ThrowIfNull(patient);

            Id = id;
            Patient = patient;
            Date = date;
            IsAccepted = isAccepted;
        }

        // Constructor protegido para EF Core
        protected Appointment() { }

        public int Id1 { get => Id; private set => Id = value; }
        public DateTime Date1 { get => Date; private set => Date = value; }
        public bool IsAccepted1 { get => IsAccepted; private set => IsAccepted = value; }
        internal Patient Patient1 { get => Patient; private set => Patient = value; }
        
        internal void UpdateDate(DateTime newDate)
        {
            if (newDate < DateTime.Now)
            {
                throw new ArgumentException("La fecha de la cita no puede ser en el pasado");
            }
            Date = newDate;
        }

        internal void Accept()
        {
            if (IsAccepted)
            {
                throw new ArgumentException("La cita ya ha sido aceptada");
            }
            IsAccepted = true;
        }
    }
}
