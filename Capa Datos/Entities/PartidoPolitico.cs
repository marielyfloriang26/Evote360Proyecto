using System.Collections.Generic;

namespace Capa_Datos.Entities
{
    public class PartidoPolitico
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Siglas { get; set; } = null!;
        public string LogoUrl { get; set; } = null!;
        public bool Estado { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Candidato> Candidatos { get; set; } = new List<Candidato>();
        public virtual ICollection<AsignacionDirigente> AsignacionesDirigentes { get; set; } = new List<AsignacionDirigente>();
        public virtual ICollection<AlianzaPolitica> AlianzasComoMayorista { get; set; } = new List<AlianzaPolitica>();
        public virtual ICollection<AlianzaPolitica> AlianzasComoAliado { get; set; } = new List<AlianzaPolitica>();
    }
}
