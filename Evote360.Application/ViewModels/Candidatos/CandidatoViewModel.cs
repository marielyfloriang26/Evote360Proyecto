using Evote360.Core.Enums;

namespace Evote360.Application.ViewModels.Candidatos
{
    public class CandidatoViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public string PuestoAsociado { get; set; } = string.Empty;
        public EstadoEnum Estado { get; set; }
    }
}