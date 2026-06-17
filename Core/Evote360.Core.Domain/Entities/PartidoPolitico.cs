using Evote360.Core.Common;
using System.Collections.Generic;

namespace Evote360.Core.Entities
{
    public class PartidoPolitico : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion {get; set;}
        public string Siglas { get; set; } = null!;
        public string LogoUrl { get; set; } = null!;

        // Navigation properties
        public virtual ICollection<Candidato> Candidatos { get; set; } = new List<Candidato>();
        public virtual ICollection<AsignacionDirigente> AsignacionesDirigentes { get; set; } = new List<AsignacionDirigente>();
        public virtual ICollection<AlianzaPolitica> AlianzasComoMayorista { get; set; } = new List<AlianzaPolitica>();
        public virtual ICollection<AlianzaPolitica> AlianzasComoAliado { get; set; } = new List<AlianzaPolitica>();
    }
}
