using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Application.Adapters.Input.Builders;
using Clinica_Herramientas_2.Application.UseCases;
using Clinica_Herramientas_2.Domain.Ports;
using Clinica_Herramientas_2.Domain.Services;


namespace Clinica_Herramientas_2.Infrastructure.Config
{
    public class NurseConfig
    {
        // Puertos
        public INurseVisit NurseVisitPort { get; private set; }
        public IPatientPort PatientPort { get; private set; }
        public IUserPort UserPort { get; private set; }
        public IAppointmentPort AppointmentPort { get; private set; }
        public IOrderPort OrderPort { get; private set; }
        
        // Servicios
        public CreateNurseVisit CreateNurseVisitService { get; private set; }
        public ViewPatientInformation ViewPatientInformationService { get; private set; }
        
        // Caso de uso
        public NurseUseCase NurseUseCase { get; private set; }
        
        // Builders
        public NurseVisitBuilder NurseVisitBuilder { get; private set; }
        
        // Input
        public NurseInputs NurseInputs { get; private set; }
        
        public NurseConfig(INurseVisit nurseVisitPort, IPatientPort patientPort, IUserPort userPort, IAppointmentPort appointmentPort, IOrderPort orderPort)
        {
            // Puertos
            NurseVisitPort = nurseVisitPort;
            PatientPort = patientPort;
            UserPort = userPort;
            AppointmentPort = appointmentPort;
            OrderPort = orderPort;
            
            // Servicios
            CreateNurseVisitService = new CreateNurseVisit(NurseVisitPort, PatientPort, UserPort);
            ViewPatientInformationService = new ViewPatientInformation(PatientPort, AppointmentPort, OrderPort);
            
            // Caso de uso
            NurseUseCase = new NurseUseCase(
                CreateNurseVisitService,
                ViewPatientInformationService,
                OrderPort,
                NurseVisitPort
            );
            
            // Builders
            NurseVisitBuilder = new NurseVisitBuilder();
            
            // Input
            NurseInputs = new NurseInputs(
                NurseVisitBuilder,
                NurseUseCase
            );
        }
    }
}
