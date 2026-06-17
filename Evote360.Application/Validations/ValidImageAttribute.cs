using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace Evote360.Application.Validations
{
    public class ValidImageAttribute : ValidationAttribute
    {
        private readonly string[] _validExtensions = { ".jpg", ".jpeg", ".png" };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !_validExtensions.Contains(extension))
                {
                    var message = ErrorMessage ?? "El archivo seleccionado no tiene un formato de imagen válido (.jpg, .jpeg, .png).";
                    return new ValidationResult(message, new[] { validationContext.MemberName! });
                }
            }
            return ValidationResult.Success;
        }
    }
}