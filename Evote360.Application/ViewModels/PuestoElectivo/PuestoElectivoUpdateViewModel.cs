using System.ComponentModel.DataAnnotations;

namespace Evote360.Application.ViewModels.PuestoElectivo
{
    public class PuestoElectivoUpdateViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del puesto es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre del puesto")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es requerida.")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado debe manejarse como booleano.")]
        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        public bool YaFueUtilizadoEnEleccion { get; set; }
    }
}