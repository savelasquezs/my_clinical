using Clinica_Herramientas_2.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinica_Herramientas_2.Domain.Ports
{
    public interface IInvoicePort
    {
        public Invoice? FindByNumber(int invoiceNumber);
        public List<Invoice> FindByPatient(string patientDni);
        public List<Invoice> FindAll();
        public void Save(Invoice invoice);
        public decimal GetAnnualCopaymentAccumulated(string patientDni, int year);
        public void UpdateAnnualCopaymentAccumulated(string patientDni, int year, decimal newAmount);
    }
}
