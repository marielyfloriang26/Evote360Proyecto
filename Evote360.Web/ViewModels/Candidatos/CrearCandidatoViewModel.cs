using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Evote360.Web.Validations;

namespace Evote360.Web.ViewModels.Candidatos
{
    public class CrearCandidatoViewModel
    {
        [Required(ErrorMessage = "El nombre del candidato es requerido.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido del candidato es requerido.")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "La foto del candidato es requerida.")]
        [Display(Name = "Foto del candidato")]
        [ValidImage(ErrorMessage = "La foto del candidato debe ser una imagen válida.")]
        public IFormFile Foto { get; set; } = null!;

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;
    }
}