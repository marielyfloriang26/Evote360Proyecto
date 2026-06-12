using Evote360.Core.Entities;

namespace Evote360.Core.Interfaces
{
    public interface IEleccionRepository : IRepositoryAsync<Eleccion>
    {
        // Regla global: Si existe una elección activa, el sistema no debe permitir crear, editar, activar ni desactivar.
        Task<bool> ExisteEleccionActivaAsync();
    }
}
