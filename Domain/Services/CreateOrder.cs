using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Domain.Ports;

namespace Clinica_Herramientas_2.Domain.Services
{
    public class CreateOrder(IOrderPort orderPort)
    {
        private readonly IOrderPort orderPort = orderPort;

        public Order Create(int orderNumber, DateTime creationDate)
        {
            // Validar máximo 6 dígitos
            if (orderNumber > 999999)
            {
                throw new Exception("El número de orden no puede tener más de 6 dígitos.");
            }

            // Unicidad de número de orden
            if (orderPort.FindByNumber(orderNumber) != null)
            {
                throw new Exception("La orden ya existe (número duplicado).");
            }

            // Crear orden vacía
            var order = new Order(orderNumber, creationDate, []);

            // Persistir orden vacía
            orderPort.Save(order);
            
            return order;
        }
    }
}

