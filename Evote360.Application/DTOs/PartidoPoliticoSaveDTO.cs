using Evote360.Core.Enums;

namespace Evote360.Application.DTOs;

public class PartidoPoliticoSaveDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Siglas { get; set; } = null!;
    public string? LogoUrl { get; set; } 
    public string? Descripcion { get; set; }
    public EstadoEnum Estado { get; set; }
}