namespace Evote360.Application.ViewModels.AsignacionDirigentePolitico
{
    public class AsignacionDirigentePoliticoViewModel
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string NombreDirigente { get; set; } = null!;
        public string NombreUsuario { get; set; } = null!;
        public int IdPartidoPolitico { get; set; }
        public string NombreDelPartido { get; set; } = null!;
        public string SiglasDelPartido { get; set; } = null!;
        public bool EstadoDelDirigente { get; set; }
        public bool EstadoDelPartido { get; set; }
    }
}
