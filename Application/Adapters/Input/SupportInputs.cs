using Clinica_Herramientas_2.Application.Adapters.Input.Builders;
using Clinica_Herramientas_2.Application.UseCases;
using Clinica_Herramientas_2.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinica_Herramientas_2.Application.Adapters.Input
{
    public class SupportInputs
    {
        private ClinicalResourceBuilder clinicalResourceBuilder;
        private SupportUseCase supportUseCase;
        
        public SupportInputs(
            ClinicalResourceBuilder clinicalResourceBuilder,
            SupportUseCase supportUseCase)
        {
            this.clinicalResourceBuilder = clinicalResourceBuilder;
            this.supportUseCase = supportUseCase;
        }
        
        public void CreateMedication(int id, string name, decimal cost, string defaultDose, int treatmentDurationDays)
        {
            supportUseCase.CreateMedication(id, name, cost, defaultDose, treatmentDurationDays);
        }
        
        public void UpdateMedication(Medication medication)
        {
            supportUseCase.UpdateMedication(medication);
        }
        
        public void CreateProcedure(int id, string name, decimal cost, int frequency, bool requiresSpecialist, int? specialistTypeId)
        {
            supportUseCase.CreateProcedure(id, name, cost, frequency, requiresSpecialist, specialistTypeId);
        }
        
        public void UpdateProcedure(Procedure procedure)
        {
            supportUseCase.UpdateProcedure(procedure);
        }
        
        public void CreateDiagnosticAid(int id, string name, decimal cost, int quantity, bool requiresSpecialist, int? specialistTypeId)
        {
            supportUseCase.CreateDiagnosticAid(id, name, cost, quantity, requiresSpecialist, specialistTypeId);
        }
        
        public void UpdateDiagnosticAid(DiagnosticAid diagnosticAid)
        {
            supportUseCase.UpdateDiagnosticAid(diagnosticAid);
        }
        
        public List<Medication> GetAllMedications()
        {
            return supportUseCase.GetAllMedications();
        }
        
        public List<Procedure> GetAllProcedures()
        {
            return supportUseCase.GetAllProcedures();
        }
        
        public List<DiagnosticAid> GetAllDiagnosticAids()
        {
            return supportUseCase.GetAllDiagnosticAids();
        }
        
        public void SetCurrentUser(User user)
        {
            supportUseCase.SetCurrentUser(user);
        }

        public void DeleteMedication(int medicationId)
        {
            supportUseCase.DeleteMedication(medicationId);
        }

        public void DeleteProcedure(int procedureId)
        {
            supportUseCase.DeleteProcedure(procedureId);
        }

        public void DeleteDiagnosticAid(int diagnosticAidId)
        {
            supportUseCase.DeleteDiagnosticAid(diagnosticAidId);
        }
    }
}
