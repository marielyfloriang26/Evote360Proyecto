/*using Evote360.Application.Interfaces;

using Evote360.Application.ViewModels.Usuario;
using Evote360.Core.Entities;
using Evote360.Core.Interfaces;

namespace Evote360.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRepositoryAsync<AsignacionDirigente> _asignacionRepository;

    public UsuarioService(
        IUsuarioRepository usuarioRepository, 
        IRepositoryAsync<AsignacionDirigente> asignacionRepository)
    {
        _usuarioRepository = usuarioRepository;
        _asignacionRepository = asignacionRepository;
    }

    // OBTENER TODOS LOS USUARIOS (Para el Index)
    public async Task<List<UsuarioViewModel>> GetAllViewModelAsync()
    {
        var usuarios = await _usuarioRepository.GetAllAsync();

        // Mapeo manual de la entidad al ViewModel de lectura plano
        return usuarios.Select(u => new UsuarioViewModel
        {
            Id = u.Id,
            Nombre = u.NombreUsuario,
            Apellido = u.Apellido,
            Correo = u.Correo,
            NombreUsuario = u.NombreUsuario,
            Rol = u.Rol,
            Estado = u.Estado
        }).ToList();
    }

    // OBTENER POR ID (Para cargar los formularios o confirmaciones)
    public async Task<SaveUsuarioViewModel> GetByIdSaveViewModelAsync(int id)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);

        if (usuario == null) return null!;

        // Mapeo manual al ViewModel de persistencia
        return new SaveUsuarioViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Correo = usuario.Correo,
            NombreUsuario = usuario.NombreUsuario,
            Rol = usuario.Rol,
            Estado = u.Estado
            // Contrasena y ConfirmarContrasena quedan vacías por seguridad
        };
    }

    // CREAR UN NUEVO USUARIO
    public async Task AddAsync(SaveUsuarioViewModel vm)
    {
        var usuario = new Usuario
        {
            Nombre = vm.Nombre.Trim(),
            Apellido = vm.Apellido.Trim(),
            Correo = vm.Correo.Trim().ToLower(),
            NombreUsuario = vm.NombreUsuario.Trim(),
            Rol = vm.Rol,
            Estado = true, // Forzado por defecto según el PDF
            
            // Hasheamos la contraseña de manera segura
            ClaveHash = BCrypt.Net.BCrypt.HashPassword(vm.Contrasena)
        };

        await _usuarioRepository.AddAsync(usuario);
    }

    // EDITAR UN USUARIO EXISTENTE
    public async Task UpdateAsync(SaveUsuarioViewModel vm)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(vm.Id);

        if (usuario != null)
        {
            usuario.Nombre = vm.Nombre.Trim();
            usuario.Apellido = vm.Apellido.Trim();
            usuario.Correo = vm.Correo.Trim().ToLower();
            usuario.NombreUsuario = vm.NombreUsuario.Trim();
            usuario.Rol = vm.Rol;
            usuario.Estado = vm.Estado;

            // Solo se actualiza la clave si el usuario escribió una nueva en el formulario
            if (!string.IsNullOrWhiteSpace(vm.Contrasena))
            {
                usuario.ClaveHash = BCrypt.Net.BCrypt.HashPassword(vm.Contrasena);
            }

            await _usuarioRepository.UpdateAsync(usuario);
        }
    }

    // VALIDACIÓN: Verificar si el Nombre de Usuario está repetido
    public async Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int idActual = 0)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario)) return false;

        string usernameLimpio = nombreUsuario.Trim().ToLower();
        var usuarios = await _usuarioRepository.GetAllAsync();

        return usuarios.Any(u => u.NombreUsuario.Trim().ToLower() == usernameLimpio && u.Id != idActual);
    }

    // VALIDACIÓN: Verificar si el Correo está repetido
    public async Task<bool> ExisteCorreoAsync(string correo, int idActual = 0)
    {
        if (string.IsNullOrWhiteSpace(correo)) return false;

        string correoLimpio = correo.Trim().ToLower();
        var usuarios = await _usuarioRepository.GetAllAsync();

        return usuarios.Any(u => u.Correo.Trim().ToLower() == correoLimpio && u.Id != idActual);
    }

    // VALIDACIÓN: Verificar si es el único Administrador activo del sistema
    public async Task<bool> EsUnicoAdminActivoAsync(int id)
    {
        var usuarios = await _usuarioRepository.GetAllAsync();

        // Cuenta cuántos administradores activos hay en total
        int adminsActivos = usuarios.Count(u => u.Rol == "Administrador" && u.Estado);

        // Si solo hay uno, verifica si coincide con el ID que estamos evaluando
        if (adminsActivos == 1)
        {
            var unicoAdmin = usuarios.FirstOrDefault(u => u.Rol == "Administrador" && u.Estado);
            return unicoAdmin != null && unicoAdmin.Id == id;
        }

        return false;
    }

    // VALIDACIÓN: Verificar si un dirigente político ya tiene un partido asignado
    public async Task<bool> TienePartidoAsignadoAsync(int usuarioId)
    {
        var asignaciones = await _asignacionRepository.GetAllAsync();
        return asignaciones.Any(a => a.UsuarioId == usuarioId);
    }

    // VALIDACIÓN SIMULADA: Elección activa (Se completará cuando se haga el módulo de Elecciones)
    public async Task<bool> ExisteEleccionActivaAsync()
    {
        return await Task.FromResult(false);
    }
} */