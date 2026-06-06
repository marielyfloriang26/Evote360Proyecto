using System.Collections.Generic;

namespace Evote360.Core.Entities
{
    public class PuestoElectivo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public bool Estado { get; set; } = true;

        // Navigation properties
        public virtual ICollection<AsignarCandidatoPuesto> AsignacionesCandidatos { get; set; } = new List<AsignarCandidatoPuesto>();
        public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();
    }
}
