using System.Collections.Generic;

namespace Evote360.Application.ViewModels.Dirigente
{
    public class HomeDirigenteViewModel
    {
        // Información del Partido
        public int PartidoId { get; set; }
        public string NombrePartido { get; set; } = null!;
        public string Siglas { get; set; } = null!;
        public string LogoUrl { get; set; } = null!;

        // Indicadores requeridos
        public int CantidadCandidatosActivos { get; set; }
        public int CantidadCandidatosInactivos { get; set; }
        public int CantidadAlianzasPoliticas { get; set; }
        public int CantidadSolicitudesPendientes { get; set; }
        public int CantidadCandidatosAsignados { get; set; }
        
        // Control de errores de los prerrequisitos
        public string? ErrorAcceso { get; set; }
    }
}