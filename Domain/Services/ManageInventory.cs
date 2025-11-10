using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinica_Herramientas_2.Domain.Services
{
    public class ManageInventory
    {
        private readonly IMedicationPort medicationPort;
        private readonly IProcedurePort procedurePort;
        private readonly IDiagnosticAidPort diagnosticAidPort;
        private readonly IInventoryPort inventoryPort;

        public ManageInventory(IMedicationPort medicationPort, IProcedurePort procedurePort, IDiagnosticAidPort diagnosticAidPort, IInventoryPort inventoryPort)
        {
            this.medicationPort = medicationPort;
            this.procedurePort = procedurePort;
            this.diagnosticAidPort = diagnosticAidPort;
            this.inventoryPort = inventoryPort;
        }

        private bool ClinicalResourceIdExists(int id)
        {
            // Verificar si el ID existe en cualquier tipo de recurso clínico
            return inventoryPort.FindMedicationById(id) != null ||
                   inventoryPort.FindProcedureById(id) != null ||
                   inventoryPort.FindDiagnosticAidById(id) != null;
        }

        private string GetResourceTypeName(int id)
        {
            if (inventoryPort.FindMedicationById(id) != null) return "medicamento";
            if (inventoryPort.FindProcedureById(id) != null) return "procedimiento";
            if (inventoryPort.FindDiagnosticAidById(id) != null) return "ayuda diagnóstica";
            return "recurso clínico";
        }

        public void CreateMedication(Medication medication)
        {
            if (ClinicalResourceIdExists(medication.Id))
            {
                var resourceType = GetResourceTypeName(medication.Id);
                throw new Exception($"Ya existe un {resourceType} con el ID {medication.Id}. El ID debe ser único entre todos los recursos clínicos (medicamentos, procedimientos y ayudas diagnósticas).");
            }
            medicationPort.Save(medication);
        }

        public void UpdateMedication(Medication medication)
        {
            _ = medicationPort.FindById(medication.Id) ?? throw new Exception("El medicamento no existe");
            medicationPort.Update(medication);
        }

        public void CreateProcedure(Procedure procedure)
        {
            if (ClinicalResourceIdExists(procedure.Id))
            {
                var resourceType = GetResourceTypeName(procedure.Id);
                throw new Exception($"Ya existe un {resourceType} con el ID {procedure.Id}. El ID debe ser único entre todos los recursos clínicos (medicamentos, procedimientos y ayudas diagnósticas).");
            }
            procedurePort.Save(procedure);
        }

        public void UpdateProcedure(Procedure procedure)
        {
            _ = procedurePort.FindById(procedure.Id) ?? throw new Exception("El procedimiento no existe");
            procedurePort.Update(procedure);
        }

        public void CreateDiagnosticAid(DiagnosticAid diagnosticAid)
        {
            if (ClinicalResourceIdExists(diagnosticAid.Id))
            {
                var resourceType = GetResourceTypeName(diagnosticAid.Id);
                throw new Exception($"Ya existe un {resourceType} con el ID {diagnosticAid.Id}. El ID debe ser único entre todos los recursos clínicos (medicamentos, procedimientos y ayudas diagnósticas).");
            }
            diagnosticAidPort.Save(diagnosticAid);
        }

        public void UpdateDiagnosticAid(DiagnosticAid diagnosticAid)
        {
            _ = diagnosticAidPort.FindById(diagnosticAid.Id) ?? throw new Exception("La ayuda diagnóstica no existe");
            diagnosticAidPort.Update(diagnosticAid);
        }

        public List<Medication> GetAllMedications()
        {
            return medicationPort.FindAll();
        }

        public List<Procedure> GetAllProcedures()
        {
            return procedurePort.FindAll();
        }

        public List<DiagnosticAid> GetAllDiagnosticAids()
        {
            return diagnosticAidPort.FindAll();
        }
    }
}
