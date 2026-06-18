namespace Evote360.Application.DTOs.Ciudadano
{
    public class CiudadanoDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public bool HaVotado { get; set; }
        public bool Estado { get; set; }
    }

}