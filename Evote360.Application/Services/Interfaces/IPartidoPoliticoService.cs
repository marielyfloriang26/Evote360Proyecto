using Evote360.Application.DTOs;
using Evote360.Application.ViewModels;

namespace Evote360.Application.Interfaces;
    public interface IPartidoPoliticoService
    {
        Task<List<PartidoPoliticoDTO>> GetAllDtoAsync();

        Task<PartidoPoliticoSaveDto> GetByIdSaveDtoAsync(int id);

        Task <PartidoPoliticoSaveDto>AddAsync(PartidoPoliticoSaveDto dto);

        Task UpdateAsync(PartidoPoliticoSaveDto dto);

        Task DeleteAsync(int id);
        Task<bool> ExisteSiglasAsync(string siglas, int idActual = 0);
        Task<bool> TieneDirigenteActivoAsync(int partidoId);
        Task<bool> TieneCandidatosActivosAsync(int partidoId);
       
    }
