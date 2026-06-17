using Evote360.Core.Common;
using System;

namespace Evote360.Core.Entities
{
    public class CodigoVerificacion : BaseEntity
    {
        public int CiudadanoId { get; set; }
        public int EleccionId { get; set; }
        public string Codigo { get; set; } = null!;
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
        public DateTime FechaExpiracion { get; set; }
        public bool Usado { get; set; } = false;

        // Navigation properties
        public virtual Ciudadano Ciudadano { get; set; } = null!;
        public virtual Eleccion Eleccion { get; set; } = null!;
    }
}
