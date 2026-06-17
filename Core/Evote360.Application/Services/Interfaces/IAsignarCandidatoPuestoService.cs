using Evote360.Application.ViewModels.Asignaciones;

namespace Evote360.Application.Services.Interfaces
{
    public interface IAsignarCandidatoPuestoService
    {
        Task<(bool Success, string ErrorMessage, AsignacionPuestoListViewModel Data)> GetIndexDataAsync(int userId);
        Task<(bool Success, string ErrorMessage, CrearAsignacionViewModel Form)> GetFormFieldsAsync(int userId);
        Task<(bool Success, string ErrorMessage)> RegistrarAsignacionAsync(int userId, CrearAsignacionViewModel model);
        Task<(bool Success, string ErrorMessage)> EliminarAsignacionAsync(int userId, int asignacionId);
    }
}