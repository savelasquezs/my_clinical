using Clinica_Herramientas_2.Application.Adapters.Input.Validators;
using Clinica_Herramientas_2.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinica_Herramientas_2.Application.Adapters.Input.Builders
{
    public class PatientBuilder
    {
        private PersonValidator personValidator;
        private PatientValidator patientValidator;
        private EmergencyContactValidator emergencyContactValidator;
        private HealthInsuranceValidator healthInsuranceValidator;

        public PatientBuilder()
        {
            personValidator = new PersonValidator();
            patientValidator = new PatientValidator();
            emergencyContactValidator = new EmergencyContactValidator();
            healthInsuranceValidator = new HealthInsuranceValidator();
        }

        internal PersonValidator PersonValidator { get => personValidator; set => personValidator = value; }
        internal PatientValidator PatientValidator { get => patientValidator; set => patientValidator = value; }
        internal EmergencyContactValidator EmergencyContactValidator { get => emergencyContactValidator; set => emergencyContactValidator = value; }
        internal HealthInsuranceValidator HealthInsuranceValidator { get => healthInsuranceValidator; set => healthInsuranceValidator = value; }

        public Patient Create(string fullname, string dni, string email, string phonenumber, DateOnly birthdate, string address,
            Gender gender, string emergencyFirstName, string emergencyLastName, string emergencyRelationship, string emergencyPhone,
            string insuranceCompanyName, string insurancePolicyNumber, DateTime insuranceExpirationDate)
        {
            var emergencyContact = new EmergencyContact(
                emergencyContactValidator.ValidateFirstName(emergencyFirstName),
                emergencyContactValidator.ValidateLastName(emergencyLastName),
                emergencyContactValidator.ValidateRelationship(emergencyRelationship),
                emergencyContactValidator.ValidatePhoneNumber(emergencyPhone)
            );

            var healthInsurance = new HealthInsurance(
                healthInsuranceValidator.ValidateCompanyName(insuranceCompanyName),
                healthInsuranceValidator.ValidatePolicyNumber(insurancePolicyNumber),
                healthInsuranceValidator.ValidateExpirationDate(insuranceExpirationDate)
            );

            return new Patient(
                personValidator.ValidateFullname(fullname),
                personValidator.ValidateDni(dni),
                personValidator.ValidateEmail(email),
                personValidator.ValidatePhoneNumber(phonenumber),
                personValidator.ValidateBirthdate(birthdate),
                personValidator.ValidateAddress(address),
                gender,
                emergencyContact,
                healthInsurance
            );
        }
    }
}
