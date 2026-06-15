using Evote360.Application.DTOs;
using Evote360.Application.Interfaces;
using Evote360.Application.Services.Interfaces;
using Evote360.Application.ViewModels.Candidatos;
using Evote360.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote360.Application.Services.Implementations
{
    public class CandidatoService : ICandidatoService
    {
        private readonly ICandidatoRepository _repository;
        private readonly IEleccionRepository _eleccionRepository;
        private readonly IAsignacionDirigenteRepository _asignacionDirigenteRepository;
        private readonly IPartidoPoliticoService _partidoPoliticoService;
        private readonly IFileStorageService _fileStorageService;

        public CandidatoService(
            ICandidatoRepository repository, 
            IEleccionRepository eleccionRepository,
            IAsignacionDirigenteRepository asignacionDirigenteRepository,
            IPartidoPoliticoService partidoPoliticoService,
            IFileStorageService fileStorageService) 
        {
            _repository = repository;
            _eleccionRepository = eleccionRepository;
            _asignacionDirigenteRepository = asignacionDirigenteRepository;
            _partidoPoliticoService = partidoPoliticoService;
            _fileStorageService = fileStorageService;
        }

        private async Task<int?> GetCurrentPartidoIdAsync(int userId)
        {
            var asignaciones = await _asignacionDirigenteRepository.GetAllAsync();
            var asignacion = asignaciones.FirstOrDefault(a => a.UsuarioId == userId);
            return asignacion?.PartidoId;
        }

        private async Task<(bool Success, string ErrorMessage, int PartidoId)> ValidatePartyAccessAsync(int userId, string action = "")
        {
            var partidoId = await GetCurrentPartidoIdAsync(userId);
            if (partidoId == null)
            {
                string msg = action == "Crear" 
                    ? "No puede crear candidatos porque no tiene un partido político asignado."
                    : "No tiene un partido político asignado. Por favor, póngase en contacto con un administrador.";
                return (false, msg, 0);
            }

            var partido = await _partidoPoliticoService.GetByIdSaveDtoAsync(partidoId.Value);
            if (partido == null || partido.Estado == false)
            {
                string msg = action == "Crear"
                    ? "No puede crear candidatos porque el partido político asignado se encuentra inactivo."
                    : "El partido político asignado a este usuario se encuentra inactivo.";
                return (false, msg, 0);
            }
            return (true, string.Empty, partidoId.Value);
        }

        public async Task<(bool Success, string ErrorMessage, IEnumerable<CandidatoViewModel>? Candidatos, bool HasActiveElection)> GetIndexDataAsync(int userId)
        {
            var partyAccess = await ValidatePartyAccessAsync(userId);
            if (!partyAccess.Success) return (false, partyAccess.ErrorMessage, null, false);

            var candidatos = await _repository.GetAllAsync();
            bool hasActiveElection = await _eleccionRepository.ExisteEleccionActivaAsync();
            var elecciones = await _eleccionRepository.GetAllAsync();
            var currentElection = elecciones.FirstOrDefault(e => e.EstadoElectoral == "Activa" || e.EstadoElectoral == "Pendiente");

            var dtos = candidatos.Where(c => c.PartidoId == partyAccess.PartidoId).Select(c => new CandidatoViewModel
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                FotoUrl = c.FotoUrl,
                PuestoAsociado = c.AsignacionesPuestos?.FirstOrDefault(a => currentElection == null || a.EleccionId == currentElection.Id)?.Puesto?.Nombre ?? "Sin puesto asignado",
                Estado = c.Estado
            }).ToList();

            return (true, string.Empty, dtos, hasActiveElection);
        }

        public async Task<(bool Success, string ErrorMessage)> ValidateCreateAccessAsync(int userId)
        {
            var partyAccess = await ValidatePartyAccessAsync(userId, "Crear");
            if (!partyAccess.Success) return (false, partyAccess.ErrorMessage);

            if (await _eleccionRepository.ExisteEleccionActivaAsync())
            {
                return (false, "No se pueden modificar candidatos mientras exista una elección activa.");
            }
            return (true, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage)> CreateCandidatoAsync(int userId, CrearCandidatoViewModel model)
        {
            var partyAccess = await ValidatePartyAccessAsync(userId, "Crear");
            if (!partyAccess.Success) return (false, partyAccess.ErrorMessage);

            if (await _eleccionRepository.ExisteEleccionActivaAsync())
            {
                return (false, "No se puede crear un candidato mientras exista una elección activa.");
            }

            string fotoUrl = string.Empty;
            if (model.Foto != null)
            {
                fotoUrl = await _fileStorageService.SaveFileAsync(model.Foto, "images/candidatos");
            }

            var candidato = new Core.Entities.Candidato
            {
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                PartidoId = partyAccess.PartidoId,
                Estado = model.Estado,
                FotoUrl = fotoUrl
            };

            await _repository.AddAsync(candidato);
            return (true, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage, EditarCandidatoViewModel? Model)> GetEditarDataAsync(int userId, int id)
        {
            var partyAccess = await ValidatePartyAccessAsync(userId);
            if (!partyAccess.Success) return (false, partyAccess.ErrorMessage, null);

            if (await _eleccionRepository.ExisteEleccionActivaAsync())
            {
                return (false, "No se pueden modificar candidatos mientras exista una elección activa.", null);
            }

            var c = await _repository.GetByIdAsync(id);
            if (c == null || c.PartidoId != partyAccess.PartidoId)
            {
                return (false, "No tiene permisos para modificar este candidato.", null);
            }

            var haParticipado = await _repository.HasParticipatedInElectionAsync(id);

            var model = new EditarCandidatoViewModel
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Estado = c.Estado,
                FotoUrlActual = c.FotoUrl,
                HaParticipado = haParticipado
            };

            return (true, string.Empty, model);
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateCandidatoAsync(int userId, EditarCandidatoViewModel model)
        {
            var partyAccess = await ValidatePartyAccessAsync(userId);
            if (!partyAccess.Success) return (false, partyAccess.ErrorMessage);

            if (await _eleccionRepository.ExisteEleccionActivaAsync())
            {
                return (false, "No se puede editar un candidato mientras exista una elección activa.");
            }

            var currentCandidato = await _repository.GetByIdAsync(model.Id);
            if (currentCandidato == null || currentCandidato.PartidoId != partyAccess.PartidoId)
            {
                return (false, "No tiene permisos para modificar este candidato.");
            }

            var haParticipado = await _repository.HasParticipatedInElectionAsync(model.Id);

            if (!haParticipado)
            {
                currentCandidato.Nombre = model.Nombre;
                currentCandidato.Apellido = model.Apellido;

                if (model.Foto != null)
                {
                    if (!string.IsNullOrEmpty(currentCandidato.FotoUrl))
                    {
                        _fileStorageService.DeleteFile(currentCandidato.FotoUrl);
                    }
                    currentCandidato.FotoUrl = await _fileStorageService.SaveFileAsync(model.Foto, "images/candidatos");
                }
            }

            currentCandidato.Estado = model.Estado;
            await _repository.UpdateAsync(currentCandidato);

            return (true, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage, string NombreCandidato)> GetConfirmacionDataAsync(int userId, int id, bool isActivar)
        {
            var partyAccess = await ValidatePartyAccessAsync(userId);
            if (!partyAccess.Success) return (false, partyAccess.ErrorMessage, string.Empty);

            var c = await _repository.GetByIdAsync(id);
            if (c == null || c.PartidoId != partyAccess.PartidoId)
            {
                return (false, isActivar ? "No tiene permisos para activar este candidato." : "No tiene permisos para desactivar este candidato.", string.Empty);
            }

            if (await _eleccionRepository.ExisteEleccionActivaAsync())
            {
                return (false, isActivar 
                    ? "No se puede activar un candidato mientras exista una elección activa."
                    : "No se puede desactivar un candidato mientras exista una elección activa.", string.Empty);
            }

            if (isActivar)
            {
                if (c.Estado == true)
                {
                    return (false, "Este candidato ya se encuentra activo.", string.Empty);
                }
            }
            else
            {
                if (await _repository.HasAssignedPuestoVigenteAsync(id))
                {
                    return (false, "No se puede desactivar este candidato porque está asignado a un puesto electivo.", string.Empty);
                }
                if (c.Estado == false)
                {
                    return (false, "Este candidato ya se encuentra inactivo.", string.Empty);
                }
            }

            return (true, string.Empty, $"{c.Nombre} {c.Apellido}");
        }

        public async Task<(bool Success, string ErrorMessage)> ActivarCandidatoAsync(int userId, int id)
        {
            var data = await GetConfirmacionDataAsync(userId, id, true);
            if (!data.Success) return (false, data.ErrorMessage);

            var candidato = await _repository.GetByIdAsync(id);
            if (candidato != null && candidato.Estado == false)
            {
                candidato.Estado = true;
                await _repository.UpdateAsync(candidato);
            }
            return (true, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage)> DesactivarCandidatoAsync(int userId, int id)
        {
            var data = await GetConfirmacionDataAsync(userId, id, false);
            if (!data.Success) return (false, data.ErrorMessage);

            var candidato = await _repository.GetByIdAsync(id);
            if (candidato != null && candidato.Estado == true)
            {
                candidato.Estado = false;
                await _repository.UpdateAsync(candidato);
            }
            return (true, string.Empty);
        }
    }
}