namespace Evote360.Core.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = null!;
        public string ClaveHash { get; set; } = null!;
        public string Rol { get; set; } = null!;
        public bool Estado { get; set; } = true;

        // Navigation property
        public virtual AsignacionDirigente? AsignacionDirigente { get; set; }
    }
}
