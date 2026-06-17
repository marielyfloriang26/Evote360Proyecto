using Evote360.Application.DTOs.PuestoElectivo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Evote360.Application.Services.Interfaces
{
    public interface IPuestoElectivoService
    {
        Task<IEnumerable<PuestoElectivoDto>> ObtenerTodosAsync();
        Task<PuestoElectivoUpdateDto?> ObtenerPorIdParaEditarAsync(int id);
        Task<bool> FueUtilizadoEnEleccionAsync(int id);
        Task<(bool Success, string Message)> CrearAsync(PuestoElectivoCreateDto dto);
        Task<(bool Success, string Message)> EditarAsync(PuestoElectivoUpdateDto dto);
        Task<(bool Success, string Message)> ActivarAsync(int id);
        Task<(bool Success, string Message)> DesactivarAsync(int id);
        Task<bool> ExisteEleccionActivaAsync();
    }
}