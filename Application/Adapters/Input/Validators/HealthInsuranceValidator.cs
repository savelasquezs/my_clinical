using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinica_Herramientas_2.Application.Adapters.Input.Validators
{
    internal class HealthInsuranceValidator : SimpleValidator
    {
        // Catálogo de compañías de seguros válidas
        private static readonly HashSet<string> ValidInsuranceCompanies = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Sura",
            "EPS Sura",
            "Nueva EPS",
            "Sanitas",
            "Coomeva",
            "Salud Total",
            "Cruz Blanca",
            "Famisanar",
            "Compensar",
            "Cafesalud",
            "Medimas",
            "Aliansalud",
            "Asmet Salud",
            "Capital Salud",
            "Comfenalco",
            "Comfandi",
            "Comfama",
            "Comfasucre",
            "Comparta",
            "Convida",
            "Coosalud",
            "Dusakawi",
            "Ecoopsos",
            "Emdisalud",
            "Emssanar",
            "Eps Familiar",
            "Fundación Salud Mía",
            "Gold Cross",
            "Humana Vivir",
            "IPS Universitaria",
            "Mallamas",
            "Medimás",
            "Mutual Ser",
            "Pijaos Salud",
            "Policía Nacional",
            "Protección Social",
            "Salud Vida",
            "Savia Salud",
            "Sisben",
            "SOS",
            "Sura EPS",
            "Total Salud",
            "Unimec",
            "Vida y Salud"
        };

        public HealthInsuranceValidator() { }

        public string ValidateCompanyName(string companyName)
        {
            ValidateStringLength(companyName, "CompanyName", max: 100, min: 1);
            var trimmedName = companyName.Trim();
            
            // Validar que la compañía esté en el catálogo de compañías válidas
            if (!ValidInsuranceCompanies.Contains(trimmedName))
            {
                throw new ArgumentException($"La compañía de seguros '{trimmedName}' no está registrada en el sistema. Las compañías válidas son: {string.Join(", ", ValidInsuranceCompanies.OrderBy(x => x).Take(10))}...");
            }
            
            return trimmedName;
        }

        public string ValidatePolicyNumber(string policyNumber)
        {
            ValidateStringLength(policyNumber, "PolicyNumber", max: 50, min: 1);
            var trimmedPolicy = policyNumber.Trim();
            
            // Validar formato básico: debe ser alfanumérico (letras, números, guiones)
            if (!System.Text.RegularExpressions.Regex.IsMatch(trimmedPolicy, @"^[a-zA-Z0-9\-]+$"))
            {
                throw new ArgumentException("El número de póliza solo puede contener letras, números y guiones.");
            }
            
            return trimmedPolicy;
        }

        public DateTime ValidateExpirationDate(DateTime expirationDate)
        {
            ValidateDateNotInFuture(expirationDate, "ExpirationDate");
            return expirationDate;
        }
    }
}
