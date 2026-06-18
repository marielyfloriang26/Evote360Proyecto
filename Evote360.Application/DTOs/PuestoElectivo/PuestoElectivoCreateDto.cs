namespace Evote360.Application.DTOs.PuestoElectivo
{
    public class PuestoElectivoCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Estado { get; set; }
    }
} 