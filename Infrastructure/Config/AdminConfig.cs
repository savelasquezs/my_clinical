using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Application.Adapters.Input.Builders;
using Clinica_Herramientas_2.Application.UseCases;
using Clinica_Herramientas_2.Domain.Ports;
using Clinica_Herramientas_2.Domain.Services;


namespace Clinica_Herramientas_2.Infrastructure.Config
{
    public class AdminConfig
    {
        // Puertos
        public IPatientPort PatientPort { get; private set; }
        public IAppointmentPort AppointmentPort { get; private set; }
        public IInvoicePort InvoicePort { get; private set; }
        public IUserPort UserPort { get; private set; }
        
        // Servicios
        public CreatePatient CreatePatientService { get; private set; }
        public UpdatePatient UpdatePatientService { get; private set; }
        public CreateAppointment CreateAppointmentService { get; private set; }
        public UpdateAppointment UpdateAppointmentService { get; private set; }
        public DeleteAppointment DeleteAppointmentService { get; private set; }
        public CreateInvoice CreateInvoiceService { get; private set; }
        public ViewPatientInformation ViewPatientInformationService { get; private set; }
        
        // Caso de uso
        public AdminUseCase AdminUseCase { get; private set; }
        
        // Builders
        public PatientBuilder PatientBuilder { get; private set; }
        public AppointmentBuilder AppointmentBuilder { get; private set; }
        public InvoiceBuilder InvoiceBuilder { get; private set; }
        
        // Input
        public AdminInputs AdminInputs { get; private set; }
        
        public AdminConfig(IPatientPort patientPort, IAppointmentPort appointmentPort, IInvoicePort invoicePort, IUserPort userPort, IOrderPort orderPort)
        {
            // Puertos
            PatientPort = patientPort;
            AppointmentPort = appointmentPort;
            InvoicePort = invoicePort;
            UserPort = userPort;
            
            // Servicios
            CreatePatientService = new CreatePatient(PatientPort, UserPort);
            UpdatePatientService = new UpdatePatient(PatientPort);
            ViewPatientInformationService = new ViewPatientInformation(PatientPort, appointmentPort, orderPort);
            CreateAppointmentService = new CreateAppointment(AppointmentPort, PatientPort);
            UpdateAppointmentService = new UpdateAppointment(AppointmentPort);
            DeleteAppointmentService = new DeleteAppointment(AppointmentPort);
            CreateInvoiceService = new CreateInvoice(InvoicePort, PatientPort, userPort, orderPort, new BillingRulesService());
            
            // Caso de uso
            AdminUseCase = new AdminUseCase(
                CreatePatientService,
                UpdatePatientService,
                CreateAppointmentService,
                UpdateAppointmentService,
                DeleteAppointmentService,
                CreateInvoiceService,
                ViewPatientInformationService,
                AppointmentPort
            );
            
            // Builders
            PatientBuilder = new PatientBuilder();
            AppointmentBuilder = new AppointmentBuilder();
            InvoiceBuilder = new InvoiceBuilder();
            
            // Input
            AdminInputs = new AdminInputs(
                PatientBuilder,
                AppointmentBuilder,
                InvoiceBuilder,
                AdminUseCase
            );
        }
    }
}
