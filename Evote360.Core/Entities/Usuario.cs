using Evote360.Core.Common;
namespace Evote360.Core.Entities
{
    public class Usuario : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string NombreUsuario { get; set; } = null!;
        public string ClaveHash { get; set; } = null!;
        public string Rol { get; set; } = null!;

        // propiedad navegacion
        public virtual AsignacionDirigente? AsignacionDirigente { get; set; }
    }
}
