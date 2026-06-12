using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Evote360.Application.Services.Interfaces;
using Evote360.Application.ViewModels.Candidatos;
using Evote360.Application.DTOs;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Evote360.Core.Interfaces;
using Evote360.Application.Interfaces;

namespace Evote360.Web.Controllers
{
    // [Authorize(Roles = "Dirigente político")] // Uncomment when Auth is implemented
    public class CandidatosController : Controller
    {
        private readonly ICandidatoService _candidatoService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IAsignacionDirigenteRepository _asignacionDirigenteRepository;
        private readonly IPartidoPoliticoService _partidoPoliticoService;

        public CandidatosController(
            ICandidatoService candidatoService, 
            IFileStorageService fileStorageService,
            IAsignacionDirigenteRepository asignacionDirigenteRepository,
            IPartidoPoliticoService partidoPoliticoService)
        {
            _candidatoService = candidatoService;
            _fileStorageService = fileStorageService;
            _asignacionDirigenteRepository = asignacionDirigenteRepository;
            _partidoPoliticoService = partidoPoliticoService;
        }

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
            
            // MOCK PARA PRUEBAS SIN AUTH:
            // Obtenemos el primer partido de la base de datos en lugar de harcodear "1"
            var partidos = await _partidoPoliticoService.GetAllViewModelAsync();
            var primerPartido = partidos.FirstOrDefault();
            
            return primerPartido?.Id;
        }

        private async Task<bool> ValidatePartyAccessAsync(string action = "")
        {
            var partidoId = await GetCurrentPartidoIdAsync();
            if (partidoId == null)
            {
                TempData["ErrorMessage"] = action == "Crear" 
                    ? "No puede crear candidatos porque no tiene un partido político asignado."
                    : "No tiene un partido político asignado. Por favor, póngase en contacto con un administrador.";
                return false;
            }

            var partido = await _partidoPoliticoService.GetByIdSaveViewModelAsync(partidoId.Value);
            if (partido == null || !partido.Estado)
            {
                TempData["ErrorMessage"] = action == "Crear"
                    ? "No puede crear candidatos porque el partido político asignado se encuentra inactivo."
                    : "El partido político asignado a este usuario se encuentra inactivo.";
                return false;
            }
            return true;
        }

        public async Task<IActionResult> Index()
        {
            if (!await ValidatePartyAccessAsync())
                return RedirectToAction("Index", "Home");

            var partidoId = await GetCurrentPartidoIdAsync();

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

        public async Task<IActionResult> Crear()
        {
            if (!await ValidatePartyAccessAsync("Crear"))
                return RedirectToAction("Index", "Home");

            if (await _candidatoService.HasActiveElectionAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar candidatos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            return View(new CrearCandidatoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CrearCandidatoViewModel model)
        {
            if (!await ValidatePartyAccessAsync("Crear"))
                return RedirectToAction("Index", "Home");

            if (await _candidatoService.HasActiveElectionAsync())
            {
                TempData["ErrorMessage"] = "No se puede crear un candidato mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            if (model.Foto == null || model.Foto.Length == 0)
            {
                ModelState.AddModelError("Foto", "La foto del candidato es requerida.");
            }
            else
            {
                var extension = Path.GetExtension(model.Foto.FileName).ToLower();
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
                if (!extensionesPermitidas.Contains(extension) || model.Foto.Length < 100)
                {
                    ModelState.AddModelError("Foto", "La foto del candidato debe ser una imagen válida.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var partidoId = await GetCurrentPartidoIdAsync();

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

        public async Task<IActionResult> Editar(int id)
        {
            if (!await ValidatePartyAccessAsync())
                return RedirectToAction("Index", "Home");

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
        public async Task<IActionResult> Editar(int id, EditarCandidatoViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!await ValidatePartyAccessAsync())
                return RedirectToAction("Index", "Home");

            if (await _candidatoService.HasActiveElectionAsync())
            {
                TempData["ErrorMessage"] = "No se puede editar un candidato mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var partidoId = await GetCurrentPartidoIdAsync();
            var allMyCandidates = await _candidatoService.GetAllByPartidoAsync(partidoId ?? 0);
            if (!allMyCandidates.Any(c => c.Id == id))
            {
                TempData["ErrorMessage"] = "No tiene permisos para modificar este candidato.";
                return RedirectToAction(nameof(Index));
            }

            var haParticipado = await _candidatoService.HasParticipatedInElectionAsync(id);
            if (haParticipado)
            {
                ModelState.Remove("Nombre");
                ModelState.Remove("Apellido");
                ModelState.Remove("Foto");
            }

            if (model.Foto != null && model.Foto.Length > 0)
            {
                var extension = Path.GetExtension(model.Foto.FileName).ToLower();
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
                if (!extensionesPermitidas.Contains(extension) || model.Foto.Length < 100)
                {
                    ModelState.AddModelError("Foto", "La foto del candidato debe ser una imagen válida.");
                }
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

        public async Task<IActionResult> ConfirmarActivar(int id)
        {
            if (!await ValidatePartyAccessAsync())
                return RedirectToAction("Index", "Home");

            var dto = await _candidatoService.GetCandidatoById(id);
            if (dto == null) return NotFound();

            var partidoId = await GetCurrentPartidoIdAsync();
            var allMyCandidates = await _candidatoService.GetAllByPartidoAsync(partidoId ?? 0);
            if (!allMyCandidates.Any(c => c.Id == id))
            {
                TempData["ErrorMessage"] = "No tiene permisos para activar este candidato.";
                return RedirectToAction(nameof(Index));
            }

            if (await _candidatoService.HasActiveElectionAsync())
            {
                TempData["ErrorMessage"] = "No se puede activar un candidato mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.NombreCandidato = $"{dto.Nombre} {dto.Apellido}";
            return View(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            if (!await ValidatePartyAccessAsync())
                return RedirectToAction("Index", "Home");

            var dto = await _candidatoService.GetCandidatoById(id);
            if (dto == null) return NotFound();

            var partidoId = await GetCurrentPartidoIdAsync();
            var allMyCandidates = await _candidatoService.GetAllByPartidoAsync(partidoId ?? 0);
            if (!allMyCandidates.Any(c => c.Id == id))
            {
                TempData["ErrorMessage"] = "No tiene permisos para activar este candidato.";
                return RedirectToAction(nameof(Index));
            }

            if (await _candidatoService.HasActiveElectionAsync())
            {
                TempData["ErrorMessage"] = "No se puede activar un candidato mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            if (!dto.Estado)
            {
                await _candidatoService.AlternarEstadoCandidato(id);
                TempData["SuccessMessage"] = "Candidato activado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ConfirmarDesactivar(int id)
        {
            if (!await ValidatePartyAccessAsync())
                return RedirectToAction("Index", "Home");

            var dto = await _candidatoService.GetCandidatoById(id);
            if (dto == null) return NotFound();

            var partidoId = await GetCurrentPartidoIdAsync();
            var allMyCandidates = await _candidatoService.GetAllByPartidoAsync(partidoId ?? 0);
            if (!allMyCandidates.Any(c => c.Id == id))
            {
                TempData["ErrorMessage"] = "No tiene permisos para desactivar este candidato.";
                return RedirectToAction(nameof(Index));
            }

            if (await _candidatoService.HasActiveElectionAsync())
            {
                TempData["ErrorMessage"] = "No se puede desactivar un candidato mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            if (await _candidatoService.HasAssignedPuestoVigenteAsync(id))
            {
                TempData["ErrorMessage"] = "No se puede desactivar este candidato porque está asignado a un puesto electivo.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.NombreCandidato = $"{dto.Nombre} {dto.Apellido}";
            return View(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            if (!await ValidatePartyAccessAsync())
                return RedirectToAction("Index", "Home");

            var dto = await _candidatoService.GetCandidatoById(id);
            if (dto == null) return NotFound();

            var partidoId = await GetCurrentPartidoIdAsync();
            var allMyCandidates = await _candidatoService.GetAllByPartidoAsync(partidoId ?? 0);
            if (!allMyCandidates.Any(c => c.Id == id))
            {
                TempData["ErrorMessage"] = "No tiene permisos para desactivar este candidato.";
                return RedirectToAction(nameof(Index));
            }

            if (await _candidatoService.HasActiveElectionAsync())
            {
                TempData["ErrorMessage"] = "No se puede desactivar un candidato mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            if (await _candidatoService.HasAssignedPuestoVigenteAsync(id))
            {
                TempData["ErrorMessage"] = "No se puede desactivar este candidato porque está asignado a un puesto electivo.";
                return RedirectToAction(nameof(Index));
            }

            if (dto.Estado)
            {
                await _candidatoService.AlternarEstadoCandidato(id);
                TempData["SuccessMessage"] = "Candidato desactivado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}