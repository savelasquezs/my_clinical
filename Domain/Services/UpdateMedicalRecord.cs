using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinica_Herramientas_2.Domain.Services
{
    public class UpdateMedicalRecord
    {
        private readonly IMedicalRecordPort medicalRecordPort;
        private readonly IUserPort userPort;

        public UpdateMedicalRecord(IMedicalRecordPort medicalRecordPort, IUserPort userPort)
        {
            this.medicalRecordPort = medicalRecordPort;
            this.userPort = userPort;
        }

        public void Update(int id, DateTime date, string consultationReason, string symptoms, string diagnosis, User performingUser)
        {
            // Validar que el registro existe
            var medicalRecord = medicalRecordPort.FindById(id) ?? throw new Exception("El registro médico no existe");

            // Validar que el usuario tiene permisos para actualizar registros
            if (performingUser.Role != Role.Doctor)
            {
                throw new Exception("Solo los médicos pueden actualizar registros médicos");
            }

            // Actualizar solo los campos editables (no se pueden cambiar Patient, Doctor, Order)
            medicalRecord.Date = date;
            medicalRecord.ConsultationReason = consultationReason;
            medicalRecord.Symptoms = symptoms;
            medicalRecord.Diagnosis = diagnosis;

            // Guardar cambios en la base de datos
            medicalRecordPort.Update(medicalRecord);
        }
    }
}

