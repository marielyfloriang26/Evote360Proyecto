using Evote360.Application.DTOs.Eleccion;
using Evote360.Application.DTOs.Resultado;
using Evote360.Application.DTOs.ResumenElectoral;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Evote360.Application.Services.Interfaces
{
    public interface IEleccionService
    {
        Task<IEnumerable<EleccionDto>> ObtenerTodasAsync();
        Task<bool> ExisteEleccionActivaAsync();
        Task<(bool Success, List<string> Messages)> CrearAsync(EleccionCreateDto dto);
        Task<(bool Success, List<string> Messages)> ActivarAsync(int id);
        Task<(bool Success, string Message)> FinalizarAsync(int id);
        Task<List<ResultadoPuestoDto>> ObtenerResultadosAsync(int id);
        Task<EleccionDto?> ObtenerPorIdAsync(int id);

        // funcionalidades del administrador
        Task<List<int>> ObtenerAniosConEleccionesAsync();
        Task<List<ResumenElectoralDto>> ObtenerResumenElectoralPorAnioAsync(int anio);
    }
}