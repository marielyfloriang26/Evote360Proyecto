using Evote360.Core.Common;
using System;

namespace Evote360.Core.Entities
{
    public class AsignacionDirigente : BaseEntity
    {
        public int UsuarioId { get; set; }
        public int PartidoId { get; set; }
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Usuario Usuario { get; set; } = null!;
        public virtual PartidoPolitico Partido { get; set; } = null!;
    }
}
