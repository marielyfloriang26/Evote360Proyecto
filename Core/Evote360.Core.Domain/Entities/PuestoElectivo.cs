using Evote360.Core.Common;
using System.Collections.Generic;

namespace Evote360.Core.Entities
{
    public class PuestoElectivo : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<AsignarCandidatoPuesto> AsignacionesCandidatos { get; set; } = new List<AsignarCandidatoPuesto>();
        public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();
    }
}
