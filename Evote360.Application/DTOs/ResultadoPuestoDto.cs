namespace Evote360.Application.DTOs
{
    public class ResultadoPuestoDto
    {
        public int PuestoId { get; set; }
        public string PuestoNombre { get; set; } = null!;
        public List<ResultadoOpcionDto> Opciones { get; set; } = new();
        public bool ExisteEmpatePrimerLugar { get; set; }
    }
    
}