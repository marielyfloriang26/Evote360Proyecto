using Evote360.Core.Enums;

namespace Evote360.Application.ViewModels.Usuario;

public class UsuarioViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string NombreUsuario { get; set; } = null!;
    public RolUsuarioEnum Rol { get; set; } 
    public bool Estado { get; set; }
}