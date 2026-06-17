using Evote360.Core.Common;
namespace Evote360.Core.Entities
{
    public class AsignarCandidatoPuesto : BaseEntity
    {
        public int CandidatoId { get; set; }
        public int PuestoId { get; set; }
        public int EleccionId { get; set; }

        // Navigation properties
        public virtual Candidato Candidato { get; set; } = null!;
        public virtual PuestoElectivo Puesto { get; set; } = null!;
        public virtual Eleccion Eleccion { get; set; } = null!;
    }
}
