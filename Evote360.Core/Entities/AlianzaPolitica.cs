namespace Evote360.Core.Entities
{
    public class AlianzaPolitica
    {
        public int Id { get; set; }
        public int EleccionId { get; set; }
        public int PartidoMayoristaId { get; set; }
        public int PartidoAliadoId { get; set; }
        public bool Estado { get; set; } = true;

        // Navigation properties
        public virtual Eleccion Eleccion { get; set; } = null!;
        public virtual PartidoPolitico PartidoMayorista { get; set; } = null!;
        public virtual PartidoPolitico PartidoAliado { get; set; } = null!;
    }
}
