using Evote360.Application.DTOs;
using Evote360.Application.Services.Interfaces;
using Evote360.Application.ViewModels.PuestoElectivo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Evote360.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PuestoElectivoController : Controller
    {
        private readonly IPuestoElectivoService _puestoService;

        public PuestoElectivoController(IPuestoElectivoService puestoService)
        {
            _puestoService = puestoService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new PuestoElectivoIndexViewModel
            {
                Puestos = await _puestoService.ObtenerTodosAsync(),
                ExisteEleccionActiva = await _puestoService.ExisteEleccionActivaAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> Crear()
        {
            if (await _puestoService.ExisteEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se puede crear un puesto electivo mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }
            return View(new PuestoElectivoCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(PuestoElectivoCreateViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var dto = new PuestoElectivoCreateDto
            {
                Nombre = viewModel.Nombre,
                Descripcion = viewModel.Descripcion,
                Estado = viewModel.Estado
            };

            var (success, message) = await _puestoService.CrearAsync(dto);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(viewModel);
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (await _puestoService.ExisteEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se puede editar un puesto electivo mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _puestoService.ObtenerPorIdParaEditarAsync(id);
            if (dto == null) return NotFound();

            var viewModel = new PuestoElectivoUpdateViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Estado = dto.Estado,
                YaFueUtilizadoEnEleccion = await _puestoService.FueUtilizadoEnEleccionAsync(id)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(PuestoElectivoUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var dto = new PuestoElectivoUpdateDto
            {
                Id = viewModel.Id,
                Nombre = viewModel.Nombre,
                Descripcion = viewModel.Descripcion,
                Estado = viewModel.Estado
            };

            var (success, message) = await _puestoService.EditarAsync(dto);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(viewModel);
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ConfirmarActivar(int id)
        {
            if (await _puestoService.ExisteEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se puede activar un puesto electivo mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var puesto = await _puestoService.ObtenerPorIdParaEditarAsync(id);
            if (puesto == null) return NotFound();

            ViewBag.NombrePuesto = puesto.Nombre;
            return View(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            var (success, message) = await _puestoService.ActivarAsync(id);
            if (!success)
            {
                TempData["ErrorMessage"] = message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ConfirmarDesactivar(int id)
        {
            if (await _puestoService.ExisteEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se puede desactivar un puesto electivo mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var puesto = await _puestoService.ObtenerPorIdParaEditarAsync(id);
            if (puesto == null) return NotFound();

            ViewBag.NombrePuesto = puesto.Nombre;
            return View(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            var (success, message) = await _puestoService.DesactivarAsync(id);
            if (!success)
            {
                TempData["ErrorMessage"] = message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }
    }
}