using System.ComponentModel.DataAnnotations;

namespace Evote360.Application.ViewModels.Votacion
{
    public class VerificacionCodigoViewModel
    {
        [Required(ErrorMessage = "Debe ingresar el código de verificación enviado a su correo electrónico.")]
        [Display(Name = "Código de verificación")]
        public string CodigoVerificacion { get; set; } = null!;
    }
}
