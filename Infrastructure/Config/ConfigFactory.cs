using Clinica_Herramientas_2.Infrastructure.Config;
using Clinica_Herramientas_2.Infrastructure.Adapters.Output.Persistence;

namespace Clinica_Herramientas_2.Infrastructure.Config
{
    internal class ConfigFactory
    {
        private readonly PortsFactory ports;
        
        private AdminConfig? adminConfig;
        private DoctorConfig? doctorConfig;
        private NurseConfig? nurseConfig;
        private RRHHConfig? rrhhConfig;
        private SupportConfig? supportConfig;
        private AuthConfig? authConfig;
        
        public ConfigFactory(PortsFactory ports)
        {
            this.ports = ports;
        }
        
        public AdminConfig AdminConfig => adminConfig ??= new AdminConfig(
            ports.PatientPort, ports.AppointmentPort, ports.InvoicePort,
            ports.UserPort, ports.OrderPort
        );
        
        public DoctorConfig DoctorConfig => doctorConfig ??= new DoctorConfig(
            ports.OrderPort, ports.InventoryPort, ports.MedicalRecordPort,
            ports.PatientPort, ports.UserPort, ports.AppointmentPort
        );
        
        public NurseConfig NurseConfig => nurseConfig ??= new NurseConfig(
            ports.NurseVisitPort, ports.PatientPort, ports.UserPort,
            ports.AppointmentPort, ports.OrderPort
        );
        
        public RRHHConfig RRHHConfig => rrhhConfig ??= new RRHHConfig(
            ports.UserPort
        );
        
        public SupportConfig SupportConfig => supportConfig ??= new SupportConfig(
            ports.MedicationPort, ports.ProcedurePort,
            ports.DiagnosticAidPort, ports.InventoryPort, ports.UserPort
        );
        
        public AuthConfig AuthConfig => authConfig ??= new AuthConfig(
            ports.UserPort
        );
    }
}
