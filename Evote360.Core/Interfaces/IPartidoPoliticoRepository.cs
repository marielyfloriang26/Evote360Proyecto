using Evote360.Core.Entities;

namespace Evote360.Core.Interfaces
{
    public interface IPartidoPoliticoRepository : IRepositoryAsync<PartidoPolitico>
    {
        Task<PartidoPolitico> GetPartidoConRelacionesOptimizadoAsync(int id);
    }
}
