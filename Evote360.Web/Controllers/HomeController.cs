using Microsoft.AspNetCore.Mvc;
using Evote360.Application.ViewModels;
using Evote360.Application.ViewModels.Votacion;
using Evote360.Application.Services.Interfaces;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Evote360.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IVotacionService _votacionService;

        public HomeController(ILogger<HomeController> logger, IVotacionService votacionService)
        {
            _logger = logger;
            _votacionService = votacionService;
        }

        public IActionResult Index()
        {
            return View(new LoginElectorViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginElectorViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _votacionService.ValidarElectorAsync(vm.DocumentoIdentidad);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(vm);
            }

            // Save DocumentoIdentidad in session or TempData for the next step
            TempData["DocumentoElector"] = vm.DocumentoIdentidad.Trim();

            return RedirectToAction("ValidacionIdentidad");
        }

        public IActionResult ValidacionIdentidad()
        {
            if (TempData["DocumentoElector"] == null)
            {
                return RedirectToAction("Index");
            }
            
            TempData.Keep("DocumentoElector");
            return View(new ValidacionIdentidadViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ValidacionIdentidad(ValidacionIdentidadViewModel vm)
        {
            var documento = TempData["DocumentoElector"]?.ToString();
            if (string.IsNullOrEmpty(documento))
            {
                return RedirectToAction("Index");
            }
            TempData.Keep("DocumentoElector");

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            if (vm.ImagenCedula == null || vm.ImagenCedula.Length == 0)
            {
                ViewBag.ErrorMessage = "Debe subir una imagen de su cédula para validar su identidad.";
                return View(vm);
            }

            using var memoryStream = new System.IO.MemoryStream();
            await vm.ImagenCedula.CopyToAsync(memoryStream);
            var imagenBytes = memoryStream.ToArray();

            var result = await _votacionService.ValidarIdentidadOcrAsync(imagenBytes, vm.ImagenCedula.FileName, documento);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(vm);
            }

            // Exitoso: se envió el correo o se generó el fallback.
            TempData["MensajeCodigo"] = result.Message;
            return RedirectToAction("VerificacionCodigo");
        }

        public IActionResult VerificacionCodigo()
        {
            if (TempData["DocumentoElector"] == null)
            {
                return RedirectToAction("Index");
            }
            TempData.Keep("DocumentoElector");

            return View(new VerificacionCodigoViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> VerificacionCodigo(VerificacionCodigoViewModel vm)
        {
            var documento = TempData["DocumentoElector"]?.ToString();
            if (string.IsNullOrEmpty(documento))
            {
                return RedirectToAction("Index");
            }
            TempData.Keep("DocumentoElector");

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _votacionService.ValidarCodigoVerificacionAsync(documento, vm.CodigoVerificacion);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(vm);
            }

            TempData["CodigoValidado"] = true;
            return RedirectToAction("PuestosDisponibles");
        }

        public async Task<IActionResult> PuestosDisponibles()
        {
            var documento = TempData["DocumentoElector"]?.ToString();
            if (string.IsNullOrEmpty(documento) || TempData["CodigoValidado"] == null)
            {
                return RedirectToAction("Index");
            }
            TempData.Keep("DocumentoElector");
            TempData.Keep("CodigoValidado");

            var puestos = await _votacionService.ObtenerPuestosDisponiblesAsync(documento);

            // Cargar qué puestos ya fueron seleccionados de la sesión
            foreach(var puesto in puestos)
            {
                // Si en la sesión existe una clave "Voto_PuestoId", significa que ya seleccionó algo
                var seleccion = HttpContext.Session.GetInt32($"Voto_{puesto.PuestoId}");
                puesto.YaSelecciono = seleccion.HasValue;
            }

            return View(puestos);
        }

        public async Task<IActionResult> Boleta(int id)
        {
            var documento = TempData["DocumentoElector"]?.ToString();
            if (string.IsNullOrEmpty(documento) || TempData["CodigoValidado"] == null)
            {
                return RedirectToAction("Index");
            }
            TempData.Keep("DocumentoElector");
            TempData.Keep("CodigoValidado");

            var boleta = await _votacionService.ObtenerBoletaPuestoAsync(id);
            if (boleta == null) return RedirectToAction("PuestosDisponibles");

            var seleccion = HttpContext.Session.GetInt32($"Voto_{id}");
            boleta.CandidatoSeleccionadoId = seleccion;

            return View(boleta);
        }

        [HttpPost]
        public IActionResult GuardarVoto(int PuestoId, int CandidatoSeleccionadoId)
        {
            var documento = TempData["DocumentoElector"]?.ToString();
            if (string.IsNullOrEmpty(documento) || TempData["CodigoValidado"] == null)
            {
                return RedirectToAction("Index");
            }
            TempData.Keep("DocumentoElector");
            TempData.Keep("CodigoValidado");

            HttpContext.Session.SetInt32($"Voto_{PuestoId}", CandidatoSeleccionadoId);

            return RedirectToAction("PuestosDisponibles");
        }

        [HttpPost]
        public async Task<IActionResult> FinalizarVotacion()
        {
            var documento = TempData["DocumentoElector"]?.ToString();
            if (string.IsNullOrEmpty(documento) || TempData["CodigoValidado"] == null)
            {
                return RedirectToAction("Index");
            }

            var puestos = await _votacionService.ObtenerPuestosDisponiblesAsync(documento);
            var selecciones = new System.Collections.Generic.Dictionary<int, int>();
            var puestosFaltantes = new System.Collections.Generic.List<string>();

            foreach (var puesto in puestos)
            {
                var seleccion = HttpContext.Session.GetInt32($"Voto_{puesto.PuestoId}");
                if (!seleccion.HasValue)
                {
                    puestosFaltantes.Add(puesto.NombrePuesto);
                }
                else
                {
                    selecciones[puesto.PuestoId] = seleccion.Value;
                }
            }

            if (puestosFaltantes.Count > 0)
            {
                TempData.Keep("DocumentoElector");
                TempData.Keep("CodigoValidado");
                TempData["ErrorMessage"] = $"Debe completar su selección para los siguientes puestos electivos: {string.Join(", ", puestosFaltantes)}.";
                return RedirectToAction("PuestosDisponibles");
            }

            var result = await _votacionService.FinalizarVotacionAsync(documento, selecciones);

            if (result.Success)
            {
                HttpContext.Session.Clear(); // Borramos la sesión
                return RedirectToAction("VotacionCompletada");
            }
            
            TempData.Keep("DocumentoElector");
            TempData.Keep("CodigoValidado");
            return RedirectToAction("PuestosDisponibles");
        }

        public IActionResult VotacionCompletada()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
