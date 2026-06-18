using Evote360.Core.Common;
namespace Evote360.Core.Entities
{
    public class AlianzaPolitica : BaseEntity
    {
        public int EleccionId { get; set; }
        public int PartidoMayoristaId { get; set; }
        public int PartidoAliadoId { get; set; }
        public DateTime FechaSolicitud {get; set;}
        public DateTime? FechaAceptacion {get; set;}

        // Navigation properties
        public virtual Eleccion Eleccion { get; set; } = null!;
        public virtual PartidoPolitico PartidoMayorista { get; set; } = null!;
        public virtual PartidoPolitico PartidoAliado { get; set; } = null!;
    }
}
