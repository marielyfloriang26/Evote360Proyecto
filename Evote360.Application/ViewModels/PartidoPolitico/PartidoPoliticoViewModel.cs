using Evote360.Core.Enums;

namespace Evote360.Application.ViewModels;
    public class PartidoPoliticoViewModel
    {
        // para listado
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Siglas { get; set; } = null!;
        public string LogoUrl { get; set; } = null!;
        public string? Descripcion { get; set; }
        public EstadoEnum Estado { get; set; }
    }
