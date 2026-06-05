namespace Capa_Datos.Entities
{
    public class Voto
    {
        public int Id { get; set; }
        public int EleccionId { get; set; }
        public int PuestoId { get; set; }
        public int? CandidatoId { get; set; } // Nullable to represent "Ninguno"

        // Navigation properties
        public virtual Eleccion Eleccion { get; set; } = null!;
        public virtual PuestoElectivo Puesto { get; set; } = null!;
        public virtual Candidato? Candidato { get; set; }
    }
}
