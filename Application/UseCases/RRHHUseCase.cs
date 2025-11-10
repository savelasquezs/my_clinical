using Clinica_Herramientas_2.Domain.Model;
using Clinica_Herramientas_2.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace Clinica_Herramientas_2.Application.UseCases
{
    public class RRHHUseCase
    {
        private CreateUser createUser;
        private UpdateUser updateUser;
        private DeleteUser deleteUser;
        private User currentUser;

        internal CreateUser CreateUser { get => createUser; set => createUser = value; }
        internal UpdateUser UpdateUser { get => updateUser; set => updateUser = value; }
        internal DeleteUser DeleteUser { get => deleteUser; set => deleteUser = value; }
        internal User CurrentUser { get => currentUser; set => currentUser = value; }

        public RRHHUseCase(CreateUser createUser, UpdateUser updateUser, DeleteUser deleteUser)
        {
            this.createUser = createUser;
            this.updateUser = updateUser;
            this.deleteUser = deleteUser;
        }

        public void SetCurrentUser(User user)
        {
            if (user.Role != Role.RRHH)
            {
                throw new Exception("Solo usuarios de RRHH pueden acceder a esta funcionalidad");
            }
            this.CurrentUser = user;
        }

        public void CreateNewUser(string fullname, string dni, string email, string phonenumber, DateOnly birthdate, string address, Role role, string username, string password)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario de RRHH válido");
            }

            var newUser = new User(fullname, dni, email, phonenumber, birthdate, address, role, username, password);
            createUser.Create(this.CurrentUser, newUser);
        }

        public void UpdateExistingUser(User userToUpdate, string fullname, string email, string phonenumber, string address, Role role)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario de RRHH válido");
            }

            // Actualizar los campos del usuario
            userToUpdate.SetEmail(email);
            userToUpdate.SetPhone(phonenumber);
            userToUpdate.SetAddress(address);
            userToUpdate.SetRole(role);
            
            updateUser.Update(this.CurrentUser, userToUpdate);
        }

        public void DeleteExistingUser(User userToDelete)
        {
            if (this.CurrentUser == null)
            {
                throw new Exception("Debe establecer un usuario de RRHH válido");
            }

            deleteUser.Delete(this.CurrentUser, userToDelete);
        }
    }
}
