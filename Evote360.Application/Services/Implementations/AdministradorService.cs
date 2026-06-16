using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evote360.Core.Entities;
using Evote360.Core.Interfaces;
using Evote360.Application.Interfaces;
using Evote360.Application.ViewModels.Administrador;

namespace Evote360.Application.Services.Implementations
{
    public class AdministradorService : IAdministradorService
    {
        private readonly IRepositoryAsync<Eleccion> _eleccionRepository;
        private readonly IRepositoryAsync<AsignarCandidatoPuesto> _asignacionCandidatoRepository;
        private readonly IRepositoryAsync<Voto> _votoRepository;
        private readonly IEleccionRepository _eleccionRepositoryEspecializado;

        public AdministradorService(
            IRepositoryAsync<Eleccion> eleccionRepository,
            IRepositoryAsync<AsignarCandidatoPuesto> asignacionCandidatoRepository,
            IRepositoryAsync<Voto> votoRepository, 
            IEleccionRepository eleccionRepositoryEspecializado)
        {
            _eleccionRepository = eleccionRepository;
            _asignacionCandidatoRepository = asignacionCandidatoRepository;
            _votoRepository = votoRepository;
            _eleccionRepositoryEspecializado = eleccionRepositoryEspecializado;
        }

        public async Task<HomeAdministradorViewModel> ObtenerDashboardAdminAsync()
        {
            var elecciones = await _eleccionRepository.GetAllAsync();
            
            var model = new HomeAdministradorViewModel
            {
                // Extraer los años utilizando la propiedad real: FechaInicio
                AniosDisponibles = elecciones?
                    .Select(e => e.FechaInicio.Year)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToList() ?? new List<int>()
            };

            return model;
        }

        public async Task<List<ResumenEleccionViewModel>> ObtenerResumenPorAnioAsync(int anio)
        {
            var elecciones = await _eleccionRepository.GetAllAsync();
            if (elecciones == null) return new List<ResumenEleccionViewModel>();

            // Filtrar elecciones del año seleccionado usando la propiedad correcta: FechaInicio
            var eleccionesFiltradas = elecciones
                .Where(e => e.FechaInicio.Year == anio)
                .OrderByDescending(e => e.FechaInicio)
                .ToList();

            var listaResumen = new List<ResumenEleccionViewModel>();

            // Carga local en memoria para efectuar los cruces de datos de forma segura
            var todasLasAsignaciones = await _asignacionCandidatoRepository.GetAllAsync() ?? new List<AsignarCandidatoPuesto>();
            var todosLosVotos = await _votoRepository.GetAllAsync() ?? new List<Voto>();

            foreach (var eleccion in eleccionesFiltradas)
            {
                // REEMPLAZA TU BLOQUE ANTERIOR POR ESTAS 3 LÍNEAS ASÍNCRONAS:
                int partidosParticipantes = await _eleccionRepositoryEspecializado.ObtenerCantidadPartidosPorEleccionAsync(eleccion.Id);
                int candidatosReales = await _eleccionRepositoryEspecializado.ObtenerCantidadCandidatosRealesPorEleccionAsync(eleccion.Id);
                int ciudadanosQueVotaron = await _eleccionRepositoryEspecializado.ObtenerCantidadCiudadanosQueVotaronAsync(eleccion.Id);

                listaResumen.Add(new ResumenEleccionViewModel
                {
                    NombreEleccion = !string.IsNullOrEmpty(eleccion.Nombre) ? eleccion.Nombre : $"Elección No. {eleccion.Id}",
                    FechaRealizacion = eleccion.FechaInicio.ToString("dd/MM/yyyy"),
                    CantidadPartidos = partidosParticipantes,
                    CantidadCandidatosReales = candidatosReales,
                    CantidadCiudadanosVotaron = ciudadanosQueVotaron
                });
            }

            return listaResumen;
        }
    }
}