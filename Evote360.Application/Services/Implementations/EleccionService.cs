using Evote360.Application.DTOs;
using Evote360.Application.Services.Interfaces;
using Evote360.Core.Common;
using Evote360.Core.Entities;
using Evote360.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote360.Application.Services.Implementations
{
    public class EleccionService : IEleccionService
    {
        private readonly IEleccionRepository _eleccionRepository;

        public EleccionService(IEleccionRepository eleccionRepository)
        {
            _eleccionRepository = eleccionRepository;
        }

        public async Task<bool> ExisteEleccionActivaAsync()
        {
            return await _eleccionRepository.ExisteEleccionActivaAsync();
        }

        public async Task<EleccionDto?> ObtenerPorIdAsync(int id)
        {
            var e = await _eleccionRepository.GetByIdAsync(id);
            if (e == null) return null;
            return new EleccionDto 
            { 
                Id = e.Id, 
                Nombre = e.Nombre, 
                EstadoElectoral = e.EstadoElectoral,
                FechaInicio = e.FechaInicio,
                FechaFin = e.FechaFin
            };
        }

        public async Task<IEnumerable<EleccionDto>> ObtenerTodasAsync()
        {
            var elecciones = await _eleccionRepository.ObtenerTodasOrdenadasAsync();
            var result = new List<EleccionDto>();

            var puestos = await _eleccionRepository.ObtenerPuestosActivosAsync();
            var partidos = await _eleccionRepository.ObtenerPartidosActivosAsync();

            int cantPuestosActivos = puestos.Count;
            int cantPartidosActivos = partidos.Count;

            foreach (var e in elecciones)
            {
                int ciudadanosVotaron = await _eleccionRepository.ObtenerCantidadCiudadanosQueVotaronAsync(e.Id);
                result.Add(new EleccionDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,
                    EstadoElectoral = e.EstadoElectoral,
                    CantidadPartidos = cantPartidosActivos,
                    CantidadPuestos = cantPuestosActivos,
                    CantidadCiudadanosVotaron = ciudadanosVotaron
                });
            }
            return result;
        }

        public async Task<(bool Success, List<string> Messages)> CrearAsync(EleccionCreateDto dto)
        {
            var errores = await ValidarConfiguracionElectoralAsync();
            
            if (await _eleccionRepository.ExisteEleccionActivaAsync())
            {
                errores.Add("No se puede crear una nueva elección mientras exista otra en estado Activa.");
            }

            if (errores.Any()) return (false, errores);

            var nuevaEleccion = new Eleccion
            {
                Nombre = dto.Nombre.Trim(),
                FechaInicio = dto.FechaRealizacion, 
                FechaFin = dto.FechaRealizacion,    
                EstadoElectoral = EstadosEleccion.Pendiente,
                Estado = true
            };

            await _eleccionRepository.AddAsync(nuevaEleccion);
            return (true, new List<string> { "Elección guardada con éxito en estado Pendiente." });
        }

        public async Task<(bool Success, List<string> Messages)> ActivarAsync(int id)
        {
            var errores = new List<string>();
            var eleccion = await _eleccionRepository.GetByIdAsync(id);

            if (eleccion == null)
            {
                errores.Add("La elección no existe.");
                return (false, errores);
            }

            if (eleccion.EstadoElectoral != EstadosEleccion.Pendiente)
            {
                errores.Add("Solo se pueden activar elecciones que estén Pendientes.");
            }

            if (await _eleccionRepository.ExisteEleccionActivaAsync())
            {
                errores.Add("Ya existe un proceso electoral Activo actualmente.");
            }

            var erroresConfig = await ValidarConfiguracionElectoralAsync();
            errores.AddRange(erroresConfig);

            if (errores.Any()) return (false, errores);

            eleccion.EstadoElectoral = EstadosEleccion.Activa;
            await _eleccionRepository.UpdateAsync(eleccion);

            return (true, new List<string> { "La elección ha sido activada de forma satisfactoria." });
        }

        public async Task<(bool Success, string Message)> FinalizarAsync(int id)
        {
            var eleccion = await _eleccionRepository.GetByIdAsync(id);
            if (eleccion == null) return (false, "La elección no existe.");

            if (eleccion.EstadoElectoral != EstadosEleccion.Activa)
                return (false, "Solo se pueden finalizar elecciones que estén Activas.");

            eleccion.EstadoElectoral = EstadosEleccion.Finalizada;
            eleccion.FechaFin = DateTime.Now; 
            await _eleccionRepository.UpdateAsync(eleccion);

            return (true, "La elección ha cerrado de forma correcta. Urnas bloqueadas.");
        }

        public async Task<List<ResultadoPuestoDto>> ObtenerResultadosAsync(int id)
        {
            var resultados = new List<ResultadoPuestoDto>();
            var puestos = await _eleccionRepository.ObtenerPuestosActivosAsync();

            foreach (var puesto in puestos)
            {
                var puestoResult = new ResultadoPuestoDto
                {
                    PuestoId = puesto.Id,
                    PuestoNombre = puesto.Nombre
                };

                int totalVotosPuesto = await _eleccionRepository.ObtenerTotalVotosPorPuestoAsync(id, puesto.Id);
                var asignaciones = await _eleccionRepository.ObtenerAsignacionesPorPuestoAsync(puesto.Id);

                foreach (var asig in asignaciones)
                {
                    int votosCandidato = await _eleccionRepository.ObtenerCantidadVotosPorOpcionAsync(id, puesto.Id, asig.CandidatoId);
                    double porcentaje = totalVotosPuesto > 0 ? ((double)votosCandidato / totalVotosPuesto) * 100 : 0.0;

                    puestoResult.Opciones.Add(new ResultadoOpcionDto
                    {
                        CandidatoNombre = $"{asig.Candidato.Nombre} {asig.Candidato.Apellido}",
                        PartidoNombre = asig.Candidato.Partido.Nombre,
                        PartidoSiglas = asig.Candidato.Partido.Siglas,
                        CantidadVotos = votosCandidato,
                        PorcentajeVotos = Math.Round(porcentaje, 2),
                        EsGanador = false
                    });
                }

                int votosNinguno = await _eleccionRepository.ObtenerCantidadVotosPorOpcionAsync(id, puesto.Id, null);
                double porcentajeNinguno = totalVotosPuesto > 0 ? ((double)votosNinguno / totalVotosPuesto) * 100 : 0.0;

                puestoResult.Opciones.Add(new ResultadoOpcionDto
                {
                    CandidatoNombre = "Ninguno",
                    PartidoNombre = "No aplica",
                    PartidoSiglas = "N/A",
                    CantidadVotos = votosNinguno,
                    PorcentajeVotos = Math.Round(porcentajeNinguno, 2),
                    EsGanador = false
                });

                puestoResult.Opciones = puestoResult.Opciones.OrderByDescending(o => o.CantidadVotos).ToList();

                if (puestoResult.Opciones.Any())
                {
                    int maxVotos = puestoResult.Opciones.First().CantidadVotos;
                    var mejoresOpciones = puestoResult.Opciones.Where(o => o.CantidadVotos == maxVotos).ToList();

                    if (mejoresOpciones.Count > 1)
                    {
                        puestoResult.ExisteEmpatePrimerLugar = true;
                    }
                    else
                    {
                        if (mejoresOpciones.First().CandidatoNombre != "Ninguno")
                        {
                            puestoResult.Opciones.First().EsGanador = true;
                        }
                    }
                }

                resultados.Add(puestoResult);
            }

            return resultados;
        }

        private async Task<List<string>> ValidarConfiguracionElectoralAsync()
        {
            var mensajes = new List<string>();

            var puestosActivos = await _eleccionRepository.ObtenerPuestosActivosAsync();
            var partidosActivos = await _eleccionRepository.ObtenerPartidosActivosAsync();

            if (!puestosActivos.Any())
            {
                mensajes.Add("No se registran puestos electivos activos en la plataforma.");
                return mensajes;
            }

            if (partidosActivos.Count < 2)
            {
                mensajes.Add("Se requieren por lo menos 2 partidos políticos activos para competir.");
                return mensajes;
            }

            foreach (var partido in partidosActivos)
            {
                var puestosFaltantes = new List<string>();

                foreach (var puesto in puestosActivos)
                {
                    bool tieneAsignacion = await _eleccionRepository.ExisteAsignacionCandidatoAsync(puesto.Id, partido.Id);

                    if (!tieneAsignacion)
                    {
                        puestosFaltantes.Add(puesto.Nombre);
                    }
                }

                if (puestosFaltantes.Any())
                {
                    string listaPuestos = string.Join(", ", puestosFaltantes);
                    mensajes.Add($"El partido '{partido.Nombre}' ({partido.Siglas}) no posee un candidato asignado para: {listaPuestos}.");
                }
            }

            return mensajes;
        }
    }
}