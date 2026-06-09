using Evote360.Application.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evote360.Application.DTOs;
using Evote360.Core.Interfaces;

namespace Evote360.Application.Services.Implementations
{
    public class CandidatoService : ICandidatoService
    {
        private readonly ICandidatoRepository _repository;
        public CandidatoService(ICandidatoRepository repository) 
        {
            _repository = repository;
        }
        public async Task<IEnumerable<CandidatoDTO>> GetAllCandidatos()
        {
            var candidatos = await _repository.GetAllAsync();
            return candidatos.Select(c => new CandidatoDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Foto = c.FotoUrl,
                PuestoAsociado = c.,
                Estado = c.Estado
            });
        }

        public Task<IEnumerable<CandidatoDTO>> GetCandidatosActivos()
        {
            throw new NotImplementedException();
        }

        public Task<CandidatoDTO> GetCandidatoById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CandidatoDTO> CreateCandidato(CrearCandidatoDTO candidato)
        {
            throw new NotImplementedException();
        }

        public Task<CandidatoDTO> UpdateCandidato(CandidatoDTO candidato)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AlternarEstadoCandidato(int id)
        {
            throw new NotImplementedException();
        }
    }
}
