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
                // 1. Check extension
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !_validExtensions.Contains(extension))
                {
                    var message = ErrorMessage ?? "El archivo seleccionado no tiene un formato de imagen válido (.jpg, .jpeg, .png).";
                    return new ValidationResult(message, new[] { validationContext.MemberName! });
                }

                // 2. Validate magic bytes (signatures)
                try
                {
                    using (var stream = file.OpenReadStream())
                    {
                        if (stream.Length < 4)
                        {
                            var message = ErrorMessage ?? "El archivo seleccionado es demasiado pequeño o inválido.";
                            return new ValidationResult(message, new[] { validationContext.MemberName! });
                        }

                        byte[] buffer = new byte[4];
                        int bytesRead = stream.Read(buffer, 0, 4);
                        if (bytesRead < 4)
                        {
                            var message = ErrorMessage ?? "No se pudo leer la cabecera del archivo.";
                            return new ValidationResult(message, new[] { validationContext.MemberName! });
                        }

                        bool isValidImageHeader = false;

                        // Check JPEG magic bytes: FF D8 FF
                        if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
                        {
                            isValidImageHeader = true;
                        }
                        // Check PNG magic bytes: 89 50 4E 47
                        else if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
                        {
                            isValidImageHeader = true;
                        }

                        if (!isValidImageHeader)
                        {
                            var message = ErrorMessage ?? "El archivo seleccionado no es una imagen válida o real.";
                            return new ValidationResult(message, new[] { validationContext.MemberName! });
                        }
                    }
                }
                catch
                {
                    var message = ErrorMessage ?? "No se pudo verificar el archivo de imagen.";
                    return new ValidationResult(message, new[] { validationContext.MemberName! });
                }
            }
            return ValidationResult.Success;
        }
    }
}