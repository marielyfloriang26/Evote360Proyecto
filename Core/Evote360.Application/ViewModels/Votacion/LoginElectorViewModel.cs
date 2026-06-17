using System.ComponentModel.DataAnnotations;

namespace Evote360.Application.ViewModels.Votacion
{
    public class LoginElectorViewModel
    {
        [Required(ErrorMessage = "El número de documento de identidad es requerido.")]
        [Display(Name = "Número de documento de identidad")]
        public string DocumentoIdentidad { get; set; } = null!;
    }
}
