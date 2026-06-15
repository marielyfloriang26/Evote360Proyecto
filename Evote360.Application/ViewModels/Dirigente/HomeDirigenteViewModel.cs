using System.ComponentModel.DataAnnotations;

namespace Evote360.Application.ViewModels.Dirigente
{
    public class HomeDirigenteViewModel
    {
        // Información del Partido Político Asignado
        public string NombrePartido { get; set; } = null!;
        public string SiglasPartido { get; set; } = null!;
        public string LogoUrl { get; set; } = null!;

        // Indicadores requeridos 
        public int CantidadCandidatosActivos { get; set; }
        public int CantidadCandidatosInactivos { get; set; }
        public int CantidadAlianzasAprobadas { get; set; }
        public int CantidadSolicitudesAlianzasPendientes { get; set; }
        public int CantidadCandidatosAsignadosPuestos { get; set; }
    }
}