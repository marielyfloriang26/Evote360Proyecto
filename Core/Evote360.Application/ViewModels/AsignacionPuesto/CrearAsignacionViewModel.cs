using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Evote360.Application.ViewModels.Asignaciones
{
    public class CrearAsignacionViewModel
    {
        [Required(ErrorMessage = "El candidato político es requerido.")]
        [Display(Name = "Candidato político")]
        public int? CandidatoId { get; set; }

        [Required(ErrorMessage = "El puesto electivo es requerido.")]
        [Display(Name = "Puesto electivo")]
        public int? PuestoId { get; set; }

        // Propiedades de carga para los DropDownLists
        public List<SelectListItem> CandidatosDisponibles { get; set; } = new();
        public List<SelectListItem> PuestosDisponibles { get; set; } = new();
    }
}