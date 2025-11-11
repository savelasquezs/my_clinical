using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinica_Herramientas_2.Application.Adapters.Input.Validators
{
    internal abstract class SimpleValidator
    {
        protected string StringNotNullOrEmpty(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{fieldName} no puede estar vacío.");
            }
            return value.Trim();
        }

        protected void ValidatePositiveInt(int value, string fieldName)
        {
            if (value <= 0)
            {
                throw new ArgumentException($"{fieldName} debe ser mayor que cero.");
            }
        }

        protected void ValidateNonNegativeDecimal(decimal value, string fieldName)
        {
            if (value < 0)
            {
                throw new ArgumentException($"{fieldName} no puede ser negativo.");
            }
        }

        protected void ValidateNotNull<T>(T value, string fieldName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(fieldName);
            }
        }

        protected void ValidateDateNotInFuture(DateTime date, string fieldName)
        {
            if (date > DateTime.Now)
            {
                throw new ArgumentException($"{fieldName} no puede ser una fecha futura.");
            }
        }

        protected void ValidateDateInFuture(DateTime date, string fieldName)
        {
            if (date <= DateTime.Now)
            {
                throw new ArgumentException($"{fieldName} debe ser una fecha futura.");
            }
        }

        protected void ValidateDateOfBirth(DateTime date, string fieldName, int minAge = 0, int maxAge = 150)
        {
            var age = DateTime.Now.Year - date.Year;
            if (date > DateTime.Now.AddYears(-age)) age--;
            if (age < minAge || age > maxAge)
            {
                throw new ArgumentException($"{fieldName} debe estar entre {minAge} y {maxAge} años.");
            }
        }

        protected void ValidateStringLength(string value, string fieldName, int? max = null, int? min = null)
        {
            StringNotNullOrEmpty(value, fieldName);
            if (max.HasValue && value.Length > max.Value)
            {
                throw new ArgumentException($"{fieldName} no puede tener más de {max.Value} caracteres.");
            }
            if (min.HasValue && value.Length < min.Value)
            {
                throw new ArgumentException($"{fieldName} no puede tener menos de {min.Value} caracteres.");
            }
        }

        protected void ValidateStringIsNumeric(string value, string fieldName)
        {
            StringNotNullOrEmpty(value, fieldName);
            if (!value.All(char.IsDigit))
            {
                throw new ArgumentException($"{fieldName} debe contener solo números.");
            }
        }

        protected void ValidateEmailConstruction(string value, string fieldName)
        {
            StringNotNullOrEmpty(value, fieldName);
            if (!value.Contains('@') || !value.Contains('.') || value.StartsWith('.') || value.EndsWith('.') || value.StartsWith('@') || value.EndsWith('@'))
            {
                throw new ArgumentException($"{fieldName} debe ser un email válido.");
            }
        }

        protected void ValidateStringIsAlphaNumeric(string value, string fieldName)
        {
            StringNotNullOrEmpty(value, fieldName);
            if (!value.All(c => char.IsLetterOrDigit(c)))
            {
                throw new ArgumentException($"{fieldName} debe contener solo letras y números.");
            }
        }

        protected void ValidatePassword(string password, string fieldName)
        {
            StringNotNullOrEmpty(password, fieldName);
            if (password.Length < 8)
            {
                throw new ArgumentException("La contraseña debe tener al menos 8 caracteres.");
            }
            if (!password.Any(char.IsUpper))
            {
                throw new ArgumentException("La contraseña debe tener al menos una letra mayúscula.");
            }
            if (!password.Any(char.IsLower))
            {
                throw new ArgumentException("La contraseña debe tener al menos una letra minúscula.");
            }
            if (!password.Any(char.IsDigit))
            {
                throw new ArgumentException("La contraseña debe tener al menos un número.");
            }
            if (password.All(char.IsLetterOrDigit))
            {
                throw new ArgumentException("La contraseña debe tener al menos un carácter especial.");
            }
        }
    }
}
