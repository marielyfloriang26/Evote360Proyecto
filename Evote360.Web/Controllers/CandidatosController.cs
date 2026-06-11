using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Evote360.Application.Services.Interfaces;
using Evote360.Application.ViewModels.Candidatos;
using Evote360.Application.DTOs;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Evote360.Core.Interfaces;

namespace Evote360.Web.Controllers
{
    // [Authorize(Roles = "Dirigente político")] // Uncomment when Auth is implemented
    public class CandidatosController : Controller
    {
        private readonly ICandidatoService _candidatoService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IAsignacionDirigenteRepository _asignacionDirigenteRepository;

        public CandidatosController(
            ICandidatoService candidatoService, 
            IFileStorageService fileStorageService,
            IAsignacionDirigenteRepository asignacionDirigenteRepository)
        {
            _candidatoService = candidatoService;
            _fileStorageService = fileStorageService;
            _asignacionDirigenteRepository = asignacionDirigenteRepository;
        }

        // Helper para obtener el PartidoId del usuario logueado
        private async Task<int?> GetCurrentPartidoIdAsync()
        {
            // Simulación o extracción real desde los Claims. 
            // Si el ID del usuario se guarda en el NameIdentifier:
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim, out int userId))
            {
                var asignaciones = await _asignacionDirigenteRepository.GetAllAsync();
                var asignacion = asignaciones.FirstOrDefault(a => a.UsuarioId == userId);
                return asignacion?.PartidoId;
            }
            
            // Para propósitos de prueba si no hay auth, retornaremos un ID mock, o puedes lanzar excepción
            return 1; // MOCK PARTIDO ID
        }

        public async Task<IActionResult> Index()
        {
            var partidoId = await GetCurrentPartidoIdAsync();
            if (partidoId == null)
            {
                TempData["ErrorMessage"] = "No tiene un partido político asignado. Por favor, póngase en contacto con un administrador.";
                return RedirectToAction("Index", "Home"); // O redirigir a Login
            }

            var dtos = await _candidatoService.GetAllByPartidoAsync(partidoId.Value);
            var viewModels = dtos.Select(dto => new CandidatoViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                FotoUrl = dto.FotoUrl,
                PuestoAsociado = dto.PuestoAsociado,
                Estado = dto.Estado
            }).ToList();

            ViewBag.HasActiveElection = await _candidatoService.HasActiveElectionAsync();

            return View(viewModels);
        }

        public async Task<IActionResult> Create()
        {
            if (await _candidatoService.HasActiveElectionAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar candidatos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            return View(new CrearCandidatoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearCandidatoViewModel model)
        {
            if (await _candidatoService.HasActiveElectionAsync())
            {
                TempData["ErrorMessage"] = "No se puede crear un candidato mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var partidoId = await GetCurrentPartidoIdAsync();
            if (partidoId == null)
            {
                TempData["ErrorMessage"] = "No puede crear candidatos porque no tiene un partido político asignado.";
                return RedirectToAction("Index", "Home");
            }

            // Ideally check if party is inactive here but we assume it's checked globally.
            // If we had a party check, the message would be: "No puede crear candidatos porque el partido político asignado se encuentra inactivo."

            string fotoUrl = string.Empty;
            if (model.Foto != null)
            {
                fotoUrl = await _fileStorageService.SaveFileAsync(model.Foto, "images/candidatos");
            }

            var dto = new CrearCandidatoDTO
            {
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Estado = model.Estado,
                PartidoId = partidoId.Value,
                Foto = model.Foto 
            };

            var createdDto = await _candidatoService.CreateCandidato(dto);
            
            if (createdDto != null)
            {
                createdDto.FotoUrl = fotoUrl;
                await _candidatoService.UpdateCandidato(createdDto); 
            }

            TempData["SuccessMessage"] = "Candidato creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (await _candidatoService.HasActiveElectionAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar candidatos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _candidatoService.GetCandidatoById(id);
            if (dto == null) return NotFound();

            var partidoId = await GetCurrentPartidoIdAsync();
            var allMyCandidates = await _candidatoService.GetAllByPartidoAsync(partidoId ?? 0);
            if (!allMyCandidates.Any(c => c.Id == id))
            {
                TempData["ErrorMessage"] = "No tiene permisos para modificar este candidato.";
                return RedirectToAction(nameof(Index));
            }

            var haParticipado = await _candidatoService.HasParticipatedInElectionAsync(id);

            var model = new EditarCandidatoViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Estado = dto.Estado,
                FotoUrlActual = dto.FotoUrl,
                HaParticipado = haParticipado
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditarCandidatoViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (await _candidatoService.HasActiveElectionAsync())
            {
                TempData["ErrorMessage"] = "No se puede editar un candidato mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var haParticipado = await _candidatoService.HasParticipatedInElectionAsync(id);
            if (haParticipado)
            {
                ModelState.Remove("Nombre");
                ModelState.Remove("Apellido");
                ModelState.Remove("Foto");
            }

            if (!ModelState.IsValid)
            {
                model.HaParticipado = haParticipado;
                return View(model);
            }

            var currentDto = await _candidatoService.GetCandidatoById(id);
            if (currentDto == null) return NotFound();

            if (!haParticipado)
            {
                currentDto.Nombre = model.Nombre;
                currentDto.Apellido = model.Apellido;

                if (model.Foto != null)
                {
                    if (!string.IsNullOrEmpty(currentDto.FotoUrl))
                    {
                        _fileStorageService.DeleteFile(currentDto.FotoUrl);
                    }
                    currentDto.FotoUrl = await _fileStorageService.SaveFileAsync(model.Foto, "images/candidatos");
                }
            }
            
            currentDto.Estado = model.Estado;

            await _candidatoService.UpdateCandidato(currentDto);

            TempData["SuccessMessage"] = "Candidato actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var dto = await _candidatoService.GetCandidatoById(id);
            if (dto == null) return NotFound();

            if (await _candidatoService.HasActiveElectionAsync())
            {
                TempData["ErrorMessage"] = dto.Estado 
                    ? "No se puede desactivar un candidato mientras exista una elección activa." 
                    : "No se puede activar un candidato mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            if (dto.Estado) // Attempting to deactivate
            {
                if (await _candidatoService.HasAssignedPuestoVigenteAsync(id))
                {
                    TempData["ErrorMessage"] = "No se puede desactivar este candidato porque está asignado a un puesto electivo.";
                    return RedirectToAction(nameof(Index));
                }
            }

            await _candidatoService.AlternarEstadoCandidato(id);
            TempData["SuccessMessage"] = "Estado modificado exitosamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}