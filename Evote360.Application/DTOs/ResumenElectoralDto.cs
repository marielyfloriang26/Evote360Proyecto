namespace Evote360.Application.DTOs
{
    public class ResumenElectoralDto
    {
        public string NombreEleccion { get; set; } = null!;
        public DateTime FechaRealizacion { get; set; }
        public int CantidadPartidos { get; set; }
        public int CantidadCandidatos { get; set; }
        public int CantidadCiudadanosVotaron { get; set; }
    }
}