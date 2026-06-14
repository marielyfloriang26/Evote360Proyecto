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

    // 1. OBTENER TODOS LOS USUARIOS (Retorna DTOs de lectura, sin contraseñas)
    public async Task<List<UsuarioDto>> GetAllDtoAsync()
    {
        var usuarios = await _usuarioRepository.GetAllAsync();

        // Mapeo manual de la Entidad de la BD al DTO plano de lectura
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

    // 2. OBTENER POR ID (Retorna el SaveUsuarioDto para cargar los campos al editar)
    public async Task<SaveUsuarioDto> GetByIdSaveDtoAsync(int id)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);

        if (usuario == null) return null!;

        // Mapeamos la entidad al DTO de persistencia
        return new SaveUsuarioDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Correo = usuario.Correo,
            NombreUsuario = usuario.NombreUsuario,
            Rol = usuario.Rol,
            Estado = usuario.Estado
            // Nota: La contraseña no se envía desde la BD por seguridad
        };
    }

    // 3. CREAR UN NUEVO USUARIO (Recibe el DTO con la Contrasena limpia)
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

    // 4. EDITAR UN USUARIO EXISTENTE
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

            // Regla del PDF: Solo se actualiza la clave si se escribió algo en el formulario
            if (!string.IsNullOrWhiteSpace(dto.Contrasena))
            {
                usuario.ClaveHash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);
            }

            await _usuarioRepository.UpdateAsync(usuario);
        }
    }

   
   

    // Validar que el Username no este repetido
    public async Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int idActual = 0)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario)) return false;

        string usernameLimpio = nombreUsuario.Trim().ToLower();
        var usuarios = await _usuarioRepository.GetAllAsync();

        return usuarios.Any(u => u.NombreUsuario.Trim().ToLower() == usernameLimpio && u.Id != idActual);
    }

    // Validar que el Correo sea único
    public async Task<bool> ExisteCorreoAsync(string correo, int idActual = 0)
    {
        if (string.IsNullOrWhiteSpace(correo)) return false;

        string correoLimpio = correo.Trim().ToLower();
        var usuarios = await _usuarioRepository.GetAllAsync();

        return usuarios.Any(u => u.Correo.Trim().ToLower() == correoLimpio && u.Id != idActual);
    }

    // Validar que no se desactive al último Administrador del sistema
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

    // Validar si un dirigente político ya tiene un partido asignado en el sistema
    public async Task<bool> TienePartidoAsignadoAsync(int usuarioId)
    {
        var asignaciones = await _asignacionRepository.GetAllAsync();
        return asignaciones.Any(a => a.UsuarioId == usuarioId);
    }

    // Bloqueo de seguridad: Evita modificaciones si hay procesos electorales en curso
    public async Task<bool> ExisteEleccionActivaAsync()
    {
        return await Task.FromResult(false); // Simulado temporalmente
    }
}