using Evote360.Application.Services.Interfaces;
using Evote360.Application.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Evote360.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly IEleccionService _eleccionService;

        public AdminController(IEleccionService eleccionService)
        {
            _eleccionService = eleccionService;
        }

        public async Task<IActionResult> Index(int? anioElectoral)
        {
            var viewModel = new AdminHomeViewModel();
            viewModel.AniosDisponibles = await _eleccionService.ObtenerAniosConEleccionesAsync();

            if (!viewModel.AniosDisponibles.Any())
            {
                ViewBag.InfoMessage = "No existen elecciones registradas en el sistema.";
                return View(viewModel);
            }

            // Regla de negocio: Por defecto se selecciona automáticamente el año más reciente
            if (!anioElectoral.HasValue)
            {
                anioElectoral = viewModel.AniosDisponibles.First();
            }

            // Validar que el año seleccionado exista dentro de los años con elecciones registradas
            if (!viewModel.AniosDisponibles.Contains(anioElectoral.Value))
            {
                ModelState.AddModelError(string.Empty, "El año seleccionado debe existir dentro de los años con elecciones registradas.");
                return View(viewModel);
            }

            viewModel.AnioSeleccionado = anioElectoral;
            viewModel.EleccionesResumen = await _eleccionService.ObtenerResumenElectoralPorAnioAsync(anioElectoral.Value);

            if (!viewModel.EleccionesResumen.Any())
            {
                ViewBag.InfoMessage = "No existen elecciones registradas para el año seleccionado.";
            }

            return View(viewModel);
        }
    }
}