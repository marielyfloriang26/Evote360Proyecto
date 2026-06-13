
/*using Evote360.Application.DTOs;
using Evote360.Application.DTOs;
using Evote360.Core.Enums;
using Evote360.Application.Interfaces;
using Evote360.Application.Services.Interfaces;
using Evote360.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evote360.Application.Services.Implementations
{
    public class AsignacionDirigentePoliticoService
    {
        private readonly IAsignacionDirigenteRepository _asignacionRepository;

        public AsignacionDirigentePoliticoService(IAsignacionDirigenteRepository asignacionRepository)
        {
            _asignacionRepository = asignacionRepository;
        }

        public async Task<IEnumerable<AsignacionDirigentePoliticoDTO>> ObtenerAsignacionesDirigentesAsync()
        {
            var asignaciones = await _asignacionRepository.GetAllAsync();

            var listaDto = asignaciones.Select(a => new AsignacionDirigentePoliticoDTO
            {
                Id = a.Id,
                IdUsuario = a.UsuarioId,
                NombreDirigente = a.Usuario.Nombre + " " + a.Usuario.Apellido,
                NombreUsuario = a.Usuario.NombreUsuario,
                IdPartidoPolitico = a.PartidoId,
                NombreDelPartido = a.Partido.Nombre,
                SiglasDelPartido = a.Partido.Siglas,
                EstadoDelDirigente = a.Usuario.Estado == EstadoEnum.Activo,
                EstadoDelPartido = a.Partido.Estado == EstadoEnum.Activo
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
                NombreDirigente = asignacion.Usuario.Nombre + " " + asignacion.Usuario.Apellido,
                NombreUsuario = asignacion.Usuario.NombreUsuario,
                IdPartidoPolitico = asignacion.PartidoId,
                NombreDelPartido = asignacion.Partido.Nombre,
                SiglasDelPartido = asignacion.Partido.Siglas,
                EstadoDelDirigente = asignacion.Usuario.Estado == EstadoEnum.Activo,
                EstadoDelPartido = asignacion.Partido.Estado == EstadoEnum.Activo
            };
        }

        public async Task<AsignacionDirigentePoliticoDTO> CrearAsignacionAsync(CrearAsignacionDirPolDTO crearAsignacionDirPolDTO)
        {
            return await Task.FromResult<AsignacionDirigentePoliticoDTO?>(null);

        }

    }
} */
