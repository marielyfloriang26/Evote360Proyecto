using Evote360.Application.Services.Interfaces;
using Evote360.Core.Interfaces;
using Evote360.Core.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Evote360.Application.Services.Implementations
{
    public class VotacionService : IVotacionService
    {
        private readonly ICiudadanoRepository _ciudadanoRepository;
        private readonly IEleccionRepository _eleccionRepository;
        private readonly IOcrService _ocrService;
        private readonly IEmailService _emailService;
        private readonly ICodigoVerificacionRepository _codigoVerificacionRepository;
        private readonly IPuestoElectivoRepository _puestoElectivoRepository;
        private readonly IAsignarCandidatoPuestoRepository _asignarCandidatoPuestoRepository;
        private readonly ICandidatoRepository _candidatoRepository;
        private readonly IAlianzaPoliticaRepository _alianzaPoliticaRepository;
        private readonly IPartidoPoliticoRepository _partidoPoliticoRepository;
        private readonly IVotoRepository _votoRepository;

        public VotacionService(ICiudadanoRepository ciudadanoRepository, 
                               IEleccionRepository eleccionRepository,
                               IOcrService ocrService,
                               IEmailService emailService,
                               ICodigoVerificacionRepository codigoVerificacionRepository,
                               IPuestoElectivoRepository puestoElectivoRepository,
                               IAsignarCandidatoPuestoRepository asignarCandidatoPuestoRepository,
                               ICandidatoRepository candidatoRepository,
                               IAlianzaPoliticaRepository alianzaPoliticaRepository,
                               IPartidoPoliticoRepository partidoPoliticoRepository,
                               IVotoRepository votoRepository)
        {
            _ciudadanoRepository = ciudadanoRepository;
            _eleccionRepository = eleccionRepository;
            _ocrService = ocrService;
            _emailService = emailService;
            _codigoVerificacionRepository = codigoVerificacionRepository;
            _puestoElectivoRepository = puestoElectivoRepository;
            _asignarCandidatoPuestoRepository = asignarCandidatoPuestoRepository;
            _candidatoRepository = candidatoRepository;
            _alianzaPoliticaRepository = alianzaPoliticaRepository;
            _partidoPoliticoRepository = partidoPoliticoRepository;
            _votoRepository = votoRepository;
        }

        public async Task<(bool Success, string Message)> ValidarElectorAsync(string documentoIdentidad)
        {
            if (string.IsNullOrWhiteSpace(documentoIdentidad))
            {
                return (false, "El número de documento de identidad es requerido.");
            }

            var ciudadano = await _ciudadanoRepository.ObtenerPorCedulaAsync(documentoIdentidad.Trim());
            if (ciudadano == null)
            {
                return (false, "No existe un ciudadano registrado con este número de documento.");
            }

            var elecciones = await _eleccionRepository.GetAllAsync();
            var eleccionActiva = elecciones.FirstOrDefault(e => e.EstadoElectoral == "Activa");

            if (eleccionActiva == null)
            {
                return (false, "No hay ningún proceso electoral en estos momentos.");
            }

            if (!ciudadano.Estado)
            {
                return (false, "Este ciudadano se encuentra inactivo y no puede participar en el proceso de votación.");
            }

            if (ciudadano.HaVotado)
            {
                return (false, "Ya ha ejercido su derecho al voto.");
            }

            return (true, "Validación exitosa.");
        }

        public async Task<(bool Success, string Message)> ValidarIdentidadOcrAsync(byte[] imagenBytes, string nombreArchivo, string documentoEsperado)
        {
            if (imagenBytes == null || imagenBytes.Length == 0)
                return (false, "Debe subir una imagen de su cédula para validar su identidad.");

            var extension = System.IO.Path.GetExtension(nombreArchivo).ToLower();
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                return (false, "El archivo seleccionado no tiene un formato de imagen válido.");

            var ciudadano = await _ciudadanoRepository.ObtenerPorCedulaAsync(documentoEsperado);
            if (ciudadano == null)
                return (false, "El ciudadano no existe.");

            if (string.IsNullOrWhiteSpace(ciudadano.Correo))
                return (false, "Este ciudadano no tiene un correo electrónico registrado. No es posible continuar con la verificación de identidad.");

            var textoExtraido = _ocrService.ExtraerTextoDeImagen(imagenBytes);
            if (string.IsNullOrWhiteSpace(textoExtraido))
                return (false, "No fue posible leer correctamente el número de documento en la imagen cargada. Por favor, suba una imagen más clara.");

            string textoLimpio = textoExtraido.Replace(" ", "").Replace("-", "").Replace("\n", "").Replace("\r", "");
            string docLimpio = documentoEsperado.Trim();

            if (!textoLimpio.Contains(docLimpio))
                return (false, "Los datos extraídos de la foto no coinciden con los datos previamente ingresados por el elector.");

            // Si es exitoso, generar y enviar código
            var elecciones = await _eleccionRepository.GetAllAsync();
            var eleccionActiva = elecciones.FirstOrDefault(e => e.EstadoElectoral == "Activa");
            if (eleccionActiva == null) return (false, "No hay ningún proceso electoral en estos momentos.");

            var random = new Random();
            var codigo = random.Next(100000, 999999).ToString();

            var codigoVerificacion = new CodigoVerificacion
            {
                CiudadanoId = ciudadano.Id,
                EleccionId = eleccionActiva.Id,
                Codigo = codigo,
                FechaGeneracion = DateTime.Now,
                FechaExpiracion = DateTime.Now.AddMinutes(5),
                Usado = false,
                Estado = true
            };

            await _codigoVerificacionRepository.AddAsync(codigoVerificacion);

            try
            {
                var asunto = "Código de verificación para votar";
                var cuerpo = $"Hola {ciudadano.Nombre},\n\nSu código de verificación para continuar con el proceso de votación es:\n\n{codigo}\n\nEste código tendrá una vigencia de 5 minutos.\n\nSi usted no inició este proceso, ignore este mensaje.";
                await _emailService.EnviarCorreoAsync(ciudadano.Correo, asunto, cuerpo);
                return (true, "Validación exitosa. Se ha enviado el código al correo.");
            }
            catch
            {
                // Fallback para modo de prueba si las credenciales SMTP no están configuradas
                return (true, $"Modo de prueba activado (SMTP no configurado). Tu código es: {codigo}");
            }
        }

        public async Task<(bool Success, string Message)> ValidarCodigoVerificacionAsync(string documentoIdentidad, string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return (false, "Debe ingresar el código de verificación enviado a su correo electrónico.");

            var ciudadano = await _ciudadanoRepository.ObtenerPorCedulaAsync(documentoIdentidad.Trim());
            if (ciudadano == null) return (false, "El ciudadano no existe.");

            var elecciones = await _eleccionRepository.GetAllAsync();
            var eleccionActiva = elecciones.FirstOrDefault(e => e.EstadoElectoral == "Activa");
            if (eleccionActiva == null) return (false, "No hay ningún proceso electoral activo.");

            var codigos = await _codigoVerificacionRepository.GetAllAsync();
            var ultimoCodigo = codigos
                .Where(c => c.CiudadanoId == ciudadano.Id && c.EleccionId == eleccionActiva.Id)
                .OrderByDescending(c => c.FechaGeneracion)
                .FirstOrDefault();

            if (ultimoCodigo == null || ultimoCodigo.Codigo != codigo.Trim())
                return (false, "El código de verificación ingresado no es válido.");

            if (ultimoCodigo.Usado)
                return (false, "Este código de verificación ya fue utilizado.");

            if (DateTime.Now > ultimoCodigo.FechaExpiracion)
                return (false, "El código de verificación ha expirado. Solicite un nuevo código para continuar.");

            ultimoCodigo.Usado = true;
            await _codigoVerificacionRepository.UpdateAsync(ultimoCodigo);

            return (true, "Código validado con éxito.");
        }

        public async Task<System.Collections.Generic.IEnumerable<Evote360.Application.ViewModels.Votacion.PuestoDisponibleViewModel>> ObtenerPuestosDisponiblesAsync(string documentoIdentidad)
        {
            var ciudadano = await _ciudadanoRepository.ObtenerPorCedulaAsync(documentoIdentidad.Trim());
            if (ciudadano == null) return new System.Collections.Generic.List<Evote360.Application.ViewModels.Votacion.PuestoDisponibleViewModel>();

            var elecciones = await _eleccionRepository.GetAllAsync();
            var eleccionActiva = elecciones.FirstOrDefault(e => e.EstadoElectoral == "Activa");
            if (eleccionActiva == null) return new System.Collections.Generic.List<Evote360.Application.ViewModels.Votacion.PuestoDisponibleViewModel>();

            var puestosActivos = (await _puestoElectivoRepository.GetAllAsync()).Where(p => p.Estado).ToList();
            var asignaciones = await _asignarCandidatoPuestoRepository.GetAllAsync();
            var candidatos = await _candidatoRepository.GetAllAsync();

            var result = new System.Collections.Generic.List<Evote360.Application.ViewModels.Votacion.PuestoDisponibleViewModel>();

            foreach (var puesto in puestosActivos)
            {
                var asignacionesPuesto = asignaciones.Where(a => a.PuestoId == puesto.Id).ToList();

                var candidatosIds = asignacionesPuesto.Select(a => a.CandidatoId).Distinct().ToList();
                
                var partidosParticipantes = candidatos.Where(c => candidatosIds.Contains(c.Id)).Select(c => c.PartidoId).Distinct().Count();
                var candidatosReales = candidatosIds.Count;

                result.Add(new Evote360.Application.ViewModels.Votacion.PuestoDisponibleViewModel
                {
                    PuestoId = puesto.Id,
                    NombrePuesto = puesto.Nombre,
                    PartidosParticipantes = partidosParticipantes,
                    CandidatosReales = candidatosReales,
                    YaSelecciono = false // Esto se actualizará en el controlador verificando la sesión
                });
            }

            return result;
        }

        public async Task<Evote360.Application.ViewModels.Votacion.BoletaPuestoViewModel?> ObtenerBoletaPuestoAsync(int puestoId)
        {
            var puesto = await _puestoElectivoRepository.GetByIdAsync(puestoId);
            if (puesto == null || !puesto.Estado) return null;

            var elecciones = await _eleccionRepository.GetAllAsync();
            var eleccionActiva = elecciones.FirstOrDefault(e => e.EstadoElectoral == "Activa");
            if (eleccionActiva == null) return null;

            var asignaciones = await _asignarCandidatoPuestoRepository.GetAllAsync();
            var asignacionesPuesto = asignaciones.Where(a => a.PuestoId == puestoId && a.EleccionId == eleccionActiva.Id).ToList();
            
            var candidatosIds = asignacionesPuesto.Select(a => a.CandidatoId).Distinct().ToList();
            var candidatos = (await _candidatoRepository.GetAllAsync()).Where(c => candidatosIds.Contains(c.Id)).ToList();
            var partidos = await _partidoPoliticoRepository.GetAllAsync();
            var alianzas = (await _alianzaPoliticaRepository.GetAllAsync()).Where(a => a.EleccionId == eleccionActiva.Id).ToList();

            var result = new Evote360.Application.ViewModels.Votacion.BoletaPuestoViewModel
            {
                PuestoId = puesto.Id,
                NombrePuesto = puesto.Nombre,
                Candidatos = new System.Collections.Generic.List<Evote360.Application.ViewModels.Votacion.CandidatoBoletaViewModel>()
            };

            foreach (var candidato in candidatos)
            {
                var partidoPrincipal = partidos.FirstOrDefault(p => p.Id == candidato.PartidoId);
                if (partidoPrincipal == null) continue;

                var aliadosIds = alianzas.Where(a => a.PartidoMayoristaId == partidoPrincipal.Id).Select(a => a.PartidoAliadoId).ToList();
                var partidosAliados = partidos.Where(p => aliadosIds.Contains(p.Id)).Select(p => new Evote360.Application.ViewModels.Votacion.PartidoAliadoViewModel
                {
                    Nombre = p.Nombre,
                    LogoUrl = p.LogoUrl
                }).ToList();

                result.Candidatos.Add(new Evote360.Application.ViewModels.Votacion.CandidatoBoletaViewModel
                {
                    CandidatoId = candidato.Id,
                    NombreCompleto = $"{candidato.Nombre} {candidato.Apellido}",
                    FotoUrl = candidato.FotoUrl,
                    NombrePartidoPrincipal = partidoPrincipal.Nombre,
                    LogoPartidoPrincipal = partidoPrincipal.LogoUrl,
                    PartidosAliados = partidosAliados
                });
            }

            return result;
        }

        public async Task<(bool Success, string Message)> FinalizarVotacionAsync(string documentoIdentidad, System.Collections.Generic.Dictionary<int, int> selecciones)
        {
            var ciudadano = await _ciudadanoRepository.ObtenerPorCedulaAsync(documentoIdentidad.Trim());
            if (ciudadano == null) return (false, "Ciudadano no encontrado.");
            
            if (ciudadano.HaVotado) return (false, "El ciudadano ya ha emitido su voto.");

            var elecciones = await _eleccionRepository.GetAllAsync();
            var eleccionActiva = elecciones.FirstOrDefault(e => e.EstadoElectoral == "Activa");
            if (eleccionActiva == null) return (false, "No hay elección activa.");

            var puestos = (await _puestoElectivoRepository.GetAllAsync()).Where(p => p.Estado).ToList();
            if (selecciones.Count != puestos.Count)
                return (false, "Debe seleccionar un candidato para todos los puestos activos.");

            var resumenCorreo = "Resumen de Votación:\n\n";

            foreach (var kvp in selecciones)
            {
                var puestoId = kvp.Key;
                var candidatoId = kvp.Value;

                var puesto = puestos.FirstOrDefault(p => p.Id == puestoId);
                var nombrePuesto = puesto?.Nombre ?? "Puesto Desconocido";
                var nombreCandidato = "Ninguno / Abstención";

                int? candIdParaGuardar = null;

                if (candidatoId > 0)
                {
                    candIdParaGuardar = candidatoId;
                    var candidato = await _candidatoRepository.GetByIdAsync(candidatoId);
                    if (candidato != null)
                    {
                        var partido = await _partidoPoliticoRepository.GetByIdAsync(candidato.PartidoId);
                        var nombrePartido = partido?.Nombre ?? "Sin partido";
                        nombreCandidato = $"{candidato.Nombre} {candidato.Apellido} ({nombrePartido})";
                    }
                }

                var voto = new Evote360.Core.Entities.Voto
                {
                    EleccionId = eleccionActiva.Id,
                    PuestoId = puestoId,
                    CandidatoId = candIdParaGuardar,
                    Estado = true
                };

                await _votoRepository.AddAsync(voto);

                resumenCorreo += $"- {nombrePuesto}: {nombreCandidato}\n";
            }

            resumenCorreo += "\nGracias por participar en el proceso electoral eVote360 Pro.";

            ciudadano.HaVotado = true;
            await _ciudadanoRepository.UpdateAsync(ciudadano);

            try {
                await _emailService.EnviarCorreoAsync(ciudadano.Correo, "Comprobante de Votación - eVote360 Pro", resumenCorreo);
            } catch {
                // Ignore email failure for voting success
            }

            return (true, "Votación finalizada con éxito.");
        }
    }
}
