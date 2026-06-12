using Evote360.Application.ViewModels;

namespace Evote360.Application.Interfaces;
    public interface IPartidoPoliticoService
    {
        // Para listar todos los partidos 
        Task<List<PartidoPoliticoViewModel>> GetAllViewModelAsync();

        // Para buscar un partido especifico cuando sea a editarlo o ver detalles
        Task<SavePartidoPoliticoViewModel> GetByIdSaveViewModelAsync(int id);

        // Para guardar el formulario cuando se cree uno nuevo
        Task AddAsync(SavePartidoPoliticoViewModel vm);

        Task UpdateAsync(SavePartidoPoliticoViewModel vm);

        Task<bool> ExisteSiglasAsync(string siglas, int idActual = 0);
       
    }
