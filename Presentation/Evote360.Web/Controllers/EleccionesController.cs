using Evote360.Application.DTOs;
using Evote360.Application.DTOs.Eleccion;
using Evote360.Application.DTOs.Resultado;
using Evote360.Application.Services.Interfaces;
using Evote360.Application.ViewModels.Eleccion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote360.Web.Controllers
{
    [Authorize(Roles = "Administrador")] // requerimiento para el módulo Administrador
    public class EleccionesController : Controller
    {
        private readonly IEleccionService _eleccionService;

        public EleccionesController(IEleccionService eleccionService)
        {
            _eleccionService = eleccionService;
        }

        public async Task<IActionResult> Index()
        {
            var elecciones = await _eleccionService.ObtenerTodasAsync();
            ViewBag.ExisteEleccionActiva = await _eleccionService.ExisteEleccionActivaAsync();
            return View(elecciones);
        }

        public async Task<IActionResult> Crear()
        {
            if (await _eleccionService.ExisteEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se puede crear una nueva elección mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }
            return View(new EleccionCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(EleccionCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = new EleccionCreateDto
            {
                Nombre = model.Nombre,
                FechaRealizacion = model.FechaRealizacion
            };

            var (success, messages) = await _eleccionService.CrearAsync(dto);

            if (!success)
            {
                foreach (var msg in messages)
                {
                    ModelState.AddModelError(string.Empty, msg);
                }
                return View(model);
            }

            TempData["SuccessMessage"] = messages.First();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ConfirmarActivacion(int id)
        {
            var eleccion = await _eleccionService.ObtenerPorIdAsync(id);
            if (eleccion == null) return NotFound();
            return View(eleccion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            var (success, messages) = await _eleccionService.ActivarAsync(id);
            if (!success)
            {
                TempData["ErrorList"] = messages;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = messages.First();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ConfirmarFinalizacion(int id)
        {
            var eleccion = await _eleccionService.ObtenerPorIdAsync(id);
            if (eleccion == null) return NotFound();
            return View(eleccion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(int id)
        {
            var (success, message) = await _eleccionService.FinalizarAsync(id);
            if (!success)
            {
                TempData["ErrorMessage"] = message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Resultados(int id)
        {
            var eleccion = await _eleccionService.ObtenerPorIdAsync(id);
            if (eleccion == null) return NotFound();

            var resultados = await _eleccionService.ObtenerResultadosAsync(id);
            ViewBag.EleccionNombre = eleccion.Nombre;
            return View(resultados);
        }
    }
}