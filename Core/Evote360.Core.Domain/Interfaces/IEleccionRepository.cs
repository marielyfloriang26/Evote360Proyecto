using Evote360.Core.Entities;

namespace Evote360.Core.Interfaces
{
    public interface IEleccionRepository : IRepositoryAsync<Eleccion>
    {
        // Si existe una elección activa, el sistema no debe permitir crear, editar, activar ni desactivar.
        Task<bool> ExisteEleccionActivaAsync();
        Task<IReadOnlyList<Eleccion>> ObtenerTodasOrdenadasAsync();
        Task<int> ObtenerCantidadCiudadanosQueVotaronAsync(int eleccionId);
        Task<int> ObtenerCantidadVotosPorOpcionAsync(int eleccionId, int puestoId, int? candidatoId);
        Task<int> ObtenerTotalVotosPorPuestoAsync(int eleccionId, int puestoId);


        Task<List<PuestoElectivo>> ObtenerPuestosActivosAsync();
        Task<List<PartidoPolitico>> ObtenerPartidosActivosAsync();
        Task<List<AsignarCandidatoPuesto>> ObtenerAsignacionesPorPuestoAsync(int puestoId);
        Task<bool> ExisteAsignacionCandidatoAsync(int puestoId, int partidoId);


        // funcionalidades del administrador
        Task<List<int>> ObtenerAniosConEleccionesAsync();
        Task<int> ObtenerCantidadCandidatosRealesPorEleccionAsync(int eleccionId);
        Task<int> ObtenerCantidadPartidosPorEleccionAsync(int eleccionId);
    }
}
