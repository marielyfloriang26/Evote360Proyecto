using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evote360.Application.DTOs;
using Evote360.Core.Entities;

namespace Evote360.Application.Services.Interfaces
{
    public interface ICandidatoService
    {
        public Task<IEnumerable<CandidatoDTO>> GetAllCandidatos();
        public Task<IEnumerable<CandidatoDTO>> GetAllByPartidoAsync(int partidoId);
        public Task<IEnumerable<CandidatoDTO>> GetCandidatosActivos();
        public Task<CandidatoDTO?> GetCandidatoById(int id);
        public Task<CandidatoDTO> CreateCandidato(CrearCandidatoDTO candidato);
        public Task<CandidatoDTO?> UpdateCandidato(CandidatoDTO candidato);
        public Task<bool> AlternarEstadoCandidato(int id);
        public Task<bool> HasActiveElectionAsync();
        public Task<bool> HasParticipatedInElectionAsync(int candidatoId);
        public Task<bool> HasAssignedPuestoVigenteAsync(int candidatoId);
    }
}
