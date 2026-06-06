using Evote360.Core.Common;
using System.Collections.Generic;

namespace Evote360.Core.Entities
{
    public class Candidato : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public int PartidoId { get; set; }
        public string? FotoUrl { get; set; }

        // Navigation properties
        public virtual PartidoPolitico Partido { get; set; } = null!;
        public virtual ICollection<AsignarCandidatoPuesto> AsignacionesPuestos { get; set; } = new List<AsignarCandidatoPuesto>();
        public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();
    }
}
