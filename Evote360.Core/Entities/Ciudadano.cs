using Evote360.Core.Common;
using System.Collections.Generic;

namespace Evote360.Core.Entities
{
    public class Ciudadano : BaseEntity
    {
        public string Cedula { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public bool HaVotado { get; set; } = false;

        // Navigation properties
        public virtual ICollection<CodigoVerificacion> CodigosVerificacion { get; set; } = new List<CodigoVerificacion>();
    }
}
