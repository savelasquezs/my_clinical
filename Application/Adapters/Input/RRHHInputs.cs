using Clinica_Herramientas_2.Application.Adapters.Input.Builders;
using Clinica_Herramientas_2.Application.UseCases;
using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinica_Herramientas_2.Application.Adapters.Input
{
    public class RRHHInputs
    {
        private UserBuilder userBuilder;
        private RRHHUseCase rrhhUseCase;
        private IUserPort userPort;
        
        public RRHHInputs(
            UserBuilder userBuilder,
            RRHHUseCase rrhhUseCase,
            IUserPort userPort)
        {
            this.userBuilder = userBuilder;
            this.rrhhUseCase = rrhhUseCase;
            this.userPort = userPort;
        }
        
        public void CreateUser(string fullname, string dni, string email, string phonenumber, DateOnly birthdate, string address, Role role, string username, string password)
        {
            // Usar builder para crear usuario
            var user = userBuilder.Create(fullname, dni, email, phonenumber, birthdate, address, role, username, password);
            
            // Llamar a rrhhUseCase.CreateNewUser()
            rrhhUseCase.CreateNewUser(fullname, dni, email, phonenumber, birthdate, address, role, username, password);
        }
        
        public void UpdateUser(User userToUpdate, string fullname, string email, string phonenumber, string address, Role role)
        {
            rrhhUseCase.UpdateExistingUser(userToUpdate, fullname, email, phonenumber, address, role);
        }
        
        public void DeleteUser(User userToDelete)
        {
            rrhhUseCase.DeleteExistingUser(userToDelete);
        }

        public List<User> GetAllUsers()
        {
            // Acceso de solo lectura a través del puerto
            return userPort.FindAll();
        }

        public User? FindByUsername(string username)
        {
            return userPort.FindByUsername(username);
        }

        public void SetCurrentUser(User user)
        {
            rrhhUseCase.SetCurrentUser(user);
        }
    }
}
