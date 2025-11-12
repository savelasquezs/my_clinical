using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace Clinica_Herramientas_2.Application.UseCases
{
    public class SupportUseCase
    {
        private ManageInventory manageInventory;
        private DeleteInventory deleteInventory;
        private User currentUser;

        internal ManageInventory ManageInventory { get => manageInventory; set => manageInventory = value; }
        internal DeleteInventory DeleteInventoryService { get => deleteInventory; set => deleteInventory = value; }
        internal User CurrentUser { get => currentUser; set => currentUser = value; }

        public SupportUseCase(ManageInventory manageInventory)
        {
            this.manageInventory = manageInventory;
        }

        public void SetCurrentUser(User user)
        {
            if (user.Role != Role.Support)
            {
                throw new Exception("Solo usuarios de soporte pueden acceder a esta funcionalidad");
            }
            this.CurrentUser = user;
        }

        public void CreateMedication(int id, string name, decimal cost, string defaultDose, int treatmentDurationDays)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario de soporte válido");
            }

            var medication = new Medication(id, name, cost, defaultDose, treatmentDurationDays);
            manageInventory.CreateMedication(medication);
        }

        public void UpdateMedication(Medication medication)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario de soporte válido");
            }

            manageInventory.UpdateMedication(medication);
        }

        public void CreateProcedure(int id, string name, decimal cost, int frequency, bool requiresSpecialist, int? specialistTypeId)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario de soporte válido");
            }

            var procedure = new Procedure(id, name, cost, frequency, requiresSpecialist, specialistTypeId);
            manageInventory.CreateProcedure(procedure);
        }

        public void UpdateProcedure(Procedure procedure)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario de soporte válido");
            }

            manageInventory.UpdateProcedure(procedure);
        }

        public void CreateDiagnosticAid(int id, string name, decimal cost, int quantity, bool requiresSpecialist, int? specialistTypeId)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario de soporte válido");
            }

            var diagnosticAid = new DiagnosticAid(id, name, cost, quantity, requiresSpecialist, specialistTypeId);
            manageInventory.CreateDiagnosticAid(diagnosticAid);
        }

        public void UpdateDiagnosticAid(DiagnosticAid diagnosticAid)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario de soporte válido");
            }

            manageInventory.UpdateDiagnosticAid(diagnosticAid);
        }

        public List<Medication> GetAllMedications()
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario de soporte válido");
            }

            return manageInventory.GetAllMedications();
        }

        public List<Procedure> GetAllProcedures()
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario de soporte válido");
            }

            return manageInventory.GetAllProcedures();
        }

        public List<DiagnosticAid> GetAllDiagnosticAids()
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario de soporte válido");
            }

            return manageInventory.GetAllDiagnosticAids();
        }

        public void DeleteMedication(int medicationId)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario de soporte válido");
            }

            deleteInventory.DeleteMedication(this.CurrentUser, medicationId);
        }

        public void DeleteProcedure(int procedureId)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario de soporte válido");
            }

            deleteInventory.DeleteProcedure(this.CurrentUser, procedureId);
        }

        public void DeleteDiagnosticAid(int diagnosticAidId)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario de soporte válido");
            }

            deleteInventory.DeleteDiagnosticAid(this.CurrentUser, diagnosticAidId);
        }
    }
}
