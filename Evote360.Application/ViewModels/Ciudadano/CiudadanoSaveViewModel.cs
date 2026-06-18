using System.ComponentModel.DataAnnotations;

namespace Evote360.Application.ViewModels.Ciudadano
{
    public class CiudadanoSaveViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El número de documento de identidad es requerido.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El número de documento solo debe contener dígitos numéricos.")]
        public string Cedula { get; set; } = null!;

        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es requerido.")]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "El correo electrónico es requerido.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido.")]
        public string Correo { get; set; } = null!;

        public bool Estado { get; set; } = true;
        public bool BloquearCedula { get; set; }
    }
}