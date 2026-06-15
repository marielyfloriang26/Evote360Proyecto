using Evote360.Core.Entities;

namespace Evote360.Core.Interfaces
{
    public interface ICiudadanoRepository : IRepositoryAsync<Ciudadano>
    {
        Task<Ciudadano?> ObtenerPorCedulaAsync(string cedula);
        Task<Ciudadano?> ObtenerPorCorreoAsync(string correo);
        Task<bool> HaParticipadoEnEleccionesAsync(int id);
        Task ResetearEstadoVotacionAsync();
    }
}
