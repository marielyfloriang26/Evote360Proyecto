using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Evote360.Application.Validations;
namespace Evote360.Application.ViewModels.Candidatos
{
    public class EditarCandidatoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del candidato es requerido.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido del candidato es requerido.")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Display(Name = "Foto del candidato")]
        [ValidImage(ErrorMessage = "La foto del candidato debe ser una imagen válida.")]
        public IFormFile? Foto { get; set; }

        public string? FotoUrlActual { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        public bool HaParticipado { get; set; }
    }
}