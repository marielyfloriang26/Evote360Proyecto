using Evote360.Application.DTOs;
using Evote360.Application.Services.Interfaces;
using Evote360.Core.Interfaces;
using Evote360.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Evote360.Core.Interfaces.ICandidatoRepository;

namespace Evote360.Application.Services.Implementations
{
    public class CandidatoService : ICandidatoService
    {
        private readonly ICandidatoRepository _repository;
        private readonly IEleccionRepository _eleccionRepository;

        public CandidatoService(ICandidatoRepository repository, IEleccionRepository eleccionRepository) 
        {
            _repository = repository;
            _eleccionRepository = eleccionRepository;
        }

        public async Task<IEnumerable<CandidatoDTO>> GetAllCandidatos()
        {
            var candidatos = await _repository.GetAllAsync();
            return candidatos.Select(c => new CandidatoDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                FotoUrl = c.FotoUrl,
                PuestoAsociado = c.AsignacionesPuestos?.FirstOrDefault()?.Puesto?.Nombre ?? "Sin puesto asociado",
                Estado = c.Estado == EstadoEnum.Activo
            });
        }

        public async Task<IEnumerable<CandidatoDTO>> GetAllByPartidoAsync(int partidoId)
        {
            var candidatos = await _repository.GetAllAsync();
            return candidatos.Where(c => c.PartidoId == partidoId).Select(c => new CandidatoDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                FotoUrl = c.FotoUrl,
                PuestoAsociado = c.AsignacionesPuestos?.FirstOrDefault()?.Puesto?.Nombre ?? "Sin puesto asociado",
                Estado = c.Estado == EstadoEnum.Activo
            });
        }

        public async Task<IEnumerable<CandidatoDTO>> GetCandidatosActivos()
        {
            var candidatos = await _repository.GetAllAsync();
            return candidatos.Where(c => c.Estado == EstadoEnum.Activo).Select(c => new CandidatoDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                FotoUrl = c.FotoUrl,
                PuestoAsociado = c.AsignacionesPuestos?.FirstOrDefault()?.Puesto?.Nombre ?? "Sin puesto asociado",
                Estado = c.Estado == EstadoEnum.Activo
            });
        }

        public async Task<CandidatoDTO?> GetCandidatoById(int id)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return null;

            return new CandidatoDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                FotoUrl = c.FotoUrl,
                PuestoAsociado = c.AsignacionesPuestos?.FirstOrDefault()?.Puesto?.Nombre ?? "Sin puesto asociado",
                Estado = c.Estado == EstadoEnum.Activo
            };
        }

        public async Task<CandidatoDTO> CreateCandidato(CrearCandidatoDTO candidatoDto)
        {
            var candidato = new Core.Entities.Candidato
            {
                Nombre = candidatoDto.Nombre,
                Apellido = candidatoDto.Apellido,
                PartidoId = candidatoDto.PartidoId,
                Estado = candidatoDto.Estado ? EstadoEnum.Activo : EstadoEnum.Inactivo,
                FotoUrl = candidatoDto.Foto != null ? "dummy" : null
            };

            await _repository.AddAsync(candidato);

            return new CandidatoDTO
            {
                Id = candidato.Id,
                Nombre = candidato.Nombre,
                Apellido = candidato.Apellido,
                Estado = candidato.Estado == EstadoEnum.Activo,
                PuestoAsociado = "Sin puesto asociado"
            };
        }

        public async Task<CandidatoDTO?> UpdateCandidato(CandidatoDTO candidatoDto)
        {
            var candidato = await _repository.GetByIdAsync(candidatoDto.Id);
            if (candidato == null) return null;

            candidato.Nombre = candidatoDto.Nombre;
            candidato.Apellido = candidatoDto.Apellido;
            candidato.Estado = candidatoDto.Estado ? EstadoEnum.Activo : EstadoEnum.Inactivo;
            candidato.FotoUrl = candidatoDto.FotoUrl ?? candidato.FotoUrl;

            await _repository.UpdateAsync(candidato);

            return new CandidatoDTO
            {
                Id = candidato.Id,
                Nombre = candidato.Nombre,
                Apellido = candidato.Apellido,
                FotoUrl = candidato.FotoUrl,
                PuestoAsociado = candidato.AsignacionesPuestos?.FirstOrDefault()?.Puesto?.Nombre ?? "Sin puesto asociado",
                Estado = candidato.Estado == EstadoEnum.Activo
            };
        }

        public async Task<bool> AlternarEstadoCandidato(int id)
        {
            var candidato = await _repository.GetByIdAsync(id);
            if (candidato == null) return false;

            candidato.Estado = candidato.Estado == EstadoEnum.Activo ? EstadoEnum.Inactivo : EstadoEnum.Activo;
            await _repository.UpdateAsync(candidato);
            return true;
        }

        public async Task<bool> HasActiveElectionAsync()
        {
            return await _eleccionRepository.ExisteEleccionActivaAsync();
        }

        public async Task<bool> HasParticipatedInElectionAsync(int candidatoId)
        {
            return await _repository.HasParticipatedInElectionAsync(candidatoId);
        }

        public async Task<bool> HasAssignedPuestoVigenteAsync(int candidatoId)
        {
            return await _repository.HasAssignedPuestoVigenteAsync(candidatoId);
        }
    }
}
