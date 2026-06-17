using Evote360.Application.DTOs.PuestoElectivo;
using System.Collections.Generic;

namespace Evote360.Application.ViewModels.PuestoElectivo
{
    public class PuestoElectivoIndexViewModel
    {
        public IEnumerable<PuestoElectivoDto> Puestos { get; set; } = new List<PuestoElectivoDto>();
        public bool ExisteEleccionActiva { get; set; } 
    }
}