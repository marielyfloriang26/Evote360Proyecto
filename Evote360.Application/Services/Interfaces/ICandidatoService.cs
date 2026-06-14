using Evote360.Application.DTOs;
using Evote360.Application.ViewModels.Candidatos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Evote360.Application.Services.Interfaces
{
    public interface ICandidatoService
    {
        Task<(bool Success, string ErrorMessage, IEnumerable<CandidatoViewModel>? Candidatos, bool HasActiveElection)> GetIndexDataAsync(int userId);
        Task<(bool Success, string ErrorMessage)> ValidateCreateAccessAsync(int userId);
        Task<(bool Success, string ErrorMessage)> CreateCandidatoAsync(int userId, CrearCandidatoViewModel model);
        Task<(bool Success, string ErrorMessage, EditarCandidatoViewModel? Model)> GetEditarDataAsync(int userId, int id);
        Task<(bool Success, string ErrorMessage)> UpdateCandidatoAsync(int userId, EditarCandidatoViewModel model);
        Task<(bool Success, string ErrorMessage, string NombreCandidato)> GetConfirmacionDataAsync(int userId, int id, bool isActivar);
        Task<(bool Success, string ErrorMessage)> ActivarCandidatoAsync(int userId, int id);
        Task<(bool Success, string ErrorMessage)> DesactivarCandidatoAsync(int userId, int id);
    }
}
