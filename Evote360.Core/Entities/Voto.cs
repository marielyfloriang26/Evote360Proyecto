using Evote360.Core.Common;
namespace Evote360.Core.Entities
{
    public class Voto : BaseEntity
    {
        public int EleccionId { get; set; }
        public int PuestoId { get; set; }
        public int? CandidatoId { get; set; } // Nullable to represent "Ninguno"

        // Navigation properties
        public virtual Eleccion Eleccion { get; set; } = null!;
        public virtual PuestoElectivo Puesto { get; set; } = null!;
        public virtual Candidato? Candidato { get; set; }
    }
}
