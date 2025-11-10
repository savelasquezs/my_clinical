using Clinica_Herramientas_2.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinica_Herramientas_2.Domain.Ports
{
    public interface IOrderPort
    {
        public Order? FindByNumber(int orderNumber);
        public List<Order> FindByPatientDni(string patientDni);
        public bool ItemExists(int orderNumber, int itemNumber);
        public OrderItem? FindItemByNumber(int orderNumber, int itemNumber);
        public void RemoveItem(int orderNumber, int itemNumber);
        public void Save(Order order);
        public void Update(Order order);
        public OrderItem Create(CreateOrderItemDTO dto);
    }
}

