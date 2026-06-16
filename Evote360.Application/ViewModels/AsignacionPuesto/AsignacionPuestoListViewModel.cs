namespace Evote360.Application.ViewModels.Asignaciones
{
    public class AsignacionPuestoListViewModel
    {
        public List<AsignacionPuestoIndexViewModel> Asignaciones { get; set; } = new();
        public bool TieneEleccionActiva { get; set; }
    }
}