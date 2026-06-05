using System;

namespace Capa_Datos.Entities
{
    public class AsignacionDirigente
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int PartidoId { get; set; }
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Usuario Usuario { get; set; } = null!;
        public virtual PartidoPolitico Partido { get; set; } = null!;
    }
}
