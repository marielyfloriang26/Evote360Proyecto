using Evote360.Application.DTOs.Ciudadano;
using Evote360.Application.Services.Interfaces;
using Evote360.Core.Common;
using Evote360.Core.Entities;
using Evote360.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote360.Application.Services
{
    public class CiudadanoService : ICiudadanoService
    {
        private readonly ICiudadanoRepository _ciudadanoRepository;
        private readonly IEleccionRepository _eleccionRepository;

        public CiudadanoService(ICiudadanoRepository ciudadanoRepository, IEleccionRepository eleccionRepository)
        {
            _ciudadanoRepository = ciudadanoRepository;
            _eleccionRepository = eleccionRepository;
        }

        public async Task<bool> ExisteEleccionActivaAsync()
        {
            var elecciones = await _eleccionRepository.GetAllAsync();
            return elecciones.Any(e => e.EstadoElectoral == EstadosEleccion.Activa);
        }

        public async Task<bool> HaParticipadoEnEleccionesAsync(int id)
        {
            return await _ciudadanoRepository.HaParticipadoEnEleccionesAsync(id);
        }

        public async Task<IEnumerable<CiudadanoDto>> ObtenerTodosAsync()
        {
            var lista = await _ciudadanoRepository.GetAllAsync();
            return lista.Select(c => new CiudadanoDto
            {
                Id = c.Id,
                Cedula = c.Cedula,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Correo = c.Correo,
                HaVotado = c.HaVotado,
                Estado = c.Estado
            }).ToList();
        }

        public async Task<CiudadanoSaveDto?> ObtenerPorIdAsync(int id)
        {
            var c = await _ciudadanoRepository.GetByIdAsync(id);
            if (c == null) return null;

            return new CiudadanoSaveDto
            {
                Id = c.Id,
                Cedula = c.Cedula,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Correo = c.Correo,
                Estado = c.Estado
            };
        }

        public async Task<(bool Success, string Message)> CrearAsync(CiudadanoSaveDto dto)
        {
            if (await ExisteEleccionActivaAsync())
                return (false, "No se puede crear un ciudadano mientras exista una elección activa.");

            string cedulaLimpia = dto.Cedula.Trim();
            string correoLimpio = dto.Correo.Trim().ToLower();

            if (await _ciudadanoRepository.ObtenerPorCedulaAsync(cedulaLimpia) != null)
                return (false, "Ya existe un ciudadano registrado con este número de documento de identidad.");

            if (await _ciudadanoRepository.ObtenerPorCorreoAsync(correoLimpio) != null)
                return (false, "Ya existe un ciudadano registrado con este correo electrónico.");

            var ciudadano = new Ciudadano
            {
                Cedula = cedulaLimpia,
                Nombre = dto.Nombre.Trim(),
                Apellido = dto.Apellido.Trim(),
                Correo = correoLimpio,
                Estado = true 
            };

            await _ciudadanoRepository.AddAsync(ciudadano);
            return (true, "Ciudadano registrado con éxito.");
        }

        public async Task<(bool Success, string Message)> EditarAsync(CiudadanoSaveDto dto)
        {
            if (await ExisteEleccionActivaAsync())
                return (false, "No se puede editar un ciudadano mientras exista una elección activa.");

            var ciudadano = await _ciudadanoRepository.GetByIdAsync(dto.Id);
            if (ciudadano == null) return (false, "El ciudadano no existe.");

            string cedulaLimpia = dto.Cedula.Trim();
            string correoLimpio = dto.Correo.Trim().ToLower();

            // Restricción de Documento
            bool yaParticipo = await _ciudadanoRepository.HaParticipadoEnEleccionesAsync(dto.Id);
            if (yaParticipo && ciudadano.Cedula != cedulaLimpia)
                return (false, "No se puede modificar el número de documento de identidad de este ciudadano porque ya participó en una elección.");

            // Validacion de documento contra terceros
            var existeCedula = await _ciudadanoRepository.ObtenerPorCedulaAsync(cedulaLimpia);
            if (existeCedula != null && existeCedula.Id != dto.Id)
                return (false, "Ya existe un ciudadano registrado con este número de documento de identidad.");

            // Validacion de correo contra terceros
            var existeCorreo = await _ciudadanoRepository.ObtenerPorCorreoAsync(correoLimpio);
            if (existeCorreo != null && existeCorreo.Id != dto.Id)
                return (false, "Ya existe un ciudadano registrado con este correo electrónico.");

            ciudadano.Nombre = dto.Nombre.Trim();
            ciudadano.Apellido = dto.Apellido.Trim();
            ciudadano.Correo = correoLimpio;
            ciudadano.Estado = dto.Estado;
            
            if (!yaParticipo)
            {
                ciudadano.Cedula = cedulaLimpia;
            }

            await _ciudadanoRepository.UpdateAsync(ciudadano);
            return (true, "Ciudadano actualizado con éxito.");
        }

        public async Task<(bool Success, string Message)> ActivarAsync(int id)
        {
            if (await ExisteEleccionActivaAsync())
                return (false, "No se puede activar un ciudadano mientras exista una elección activa.");

            var ciudadano = await _ciudadanoRepository.GetByIdAsync(id);
            if (ciudadano == null) return (false, "El ciudadano no existe.");

            if (ciudadano.Estado)
                return (false, "Este ciudadano ya se encuentra activo.");

            ciudadano.Estado = true;
            await _ciudadanoRepository.UpdateAsync(ciudadano);
            return (true, "Ciudadano activado con éxito.");
        }

        public async Task<(bool Success, string Message)> DesactivarAsync(int id)
        {
            if (await ExisteEleccionActivaAsync())
                return (false, "No se puede desactivar un ciudadano mientras exista una elección activa.");

            var ciudadano = await _ciudadanoRepository.GetByIdAsync(id);
            if (ciudadano == null) return (false, "El ciudadano no existe.");

            if (!ciudadano.Estado)
                return (false, "Este ciudadano ya se encuentra inactivo.");

            ciudadano.Estado = false;
            await _ciudadanoRepository.UpdateAsync(ciudadano);
            return (true, "Ciudadano desactivado con éxito.");
        }
    }
}