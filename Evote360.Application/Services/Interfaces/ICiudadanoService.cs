using Evote360.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Evote360.Application.Services.Interfaces
{
    public interface ICiudadanoService
    {
        Task<IEnumerable<CiudadanoDto>> ObtenerTodosAsync();
        Task<CiudadanoSaveDto?> ObtenerPorIdAsync(int id);
        Task<(bool Success, string Message)> CrearAsync(CiudadanoSaveDto dto);
        Task<(bool Success, string Message)> EditarAsync(CiudadanoSaveDto dto);
        Task<(bool Success, string Message)> ActivarAsync(int id);
        Task<(bool Success, string Message)> DesactivarAsync(int id);
        Task<bool> ExisteEleccionActivaAsync();
        Task<bool> HaParticipadoEnEleccionesAsync(int id);
    }
}