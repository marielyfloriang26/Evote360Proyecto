using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Evote360.Application.ViewModels.Administrador
{
    public class HomeAdministradorViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un año para consultar el resumen electoral.")]
        public int? AnioElectoral { get; set; }
        public List<int> AniosDisponibles { get; set; } = new();
        public List<ResumenEleccionViewModel> EleccionesResumen { get; set; } = new();
    }
}