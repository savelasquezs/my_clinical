using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Domain.Ports;
using System;

namespace Clinica_Herramientas_2.Domain.Services
{
    public class DeleteInventory
    {
        private readonly IMedicationPort medicationPort;
        private readonly IProcedurePort procedurePort;
        private readonly IDiagnosticAidPort diagnosticAidPort;
        private readonly IOrderPort orderPort;

        public DeleteInventory(
            IMedicationPort medicationPort,
            IProcedurePort procedurePort,
            IDiagnosticAidPort diagnosticAidPort,
            IOrderPort orderPort)
        {
            this.medicationPort = medicationPort;
            this.procedurePort = procedurePort;
            this.diagnosticAidPort = diagnosticAidPort;
            this.orderPort = orderPort;
        }

        public void DeleteMedication(User performingUser, int medicationId)
        {
            if (performingUser == null || performingUser.Role != Role.Support)
            {
                throw new Exception("Solo usuarios con rol Support pueden eliminar medicamentos.");
            }

            var medication = medicationPort.FindById(medicationId);
            if (medication == null)
            {
                throw new Exception("El medicamento no existe.");
            }

            if (orderPort.IsMedicationInUse(medicationId))
            {
                throw new Exception($"No se puede eliminar el medicamento '{medication.Name}' porque está siendo utilizado en una o más órdenes.");
            }

            medicationPort.Delete(medicationId);
        }

        public void DeleteProcedure(User performingUser, int procedureId)
        {
            if (performingUser == null || performingUser.Role != Role.Support)
            {
                throw new Exception("Solo usuarios con rol Support pueden eliminar procedimientos.");
            }

            var procedure = procedurePort.FindById(procedureId);
            if (procedure == null)
            {
                throw new Exception("El procedimiento no existe.");
            }

            if (orderPort.IsProcedureInUse(procedureId))
            {
                throw new Exception($"No se puede eliminar el procedimiento '{procedure.Name}' porque está siendo utilizado en una o más órdenes.");
            }

            procedurePort.Delete(procedureId);
        }

        public void DeleteDiagnosticAid(User performingUser, int diagnosticAidId)
        {
            if (performingUser == null || performingUser.Role != Role.Support)
            {
                throw new Exception("Solo usuarios con rol Support pueden eliminar ayudas diagnósticas.");
            }

            var diagnosticAid = diagnosticAidPort.FindById(diagnosticAidId);
            if (diagnosticAid == null)
            {
                throw new Exception("La ayuda diagnóstica no existe.");
            }

            if (orderPort.IsDiagnosticAidInUse(diagnosticAidId))
            {
                throw new Exception($"No se puede eliminar la ayuda diagnóstica '{diagnosticAid.Name}' porque está siendo utilizada en una o más órdenes.");
            }

            diagnosticAidPort.Delete(diagnosticAidId);
        }
    }
}

