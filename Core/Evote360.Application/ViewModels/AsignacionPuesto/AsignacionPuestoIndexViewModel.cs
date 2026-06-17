namespace Evote360.Application.ViewModels.Asignaciones
{
    public class AsignacionPuestoIndexViewModel
    {
        public int Id { get; set; }
        public string NombreCandidato { get; set; } = string.Empty;
        public string ApellidoCandidato { get; set; } = string.Empty;
        public string PartidoOrigenCandidato { get; set; } = string.Empty;
        public string PuestoElectivoAsociado { get; set; } = string.Empty;
        public string TipoCandidatura { get; set; } = string.Empty; // "Propio" o "Aliado"
    }
}