using Evote360.Application.DTOs;
using Evote360.Application.Services.Interfaces;
using Evote360.Application.ViewModels.AsignacionDirigentePolitico;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Evote360.Web.Controllers
{
    [Authorize(Roles = "Administrador")] // requerimiento para el módulo administrativo
    public class AsignacionDirigentePoliticoController : Controller
    {
        private readonly IAsignacionDirigentePoliticoService _asignacionService;

        public AsignacionDirigentePoliticoController(IAsignacionDirigentePoliticoService asignacionService)
        {
            _asignacionService = asignacionService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.HayEleccionActiva = await _asignacionService.HayEleccionActivaAsync();
            var dtos = await _asignacionService.ObtenerAsignacionesDirigentesAsync();
            var viewModels = dtos.Select(d => new AsignacionDirigentePoliticoViewModel
            {
                Id = d.Id,
                IdUsuario = d.IdUsuario,
                NombreDirigente = d.NombreDirigente,
                NombreUsuario = d.NombreUsuario,
                IdPartidoPolitico = d.IdPartidoPolitico,
                NombreDelPartido = d.NombreDelPartido,
                SiglasDelPartido = d.SiglasDelPartido,
                EstadoDelDirigente = d.EstadoDelDirigente,
                EstadoDelPartido = d.EstadoDelPartido
            }).ToList();

            return View(viewModels);
        }

        public async Task<IActionResult> Crear()
        {
            if (await _asignacionService.HayEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar asignaciones de dirigentes políticos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new CrearAsignacionDirigentePoliticoViewModel();
            await LlenarListasViewBag(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CrearAsignacionDirigentePoliticoViewModel vm)
        {
            if (await _asignacionService.HayEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar asignaciones de dirigentes políticos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await LlenarListasViewBag(vm);
                return View(vm);
            }

            try
            {
                var dto = new CrearAsignacionDirPolDTO
                {
                    UsuarioId = vm.UsuarioId,
                    PartidoId = vm.PartidoId
                };

                await _asignacionService.CrearAsignacionAsync(dto);
                TempData["SuccessMessage"] = "Asignación creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LlenarListasViewBag(vm);
                return View(vm);
            }
        }

        public async Task<IActionResult> ConfirmarDesvincular(int id)
        {
            if (await _asignacionService.HayEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar asignaciones de dirigentes políticos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _asignacionService.ObtenerAsignacionDirigentePorIdAsync(id);
            if (dto == null)
            {
                TempData["ErrorMessage"] = "La asignación seleccionada no existe o ya fue eliminada.";
                return RedirectToAction(nameof(Index));
            }

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (await _asignacionService.HayEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar asignaciones de dirigentes políticos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _asignacionService.EliminarAsignacionAsync(id);
                TempData["SuccessMessage"] = "Asignación eliminada exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LlenarListasViewBag(CrearAsignacionDirigentePoliticoViewModel vm)
        {
            var usuarios = await _asignacionService.ObtenerUsuariosDirigentesDisponiblesAsync();
            var partidos = await _asignacionService.ObtenerPartidosDisponiblesAsync();

            vm.Usuarios = usuarios.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = $"{u.Nombre} {u.Apellido} - {u.NombreUsuario}"
            });

            vm.Partidos = partidos.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Nombre} - {p.Siglas}"
            });
        }
    }
}
