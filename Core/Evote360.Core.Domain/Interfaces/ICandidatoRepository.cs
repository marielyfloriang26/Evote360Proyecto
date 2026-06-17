using Evote360.Core.Entities;

namespace Evote360.Core.Interfaces
{
    public interface ICandidatoRepository : IRepositoryAsync<Candidato>
    {
        Task<bool> HasParticipatedInElectionAsync(int candidatoId);
        Task<bool> HasAssignedPuestoVigenteAsync(int candidatoId);
    }
}
