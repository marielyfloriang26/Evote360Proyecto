using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Evote360.Application.ViewModels.AsignacionDirigentePolitico
{
    public class CrearAsignacionDirigentePoliticoViewModel
    {
        [Required(ErrorMessage = "El dirigente político es requerido.")]
        [Display(Name = "Dirigente Político")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El partido político es requerido.")]
        [Display(Name = "Partido Político")]
        public int PartidoId { get; set; }

        public IEnumerable<SelectListItem> Usuarios { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Partidos { get; set; } = new List<SelectListItem>();
    }
}
