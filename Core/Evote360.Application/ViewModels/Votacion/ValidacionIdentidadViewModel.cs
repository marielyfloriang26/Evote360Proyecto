using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Evote360.Application.ViewModels.Votacion
{
    public class ValidacionIdentidadViewModel
    {
        [Required(ErrorMessage = "Debe subir una imagen de su cédula para validar su identidad.")]
        [Display(Name = "Imagen de la cédula")]
        public IFormFile ImagenCedula { get; set; } = null!;
    }
}
