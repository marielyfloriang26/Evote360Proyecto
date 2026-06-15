using Evote360.Application.DTOs;
using System.Collections.Generic;

namespace Evote360.Application.ViewModels.Admin
{
    public class AdminHomeViewModel
    {
        public int? AnioSeleccionado { get; set; }
        public List<int> AniosDisponibles { get; set; } = new List<int>();
        public List<ResumenElectoralDto> EleccionesResumen { get; set; } = new List<ResumenElectoralDto>();
    }
}