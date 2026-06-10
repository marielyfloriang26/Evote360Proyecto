using Evote360.Application.Interfaces;
using Evote360.Application.ViewModels;
using Evote360.Core.Entities;
using Evote360.Core.Interfaces;

namespace Evote360.Application.Services;
    public class PartidoPoliticoService : IPartidoPoliticoService
    {
        private readonly IPartidoPoliticoRepository _partidoRepository;

        public PartidoPoliticoService(IPartidoPoliticoRepository partidoRepository)
        {
            _partidoRepository = partidoRepository;
        }

        // OBTENER TODOS LOS PARTIDOS 
        public async Task<List<PartidoPoliticoViewModel>> GetAllViewModelAsync()
        {
            var partidos = await _partidoRepository.GetAllAsync();

            // Mapeo manual de la entidad al ViewModel de lectura
            return partidos.Select(p => new PartidoPoliticoViewModel
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Siglas = p.Siglas,
                LogoUrl = p.LogoUrl,
                Descripcion = p.Descripcion,
                Estado = p.Estado
            }).ToList();
        }

        // OBTENER POR ID 
        public async Task<SavePartidoPoliticoViewModel> GetByIdSaveViewModelAsync(int id)
        {
            var partido = await _partidoRepository.GetByIdAsync(id);
            
            if (partido == null) return null!;

            // Mapeo manual de la entidad al ViewModel de edicion
            return new SavePartidoPoliticoViewModel
            {
                Id = partido.Id,
                Nombre = partido.Nombre,
                Siglas = partido.Siglas,
                LogoUrl = partido.LogoUrl,
                Descripcion = partido.Descripcion,
                Estado = partido.Estado
            };
        }

        // CREAR UN NUEVO PARTIDO
        public async Task AddAsync(SavePartidoPoliticoViewModel vm)
        {
            // Mapeo manual del ViewModel que viene de la pantalla a la Entidad de la bd
            var partido = new PartidoPolitico
            {
                Nombre = vm.Nombre,
                Siglas = vm.Siglas,
                LogoUrl = vm.LogoUrl,
                Descripcion = vm.Descripcion,
                Estado = vm.Estado
            };

            await _partidoRepository.AddAsync(partido);
        }

        // EDITAR UN PARTIDO EXISTENTE
        public async Task UpdateAsync(SavePartidoPoliticoViewModel vm)
        {
            var partido = await _partidoRepository.GetByIdAsync(vm.Id);

            if (partido != null)
            {
                // Actualiza las propiedades de la entidad con lo que ingreso el usuario
                partido.Nombre = vm.Nombre;
                partido.Siglas = vm.Siglas;
                partido.LogoUrl = vm.LogoUrl;
                partido.Descripcion = vm.Descripcion;
                partido.Estado = vm.Estado;

                await _partidoRepository.UpdateAsync(partido);
            }
        }

        // ELIMINAR PARTIDO
        public async Task DeleteAsync(int id)
        {
            var partido = await _partidoRepository.GetByIdAsync(id);
            if (partido != null)
            {
                await _partidoRepository.DeleteAsync(partido);
            }
        }
    }
