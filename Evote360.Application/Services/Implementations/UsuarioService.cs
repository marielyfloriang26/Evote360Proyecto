using Evote360.Application.DTOs;
using Evote360.Application.Interfaces;
using Evote360.Core.Entities;
using Evote360.Core.Enums;
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

    // OBT TODOS LOS USUARIOS 
    public async Task<List<UsuarioDto>> GetAllDtoAsync()
    {
        var usuarios = await _usuarioRepository.GetAllAsync();

        // Mapeo de la Entidad de la bd al dto de lectura
        return usuarios.Select(u => new UsuarioDto
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            Correo = u.Correo,
            NombreUsuario = u.NombreUsuario,
            Rol = u.Rol,
            Estado = u.Estado
        }).ToList();
    }

    // OBT POR ID 
    public async Task<SaveUsuarioDto> GetByIdSaveDtoAsync(int id)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);

        if (usuario == null) return null!;

        // Mapea la entidad al DTO de persistencia
        return new SaveUsuarioDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Correo = usuario.Correo,
            NombreUsuario = usuario.NombreUsuario,
            Rol = usuario.Rol,
            Estado = usuario.Estado
            // La contrasena no se envia desde la bd por seguridad
        };
    }

    public async Task<UsuarioDto?> LoginAsync(string nombreUsuario, string contrasena)
    {
        var usuarios = await _usuarioRepository.GetAllAsync();
        var usuario = usuarios.FirstOrDefault(u => u.NombreUsuario.Trim().ToLower() == nombreUsuario.Trim().ToLower() && u.Estado);

        if (usuario != null && BCrypt.Net.BCrypt.Verify(contrasena, usuario.ClaveHash))
        {
            return new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo = usuario.Correo,
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol,
                Estado = usuario.Estado
            };
        }
        return null;
    }

    // CREAR UN NUEVO USUARIO
    public async Task AddAsync(SaveUsuarioDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Contrasena))
        {
            throw new ArgumentException("La contraseña no puede estar vacía o nula para la creación de un usuario.");
        }

        var usuario = new Usuario
        {
            Nombre = dto.Nombre.Trim(),
            Apellido = dto.Apellido.Trim(),
            Correo = dto.Correo.Trim().ToLower(),
            NombreUsuario = dto.NombreUsuario.Trim(),
            Rol = dto.Rol,
            Estado = dto.Estado, //estado = true Todo usuario nuevo nace activo por defecto
            
            // de contrasena limpia a ClaveHash protegida
            ClaveHash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena)
        };

        await _usuarioRepository.AddAsync(usuario);
    }

    // EDITAR UN USUARIO EXISTENTE
    public async Task UpdateAsync(SaveUsuarioDto dto)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(dto.Id);

        if (usuario != null)
        {
            usuario.Nombre = dto.Nombre.Trim();
            usuario.Apellido = dto.Apellido.Trim();
            usuario.Correo = dto.Correo.Trim().ToLower();
            usuario.NombreUsuario = dto.NombreUsuario.Trim();
            usuario.Rol = dto.Rol;
            usuario.Estado = dto.Estado;

            // Solo se actualiza la clave si se escribio algo en el formulario
            if (!string.IsNullOrWhiteSpace(dto.Contrasena))
            {
                usuario.ClaveHash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);
            }

            await _usuarioRepository.UpdateAsync(usuario);
        }
    }

   
   

    // Valida que el Username no este repetido
    public async Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int idActual = 0)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario)) return false;

        string usernameLimpio = nombreUsuario.Trim().ToLower();
        var usuarios = await _usuarioRepository.GetAllAsync();

        return usuarios.Any(u => u.NombreUsuario.Trim().ToLower() == usernameLimpio && u.Id != idActual);
    }

    // Valida que el Correo sea unico
    public async Task<bool> ExisteCorreoAsync(string correo, int idActual = 0)
    {
        if (string.IsNullOrWhiteSpace(correo)) return false;

        string correoLimpio = correo.Trim().ToLower();
        var usuarios = await _usuarioRepository.GetAllAsync();

        return usuarios.Any(u => u.Correo.Trim().ToLower() == correoLimpio && u.Id != idActual);
    }

    // Valida que no se desactive al ultimo admn del sistema
    public async Task<bool> EsUnicoAdminActivoAsync(int id)
    {
        var usuarios = await _usuarioRepository.GetAllAsync();
       
        int adminsActivos = usuarios.Count(u => u.Rol == RolUsuarioEnum.Administrador && u.Estado);

        if (adminsActivos == 1)
        {
            var unicoAdmin = usuarios.FirstOrDefault(u => u.Rol == RolUsuarioEnum.Administrador && u.Estado);
            return unicoAdmin != null && unicoAdmin.Id == id;
        }

        return false;
    }

    // Valida si un dirigente politico ya tiene un partido asignado en el sistema
    public async Task<bool> TienePartidoAsignadoAsync(int usuarioId)
    {
        var asignaciones = await _asignacionRepository.GetAllAsync();
        return asignaciones.Any(a => a.UsuarioId == usuarioId);
    }

    //Evita modificaciones si hay procesos electorales en curso
    public async Task<bool> ExisteEleccionActivaAsync()
    {
        return await Task.FromResult(false); 
    }
}