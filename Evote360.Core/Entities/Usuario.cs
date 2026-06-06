using Evote360.Core.Common;
namespace Evote360.Core.Entities
{
    public class Usuario : BaseEntity
    {
        public string NombreUsuario { get; set; } = null!;
        public string ClaveHash { get; set; } = null!;
        public string Rol { get; set; } = null!;

        // Navigation property
        public virtual AsignacionDirigente? AsignacionDirigente { get; set; }
    }
}
