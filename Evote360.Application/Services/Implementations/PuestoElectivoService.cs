using Evote360.Application.DTOs;
using Evote360.Application.Services.Interfaces;
using Evote360.Core.Entities;
using Evote360.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote360.Application.Services
{
    public class PuestoElectivoService : IPuestoElectivoService
    {
        private readonly IPuestoElectivoRepository _repository;
        private readonly IEleccionRepository _eleccionRepository;

        public PuestoElectivoService(IPuestoElectivoRepository repository, IEleccionRepository eleccionRepository)
        {
            _repository = repository;
            _eleccionRepository = eleccionRepository;
        }

        public async Task<bool> ExisteEleccionActivaAsync()
        {
            return await _eleccionRepository.ExisteEleccionActivaAsync();
        }

        public async Task<bool> FueUtilizadoEnEleccionAsync(int id)
        {
            return await _repository.FueUtilizadoEnEleccionAsync(id);
        }

        public async Task<IEnumerable<PuestoElectivoDto>> ObtenerTodosAsync()
        {
            var puestos = await _repository.GetAllAsync();
            return puestos.Select(p => new PuestoElectivoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Estado = p.Estado
            });
        }

        public async Task<PuestoElectivoUpdateDto?> ObtenerPorIdParaEditarAsync(int id)
        {
            var puesto = await _repository.GetByIdAsync(id);
            if (puesto == null) return null;

            return new PuestoElectivoUpdateDto
            {
                Id = puesto.Id,
                Nombre = puesto.Nombre,
                Descripcion = puesto.Descripcion,
                Estado = puesto.Estado
            };
        }

        public async Task<(bool Success, string Message)> CrearAsync(PuestoElectivoCreateDto dto)
        {
            if (await ExisteEleccionActivaAsync())
                return (false, "No se puede crear un puesto electivo mientras exista una elección activa.");

            string nombreLimpio = dto.Nombre.Trim();

            var puestoExistente = await _repository.GetByNombreAsync(nombreLimpio);
            if (puestoExistente != null)
                return (false, "Ya existe un puesto electivo registrado con este nombre.");

            var puesto = new PuestoElectivo
            {
                Nombre = nombreLimpio,
                Descripcion = dto.Descripcion.Trim(),
                Estado = true // Por defecto activo según el PDF
            };

            await _repository.AddAsync(puesto);
            return (true, "Puesto electivo creado con éxito.");
        }

        public async Task<(bool Success, string Message)> EditarAsync(PuestoElectivoUpdateDto dto)
        {
            if (await ExisteEleccionActivaAsync())
                return (false, "No se puede editar un puesto electivo mientras exista una elección activa.");

            var puesto = await _repository.GetByIdAsync(dto.Id);
            if (puesto == null) return (false, "El puesto electivo no existe.");

            string nombreLimpio = dto.Nombre.Trim();

            // Validación histórica: si ya fue usado en elecciones pasadas o activas
            bool yaFueUtilizado = await _repository.FueUtilizadoEnEleccionAsync(dto.Id);
            if (yaFueUtilizado && puesto.Nombre.ToLower() != nombreLimpio.ToLower())
                return (false, "No se puede modificar el nombre de este puesto electivo porque ya fue utilizado en una elección.");

            // Validar que no se duplique con otro id diferente
            var puestoConMismoNombre = await _repository.GetByNombreAsync(nombreLimpio);
            if (puestoConMismoNombre != null && puestoConMismoNombre.Id != dto.Id)
                return (false, "Ya existe un puesto electivo registrado con este nombre.");

            if (!yaFueUtilizado) puesto.Nombre = nombreLimpio;
            puesto.Descripcion = dto.Descripcion.Trim();
            puesto.Estado = dto.Estado;

            await _repository.UpdateAsync(puesto);
            return (true, "Puesto electivo actualizado con éxito.");
        }

        public async Task<(bool Success, string Message)> ActivarAsync(int id)
        {
            if (await ExisteEleccionActivaAsync())
                return (false, "No se puede activar un puesto electivo mientras exista una elección activa.");

            var puesto = await _repository.GetByIdAsync(id);
            if (puesto == null) return (false, "El puesto electivo no existe.");

            if (puesto.Estado)
                return (false, "Este puesto electivo ya se encuentra activo.");

            var puestoDuplicado = await _repository.GetByNombreAsync(puesto.Nombre);
            if (puestoDuplicado != null && puestoDuplicado.Id != id && puestoDuplicado.Estado)
                return (false, "No se puede activar porque ya existe otro puesto activo con el mismo nombre.");

            puesto.Estado = true;
            await _repository.UpdateAsync(puesto);
            return (true, "Puesto electivo activado con éxito.");
        }

        public async Task<(bool Success, string Message)> DesactivarAsync(int id)
        {
            if (await ExisteEleccionActivaAsync())
                return (false, "No se puede desactivar un puesto electivo mientras exista una elección activa.");

            var puesto = await _repository.GetByIdAsync(id);
            if (puesto == null) return (false, "El puesto electivo no existe.");

            if (!puesto.Estado)
                return (false, "Este puesto electivo ya se encuentra inactivo.");

            if (await _repository.TieneCandidatosActivosAsignadosAsync(id))
                return (false, "No se puede desactivar este puesto electivo porque tiene candidatos activos asignados.");

            puesto.Estado = false;
            await _repository.UpdateAsync(puesto);
            return (true, "Puesto electivo desactivado con éxito.");
        }
    }
}