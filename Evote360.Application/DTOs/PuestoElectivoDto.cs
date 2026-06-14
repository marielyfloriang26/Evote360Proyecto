namespace Evote360.Application.DTOs
{
    public class PuestoElectivoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Estado { get; set; }

        
       public string EstadoTexto => Estado == true ? "Activo" : "Inactivo";
    }
} 