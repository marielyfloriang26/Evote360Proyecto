using Evote360.Core.Entities;

namespace Evote360.Core.Interfaces
{
    public interface IPuestoElectivoRepository : IRepositoryAsync<PuestoElectivo>
    {
        // Regla: El nombre del puesto no debe repetirse. Necesitamos buscar por nombre.
        Task<PuestoElectivo?> GetByNombreAsync(string nombre);

        // Regla: No se debe permitir desactivar un puesto electivo si tiene candidatos activos asignados.
        Task<bool> TieneCandidatosActivosAsignadosAsync(int puestoId);

        // Regla: Si ya fue utilizado en una elección activa o finalizada, no se permite modificar su nombre.
        Task<bool> FueUtilizadoEnEleccionAsync(int puestoId);
    }
}
