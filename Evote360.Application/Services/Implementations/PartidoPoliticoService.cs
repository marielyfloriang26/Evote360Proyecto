using Evote360.Application.DTOs;
using Evote360.Application.Interfaces;
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
        public async Task<List<PartidoPoliticoDTO>> GetAllDtoAsync()
        {
            var partidos = await _partidoRepository.GetAllAsync();

            return partidos.Select(p => new PartidoPoliticoDTO
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
        public async Task<PartidoPoliticoSaveDto> GetByIdSaveDtoAsync(int id)
        {
            var partido = await _partidoRepository.GetByIdAsync(id);
            
            if (partido == null) return null!;

            return new PartidoPoliticoSaveDto
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
        public async Task<PartidoPoliticoSaveDto> AddAsync(PartidoPoliticoSaveDto dto)
        {
            // Mapeo manual del ViewModel que viene de la pantalla a la Entidad de la bd
            var partido = new PartidoPolitico
            {
                Nombre = dto.Nombre,
                Siglas = dto.Siglas.Trim().ToUpper(),
                LogoUrl = dto.LogoUrl,
                Descripcion = dto.Descripcion?.Trim(),
                Estado = dto.Estado
            };

            await _partidoRepository.AddAsync(partido);

            dto.Id = partido.Id;
            return dto;
        }

        // EDITAR UN PARTIDO EXISTENTE
        public async Task UpdateAsync(PartidoPoliticoSaveDto dto)
        {
            var partido = await _partidoRepository.GetByIdAsync(dto.Id);

            if (partido != null)
            {
                // Actualiza las propiedades de la entidad con lo que ingreso el usuario
                partido.Nombre = dto.Nombre;
                partido.Siglas = dto.Siglas.Trim().ToUpper();
                partido.LogoUrl = dto.LogoUrl;
                partido.Descripcion = dto.Descripcion?.Trim();
                partido.Estado = dto.Estado;

                await _partidoRepository.UpdateAsync(partido);
            }
        }
        public async Task<bool> ExisteSiglasAsync(string siglas, int idActual = 0)
        {
            if (string.IsNullOrWhiteSpace(siglas)) return false;
            
            // Limpia las siglas tal cual como se van a guardar
            string siglasLimpia = siglas.Trim().ToUpper();

            var partidos = await _partidoRepository.GetAllAsync();
            
            // Valida si ya existe algun partido con esas siglas
            return partidos.Any(p => p.Siglas.Trim().ToUpper() == siglasLimpia && p.Id != idActual);
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
