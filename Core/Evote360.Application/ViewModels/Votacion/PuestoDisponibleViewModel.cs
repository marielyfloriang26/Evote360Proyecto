namespace Evote360.Application.ViewModels.Votacion
{
    public class PuestoDisponibleViewModel
    {
        public int PuestoId { get; set; }
        public string NombrePuesto { get; set; } = null!;
        public int PartidosParticipantes { get; set; }
        public int CandidatosReales { get; set; }
        public bool YaSelecciono { get; set; }
    }
}
