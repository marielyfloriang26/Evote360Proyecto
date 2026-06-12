using System.ComponentModel.DataAnnotations;

namespace Evote360.Application.ViewModels.Usuario;

public class SaveUsuarioViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es requerido.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es requerido.")]
    [MaxLength(100, ErrorMessage = "El apellido no puede exceder los 100 caracteres.")]
    public string Apellido { get; set; } = null!;

    [Required(ErrorMessage = "El correo electrónico es requerido.")]
    [EmailAddress(ErrorMessage = "El correo electrónico debe tener un formato válido.")]
    [MaxLength(150, ErrorMessage = "El correo electrónico no puede exceder los 150 caracteres.")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "El nombre de usuario es requerido.")]
    [MaxLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
    public string NombreUsuario { get; set; } = null!;

    // Es opcional aqui para no romper la edicion si se deja vacio
    public string? Contrasena { get; set; }

    public string? ConfirmarContrasena { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un rol válido para el usuario.")]
    public string Rol { get; set; } = null!;

    public bool Estado { get; set; } = true; // Activo por defecto al crear
}