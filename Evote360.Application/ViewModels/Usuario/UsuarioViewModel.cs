namespace Evote360.Application.ViewModels.Usuario;

public class UsuarioViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string NombreUsuario { get; set; } = null!;
    public string Rol { get; set; } = null!;
    public bool Estado { get; set; }
}