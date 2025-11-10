#nullable disable

namespace Clinica_Herramientas_2.Domain.Model
{

    public enum Role
    {
        Admin,
        Doctor,
        Nurse,
        RRHH,
        Support
    }
    public class User:Person
    {
        private  Role role;
        private string username;
        private string password;
        
        public User(string fullname, string dni, string email, string phonenumber, DateOnly birthdate, string address,
            Role role, string username, string password)
            : base(fullname, dni, email, phonenumber, birthdate, address)
        {
            this.role = role;
            this.username = username.Trim();
            this.password = password;
        }

        // Constructor protegido para EF Core
        protected User() { }

        public Role Role { get => role; private set => role = value; }
        public string Username { get => username; private set => username = value; }
        public string Password { get => password; private set => password = value; }
        
        internal void SetRole(Role role)
        {
            this.role = role;
        }
        
        // Propiedades de navegación
        public ICollection<MedicalRecord> MedicalRecordsAsDoctor { get; set; } = new List<MedicalRecord>();
        public ICollection<NurseVisit> NurseVisits { get; set; } = new List<NurseVisit>();
        public ICollection<Invoice> InvoicesAsDoctor { get; set; } = new List<Invoice>();
    }
}
