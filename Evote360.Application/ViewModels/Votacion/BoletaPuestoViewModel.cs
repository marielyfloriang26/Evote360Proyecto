using System.Collections.Generic;

namespace Evote360.Application.ViewModels.Votacion
{
    public class PartidoAliadoViewModel
    {
        public string Nombre { get; set; } = null!;
        public string? LogoUrl { get; set; }
    }

    public class CandidatoBoletaViewModel
    {
        public int CandidatoId { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string? FotoUrl { get; set; }
        public string NombrePartidoPrincipal { get; set; } = null!;
        public string? LogoPartidoPrincipal { get; set; }
        public List<PartidoAliadoViewModel> PartidosAliados { get; set; } = new();
    }

    public class BoletaPuestoViewModel
    {
        public int PuestoId { get; set; }
        public string NombrePuesto { get; set; } = null!;
        public List<CandidatoBoletaViewModel> Candidatos { get; set; } = new();
        public int? CandidatoSeleccionadoId { get; set; }
    }
}
