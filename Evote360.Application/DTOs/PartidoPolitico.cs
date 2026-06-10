namespace Evote360.Application.DTOs;
    public class PartidoPoliticoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Siglas { get; set; } = null!;
        public string LogoUrl { get; set; } = null!;
        public string? Descripcion { get; set; } 
        public bool Estado { get; set; }
    }
