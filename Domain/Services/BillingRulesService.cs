using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clinica_Herramientas_2.Domain.Model;

namespace Clinica_Herramientas_2.Domain.Services
{
    public class BillingRulesService
    {
        public static BillingCalculationResult CalculateBilling(Patient patient, decimal totalAmount, decimal annualCopaymentAccumulated)
        {
            // IsActive es una propiedad calculada que verifica ExpirationDate > DateTime.Today
            if (patient.Insurance == null || !patient.Insurance.IsActive)
            {
                // Sin seguro: paciente paga todo
                return new BillingCalculationResult
                {
                    CopaymentAmount = totalAmount,
                    InsuranceAmount = 0,
                    AnnualCopaymentAccumulated = annualCopaymentAccumulated
                };
            }
            else if (annualCopaymentAccumulated >= 1000000) // 1 millón de pesos
            {
                // Ya alcanzó el tope anual: aseguradora paga todo
                return new BillingCalculationResult
                {
                    CopaymentAmount = 0,
                    InsuranceAmount = totalAmount,
                    AnnualCopaymentAccumulated = annualCopaymentAccumulated
                };
            }
            else
            {
                // Copago normal: $50,000
                var copayment = Math.Min(50000, totalAmount);
                return new BillingCalculationResult
                {
                    CopaymentAmount = copayment,
                    InsuranceAmount = totalAmount - copayment,
                    AnnualCopaymentAccumulated = annualCopaymentAccumulated
                };
            }
        }
    }

    public class BillingCalculationResult
    {
        public decimal CopaymentAmount { get; set; }
        public decimal InsuranceAmount { get; set; }
        public decimal AnnualCopaymentAccumulated { get; set; }
    }
}
