using System.Collections.Generic;

namespace Capa_Datos.Entities
{
    public class Ciudadano
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public bool HaVotado { get; set; } = false;
        public bool Estado { get; set; } = true;

        // Navigation properties
        public virtual ICollection<CodigoVerificacion> CodigosVerificacion { get; set; } = new List<CodigoVerificacion>();
    }
}
