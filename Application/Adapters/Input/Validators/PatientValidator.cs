using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinica_Herramientas_2.Application.Adapters.Input.Validators
{
    internal class PatientValidator : SimpleValidator
    {
        public PatientValidator() { }

        public string ValidateFullname(string fullname)
        {
            return StringNotNullOrEmpty(fullname, "Fullname");
        }

        public string ValidateDni(string dni)
        {
            ValidateStringIsNumeric(dni, "Dni");
            ValidateStringLength(dni, "Dni", max: 10, min: 1);
            return dni.Trim();
        }

        public string ValidateEmail(string email)
        {
            ValidateEmailConstruction(email, "Email");
            return email.Trim();
        }

        public string ValidatePhoneNumber(string phoneNumber)
        {
            ValidateStringIsNumeric(phoneNumber, "PhoneNumber");
            ValidateStringLength(phoneNumber, "PhoneNumber", max: 10, min: 10);
            return phoneNumber.Trim();
        }

        public DateOnly ValidateBirthdate(DateOnly birthdate)
        {
            var birthdateDt = new DateTime(birthdate.Year, birthdate.Month, birthdate.Day);
            ValidateDateOfBirth(birthdateDt, "Birthdate", 0, 150);
            return birthdate;
        }

        public string ValidateAddress(string address)
        {
            ValidateStringLength(address, "Address", max: 30, min: 1);
            return address.Trim();
        }

        public string ValidateEmergencyContactFirstName(string firstName)
        {
            ValidateStringLength(firstName, "EmergencyContactFirstName", max: 100, min: 1);
            return firstName.Trim();
        }

        public string ValidateEmergencyContactLastName(string lastName)
        {
            ValidateStringLength(lastName, "EmergencyContactLastName", max: 100, min: 1);
            return lastName.Trim();
        }

        public string ValidateEmergencyContactRelationship(string relationship)
        {
            ValidateStringLength(relationship, "EmergencyContactRelationship", max: 50, min: 1);
            return relationship.Trim();
        }

        public string ValidateEmergencyContactPhone(string phoneNumber)
        {
            ValidateStringIsNumeric(phoneNumber, "EmergencyContactPhone");
            ValidateStringLength(phoneNumber, "EmergencyContactPhone", max: 10, min: 10);
            return phoneNumber.Trim();
        }

        public string ValidateInsuranceCompanyName(string companyName)
        {
            ValidateStringLength(companyName, "InsuranceCompanyName", max: 100, min: 1);
            return companyName.Trim();
        }

        public string ValidateInsurancePolicyNumber(string policyNumber)
        {
            ValidateStringLength(policyNumber, "InsurancePolicyNumber", max: 50, min: 1);
            return policyNumber.Trim();
        }

        public DateTime ValidateInsuranceExpirationDate(DateTime expirationDate)
        {
            ValidateDateInFuture(expirationDate, "InsuranceExpirationDate");
            return expirationDate;
        }
    }
}
