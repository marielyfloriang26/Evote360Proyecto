using Evote360.Application.DTOs;
using Evote360.Application.ViewModels;

namespace Evote360.Application.Interfaces;
    public interface IPartidoPoliticoService
    {
        // Para listar todos los partidos 
        Task<List<PartidoPoliticoDTO>> GetAllDtoAsync();

        // Para buscar un partido especifico cuando sea a editarlo o ver detalles
        Task<PartidoPoliticoSaveDto> GetByIdSaveDtoAsync(int id);

        // Para guardar el formulario cuando se cree uno nuevo
        Task <PartidoPoliticoSaveDto>AddAsync(PartidoPoliticoSaveDto dto);

        Task UpdateAsync(PartidoPoliticoSaveDto dto);

        Task DeleteAsync(int id);
        Task<bool> ExisteSiglasAsync(string siglas, int idActual = 0);
       
    }
