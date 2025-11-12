using Clinica_Herramientas_2.Application.Adapters.Input;
using Clinica_Herramientas_2.Application.Adapters.Input.Builders;
using Clinica_Herramientas_2.Application.UseCases;
using Clinica_Herramientas_2.Domain.Ports;
using Clinica_Herramientas_2.Domain.Services;


namespace Clinica_Herramientas_2.Infrastructure.Config
{
    public class SupportConfig
    {
        // Puertos
        public IMedicationPort MedicationPort { get; private set; }
        public IProcedurePort ProcedurePort { get; private set; }
        public IDiagnosticAidPort DiagnosticAidPort { get; private set; }
        public IInventoryPort InventoryPort { get; private set; }
        public IUserPort UserPort { get; private set; }
        public IOrderPort OrderPort { get; private set; }
        
        // Servicios
        public ManageInventory ManageInventoryService { get; private set; }
        public InventoryService InventoryService { get; private set; }
        public DeleteInventory DeleteInventoryService { get; private set; }
        
        // Caso de uso
        public SupportUseCase SupportUseCase { get; private set; }
        
        // Builders
        public ClinicalResourceBuilder ClinicalResourceBuilder { get; private set; }
        
        // Input
        public SupportInputs SupportInputs { get; private set; }
        
        public SupportConfig(IMedicationPort medicationPort, IProcedurePort procedurePort, IDiagnosticAidPort diagnosticAidPort, IInventoryPort inventoryPort, IUserPort userPort, IOrderPort orderPort)
        {
            // Puertos
            MedicationPort = medicationPort;
            ProcedurePort = procedurePort;
            DiagnosticAidPort = diagnosticAidPort;
            InventoryPort = inventoryPort;
            UserPort = userPort;
            OrderPort = orderPort;
            
            // Servicios
            ManageInventoryService = new ManageInventory(MedicationPort, ProcedurePort, DiagnosticAidPort, InventoryPort);
            InventoryService = new InventoryService(MedicationPort, ProcedurePort, DiagnosticAidPort);
            DeleteInventoryService = new DeleteInventory(MedicationPort, ProcedurePort, DiagnosticAidPort, OrderPort);
            
            // Caso de uso
            SupportUseCase = new SupportUseCase(ManageInventoryService);
            SupportUseCase.DeleteInventoryService = DeleteInventoryService;
            
            // Builders
            ClinicalResourceBuilder = new ClinicalResourceBuilder();
            
            // Input
            SupportInputs = new SupportInputs(
                ClinicalResourceBuilder,
                SupportUseCase
            );
        }
    }
}
