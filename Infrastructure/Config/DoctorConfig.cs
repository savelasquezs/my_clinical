using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Application.Adapters.Input.Builders;
using Clinica_Herramientas_2.Application.UseCases;
using Clinica_Herramientas_2.Domain.Ports;
using Clinica_Herramientas_2.Domain.Services;


namespace Clinica_Herramientas_2.Infrastructure.Config
{
    public class DoctorConfig
    {
        // Puertos
        public IOrderPort OrderPort { get; private set; }
        public IInventoryPort InventoryPort { get; private set; }
        public IMedicalRecordPort MedicalRecordPort { get; private set; }
        public IPatientPort PatientPort { get; private set; }
        public IUserPort UserPort { get; private set; }
        public IAppointmentPort AppointmentPort { get; private set; }
        public INurseVisit NurseVisitPort { get; private set; }
        
        // Servicios
        public CreateOrder CreateOrderService { get; private set; }
        public CreateOrderItem CreateOrderItemService { get; private set; }
        public AddOrderItem AddOrderItemService { get; private set; }
        public UpdateOrderItem UpdateOrderItemService { get; private set; }
        public CreateMedicalRecord CreateMedicalRecordService { get; private set; }
        public UpdateMedicalRecord UpdateMedicalRecordService { get; private set; }
        public ViewMedicalHistory ViewMedicalHistoryService { get; private set; }
        public ViewPatientInformation ViewPatientInformationService { get; private set; }
        
        // Caso de uso
        public DoctorUseCase DoctorUseCase { get; private set; }
        
        // Builders
        public OrderBuilder OrderBuilder { get; private set; }
        public MedicalRecordBuilder MedicalRecordBuilder { get; private set; }
        public CreateOrderItemDTOBuilder CreateOrderItemDTOBuilder { get; private set; }
        
        // Input
        public DoctorInputs DoctorInputs { get; private set; }
        
        public DoctorConfig(IOrderPort orderPort, IInventoryPort inventoryPort, IMedicalRecordPort medicalRecordPort, IPatientPort patientPort, IUserPort userPort, IAppointmentPort appointmentPort, INurseVisit nurseVisitPort)
        {
            // Puertos
            OrderPort = orderPort;
            InventoryPort = inventoryPort;
            MedicalRecordPort = medicalRecordPort;
            PatientPort = patientPort;
            UserPort = userPort;
            AppointmentPort = appointmentPort;
            NurseVisitPort = nurseVisitPort;
            
            // Servicios
            CreateOrderService = new CreateOrder(OrderPort);
            CreateOrderItemService = new CreateOrderItem(OrderPort, InventoryPort);
            AddOrderItemService = new AddOrderItem(OrderPort, InventoryPort, new OrderRulesService());
            UpdateOrderItemService = new UpdateOrderItem(OrderPort, InventoryPort, new OrderRulesService());
            CreateMedicalRecordService = new CreateMedicalRecord(PatientPort, UserPort, OrderPort, MedicalRecordPort, AppointmentPort);
            UpdateMedicalRecordService = new UpdateMedicalRecord(MedicalRecordPort, UserPort);
            ViewMedicalHistoryService = new ViewMedicalHistory(PatientPort, UserPort, MedicalRecordPort);
            ViewPatientInformationService = new ViewPatientInformation(PatientPort, AppointmentPort, OrderPort);
            
            // Caso de uso
            DoctorUseCase = new DoctorUseCase(
                CreateOrderService,
                CreateOrderItemService,
                AddOrderItemService,
                UpdateOrderItemService,
                CreateMedicalRecordService,
                UpdateMedicalRecordService,
                ViewMedicalHistoryService,
                ViewPatientInformationService,
                OrderPort,
                AppointmentPort
            );
            
            // Builders
            OrderBuilder = new OrderBuilder();
            MedicalRecordBuilder = new MedicalRecordBuilder();
            CreateOrderItemDTOBuilder = new CreateOrderItemDTOBuilder();
            
            // Input
            DoctorInputs = new DoctorInputs(
                OrderBuilder,
                MedicalRecordBuilder,
                CreateOrderItemDTOBuilder,
                DoctorUseCase
            );
        }
    }
}
