using Evote360.Application.Services.Interfaces;
using Evote360.Core.Interfaces;
using Evote360.Core.Entities;
using Evote360.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evote360.Application.DTOs.AsignacionDirigentePolitico;
using Evote360.Application.DTOs.PartidoPolitico;
using Evote360.Application.DTOs.Usuario;

namespace Evote360.Application.Services.Implementations
{
    public class AsignacionDirigentePoliticoService : IAsignacionDirigentePoliticoService
    {
        private readonly IAsignacionDirigenteRepository _asignacionRepository;
        private readonly IEleccionRepository _eleccionRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPartidoPoliticoRepository _partidoRepository;

        public AsignacionDirigentePoliticoService(
            IAsignacionDirigenteRepository asignacionRepository,
            IEleccionRepository eleccionRepository,
            IUsuarioRepository usuarioRepository,
            IPartidoPoliticoRepository partidoRepository)
        {
            _asignacionRepository = asignacionRepository;
            _eleccionRepository = eleccionRepository;
            _usuarioRepository = usuarioRepository;
            _partidoRepository = partidoRepository;
        }

        public async Task<IEnumerable<AsignacionDirigentePoliticoDTO>> ObtenerAsignacionesDirigentesAsync()
        {
            var asignaciones = await _asignacionRepository.GetAllAsync();

            var listaDto = asignaciones.Select(a => new AsignacionDirigentePoliticoDTO
            {
                Id = a.Id,
                IdUsuario = a.UsuarioId,
                NombreDirigente = a.Usuario != null ? a.Usuario.Nombre + " " + a.Usuario.Apellido : "",
                NombreUsuario = a.Usuario?.NombreUsuario ?? "",
                IdPartidoPolitico = a.PartidoId,
                NombreDelPartido = a.Partido?.Nombre ?? "",
                SiglasDelPartido = a.Partido?.Siglas ?? "",
                EstadoDelDirigente = a.Usuario?.Estado ?? false,
                EstadoDelPartido = a.Partido?.Estado ?? false
            });

            return listaDto;
        }

        public async Task<AsignacionDirigentePoliticoDTO> ObtenerAsignacionDirigentePorIdAsync(int id)
        {
            var asignacion = await _asignacionRepository.GetByIdAsync(id);
            if (asignacion == null) return null;
            return new AsignacionDirigentePoliticoDTO
            {
                Id = asignacion.Id,
                IdUsuario = asignacion.UsuarioId,
                NombreDirigente = asignacion.Usuario != null ? asignacion.Usuario.Nombre + " " + asignacion.Usuario.Apellido : "",
                NombreUsuario = asignacion.Usuario?.NombreUsuario ?? "",
                IdPartidoPolitico = asignacion.PartidoId,
                NombreDelPartido = asignacion.Partido?.Nombre ?? "",
                SiglasDelPartido = asignacion.Partido?.Siglas ?? "",
                EstadoDelDirigente = asignacion.Usuario?.Estado ?? false,
                EstadoDelPartido = asignacion.Partido?.Estado ?? false
            };
        }

        public async Task<AsignacionDirigentePoliticoDTO> CrearAsignacionAsync(CrearAsignacionDirPolDTO crearAsignacionDirPolDTO)
        {
            // Validaciones
            var elecciones = await _eleccionRepository.GetAllAsync();
            if (elecciones.Any(e => e.EstadoElectoral == "Activa"))
            {
                throw new InvalidOperationException("No se puede crear una asignación de dirigente político mientras exista una elección activa.");
            }

            var usuario = await _usuarioRepository.GetByIdAsync(crearAsignacionDirPolDTO.UsuarioId);
            if (usuario == null) throw new InvalidOperationException("El usuario seleccionado debe existir.");
            if (!usuario.Estado) throw new InvalidOperationException("El usuario seleccionado debe estar activo.");
            if (usuario.Rol != RolUsuarioEnum.DirigentePolitico) throw new InvalidOperationException("El usuario seleccionado no tiene el rol de dirigente político.");

            var asignacionesExistentes = await _asignacionRepository.GetAllAsync();
            if (asignacionesExistentes.Any(a => a.UsuarioId == crearAsignacionDirPolDTO.UsuarioId))
            {
                throw new InvalidOperationException("Este dirigente ya está relacionado con otro partido político.");
            }

            var partido = await _partidoRepository.GetByIdAsync(crearAsignacionDirPolDTO.PartidoId);
            if (partido == null) throw new InvalidOperationException("El partido político seleccionado debe existir.");
            if (!partido.Estado) throw new InvalidOperationException("El partido político seleccionado debe estar activo.");

            if (asignacionesExistentes.Any(a => a.PartidoId == crearAsignacionDirPolDTO.PartidoId))
            {
                throw new InvalidOperationException("Este partido político ya tiene un dirigente asignado.");
            }

            var nuevaAsignacion = new AsignacionDirigente
            {
                UsuarioId = crearAsignacionDirPolDTO.UsuarioId,
                PartidoId = crearAsignacionDirPolDTO.PartidoId,
                FechaAsignacion = DateTime.Now
            };

            await _asignacionRepository.AddAsync(nuevaAsignacion);

            return new AsignacionDirigentePoliticoDTO
            {
                Id = nuevaAsignacion.Id,
                IdUsuario = nuevaAsignacion.UsuarioId,
                IdPartidoPolitico = nuevaAsignacion.PartidoId
            };
        }

        public async Task EliminarAsignacionAsync(int id)
        {
            var elecciones = await _eleccionRepository.GetAllAsync();
            if (elecciones.Any(e => e.EstadoElectoral == "Activa"))
            {
                throw new InvalidOperationException("No se puede eliminar una asignación de dirigente político mientras exista una elección activa.");
            }

            var asignacion = await _asignacionRepository.GetByIdAsync(id);
            if (asignacion == null)
            {
                throw new InvalidOperationException("La asignación seleccionada no existe o ya fue eliminada.");
            }

            await _asignacionRepository.DeleteAsync(asignacion);
        }

        public async Task<bool> HayEleccionActivaAsync()
        {
            var elecciones = await _eleccionRepository.GetAllAsync();
            return elecciones.Any(e => e.EstadoElectoral == "Activa");
        }

        public async Task<IEnumerable<UsuarioDto>> ObtenerUsuariosDirigentesDisponiblesAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            var asignaciones = await _asignacionRepository.GetAllAsync();
            var asignadosIds = asignaciones.Select(a => a.UsuarioId).ToHashSet();

            return usuarios
                .Where(u => u.Estado && u.Rol == RolUsuarioEnum.DirigentePolitico && !asignadosIds.Contains(u.Id))
                .Select(u => new UsuarioDto
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    NombreUsuario = u.NombreUsuario,
                    Correo = u.Correo,
                    Rol = u.Rol,
                    Estado = u.Estado
                });
        }

        public async Task<IEnumerable<PartidoPoliticoDTO>> ObtenerPartidosDisponiblesAsync()
        {
            var partidos = await _partidoRepository.GetAllAsync();
            var asignaciones = await _asignacionRepository.GetAllAsync();
            var asignadosIds = asignaciones.Select(a => a.PartidoId).ToHashSet();

            return partidos
                .Where(p => p.Estado && !asignadosIds.Contains(p.Id))
                .Select(p => new PartidoPoliticoDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Siglas = p.Siglas,
                    LogoUrl = p.LogoUrl,
                    Estado = p.Estado
                });
        }
    }
} 
