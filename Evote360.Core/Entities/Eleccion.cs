using Evote360.Core.Common;
using System;
using System.Collections.Generic;

namespace Evote360.Core.Entities
{
    public class Eleccion : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string EstadoElectoral { get; set; } = EstadosEleccion.Pendiente;

        // Navigation properties
        public virtual ICollection<AlianzaPolitica> Alianzas { get; set; } = new List<AlianzaPolitica>();
        public virtual ICollection<AsignarCandidatoPuesto> AsignacionesCandidatos { get; set; } = new List<AsignarCandidatoPuesto>();
        public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();
        public virtual ICollection<CodigoVerificacion> CodigosVerificacion { get; set; } = new List<CodigoVerificacion>();
    }
}
