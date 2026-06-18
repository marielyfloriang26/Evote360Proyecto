using System;
using System.ComponentModel.DataAnnotations;

namespace Evote360.Application.ViewModels.Eleccion
{
    public class EleccionCreateViewModel
    {
        [Required(ErrorMessage = "El nombre de la elección es requerido.")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder los 150 caracteres.")]
        [Display(Name = "Nombre de la Elección")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de realización es requerida.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Realización")]
        public DateTime FechaRealizacion { get; set; } = DateTime.Today;
    }
}