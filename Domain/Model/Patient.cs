#nullable disable

namespace Clinica_Herramientas_2.Domain.Model
{
    public enum Gender
    {
        Male, Female, Other
    }
    public class Patient : Person
    {
        private Gender gender;
        private EmergencyContact emergencyContact;
        private HealthInsurance insurance;

        // Constructor principal - valida que emergencyContact e insurance no sean null
        public Patient(string fullname, string dni, string email, string phonenumber, DateOnly birthdate, string address,
            Gender gender, EmergencyContact emergencyContact, HealthInsurance insurance) 
            : base(fullname, dni, email, phonenumber, birthdate, address)
        {
            this.gender = gender;
            this.emergencyContact = emergencyContact ?? throw new ArgumentNullException(nameof(emergencyContact), "Debe registrar un contacto de emergencia.");
            this.insurance = insurance ?? throw new ArgumentNullException(nameof(insurance), "Debe registrar un seguro de salud.");
        }

        // Constructor protegido para EF Core - permite null temporalmente para materialización
        protected Patient() : base("", "", "", "", DateOnly.MinValue, "")
        {
            // EF Core establecerá estas propiedades después de la materialización
            this.gender = Gender.Male;
        }

        public HealthInsurance Insurance { get => insurance; private set => insurance = value; }
        internal Gender Gender { get => gender; private set => gender = value; }
        internal EmergencyContact EmergencyContact { get => emergencyContact; private set => emergencyContact = value; }
        
        // Propiedades de navegación
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public ICollection<NurseVisit> NurseVisits { get; set; } = new List<NurseVisit>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        
        public void UpdateBasicInfo(string email, string phone, string address)
        {
            SetEmail(email);
            SetPhone(phone);
            SetAddress(address);
        }
        
        internal void UpdateEmergencyContact(string firstname, string lastname, string relationship, string phoneNumber)
        {
            if (this.emergencyContact == null)
            {
                throw new InvalidOperationException("El contacto de emergencia no está inicializado.");
            }
            this.emergencyContact.Update(firstname, lastname, relationship, phoneNumber);
        }
        
        internal void UpdateInsurance(string companyName, string policyNumber, bool isActive, DateTime expirationDate)
        {
            if (this.insurance == null)
            {
                throw new InvalidOperationException("El seguro de salud no está inicializado.");
            }
            this.insurance.Update(companyName, policyNumber, isActive, expirationDate);
        }
    }

    public class EmergencyContact
    {
        private string firtname;
        private string lastname;
        private string relationship;
        private string phoneNumber;
        
        public EmergencyContact(string firstname, string lastname, string relationship, string phoneNumber)
        {
            this.firtname = firstname.Trim();
            this.lastname = lastname.Trim();
            this.relationship = relationship.Trim();
            this.phoneNumber = phoneNumber.Trim();
        }

        // Constructor protegido para EF Core
        protected EmergencyContact() { }

        public string Firtname { get => firtname; private set => firtname = value; }
        public string Lastname { get => lastname; private set => lastname = value; }
        public string Relationship { get => relationship; private set => relationship = value; }
        public string PhoneNumber { get => phoneNumber; private set => phoneNumber = value; }
        
        internal void Update(string firstname, string lastname, string relationship, string phoneNumber)
        {
            this.firtname = firstname.Trim();
            this.lastname = lastname.Trim();
            this.relationship = relationship.Trim();
            this.phoneNumber = phoneNumber.Trim();
        }
    }

    public class HealthInsurance
    {
        private string _companyName;
        private string _policyNumber;
        private bool _isActive;
        private DateTime _expirationDate;
        
        public HealthInsurance(string companyName, string policyNumber, bool isActive, DateTime expirationDate)
        {
            _companyName = companyName.Trim();
            _policyNumber = policyNumber.Trim();
            _isActive = isActive;
            _expirationDate = expirationDate;
        }

        // Constructor protegido para EF Core
        protected HealthInsurance() { }

        public string CompanyName { get => _companyName; private set => _companyName = value; }
        public string PolicyNumber { get => _policyNumber; private set => _policyNumber = value; }
        public bool IsActive { get => _isActive; private set => _isActive = value; }
        public DateTime ExpirationDate { get => _expirationDate; private set => _expirationDate = value; }
        
        internal void Update(string companyName, string policyNumber, bool isActive, DateTime expirationDate)
        {
            this._companyName = companyName.Trim();
            this._policyNumber = policyNumber.Trim();
            this._isActive = isActive;
            this._expirationDate = expirationDate;
        }
    }
}
