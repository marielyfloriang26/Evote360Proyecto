using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Evote360.Application.DTOs.Alianza;
using Evote360.Application.ViewModels.AlianzaPolitica;
using Evote360.Application.Interfaces;
using Evote360.Core.Enums;

namespace Evote360.Web.Controllers;

// [Authorize(Roles = nameof(RolUsuarioEnum.DirigentePolitico))] // Asegura que solo los dirigentes entren a este módulo
public class AlianzaPoliticaController : Controller
{
    private readonly IAlianzaPoliticaService _allianceService;

    public AlianzaPoliticaController(IAlianzaPoliticaService allianceService)
    {
        _allianceService = allianceService;
    }

    // Pantalla de Inicio (Listados)

    public async Task<IActionResult> Index()
    {
        int usuarioId = 1;
        int partidoId = 1;
       /* COMENTADO PA PROBAR // Obtiene el id del Usuario Autenticado desde los Claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int usuarioId))
        {
            return RedirectToAction("Login", "Account");
        } */

        // Busca el PartidoId asignado a este dirigente a traves del servicio
      /*  int? partidoId = await _allianceService.ObtenerPartidoIdPorUsuarioIdAsync(usuarioId);
        if (partidoId == null)
        {
            TempData["ErrorMessage"] = "Usted no tiene un partido político asignado. Contacte al administrador.";
            return RedirectToAction("Index", "Home");
        } */
  
        // El controlador llama al servicio y recibe estrictamente dtos
        var recibidasDto = await _allianceService.GetSolicitudesRecibidasAsync(partidoId);
        var enviadasDto = await _allianceService.GetSolicitudesEnviadasAsync(partidoId);
        var vigentesDto = await _allianceService.GetAlianzasVigentesAsync(partidoId);
        bool existeEleccionActiva = await _allianceService.ExisteEleccionActivaAsync();
      /*  var recibidasDto = await _allianceService.GetSolicitudesRecibidasAsync(partidoId.Value);
        var enviadasDto = await _allianceService.GetSolicitudesEnviadasAsync(partidoId.Value);
        var vigentesDto = await _allianceService.GetAlianzasVigentesAsync(partidoId.Value);
        bool existeEleccionActiva = await _allianceService.ExisteEleccionActivaAsync(); */

        // Mapeo de dto hacia el ViewModel requerido por la Vista
        var viewModel = new AlianzaPoliticaViewModel
        {
            ExisteEleccionActiva = existeEleccionActiva,
            
            SolicitudesPendientes = recibidasDto.Select(dto => new SolicitudAlianzaViewModel
            {
                Id = dto.Id,
                PartidoNombre = dto.PartidoSolicitanteNombre,
                PartidoSiglas = dto.PartidoSolicitanteSiglas,
                FechaSolicitud = dto.FechaSolicitud.ToString("dd/MM/yyyy hh:mm tt"),
                Estado = dto.Estado
            }).ToList(),

            SolicitudesRealizadas = enviadasDto.Select(dto => new SolicitudAlianzaViewModel
            {
                Id = dto.Id,
                PartidoNombre = dto.PartidoReceptorNombre,
                PartidoSiglas = dto.PartidoReceptorSiglas,
                FechaSolicitud = dto.FechaSolicitud.ToString("dd/MM/yyyy hh:mm tt"),
                Estado = dto.Estado
            }).ToList(),

            AlianzasVigentes = vigentesDto.Select(dto => new AlianzaVigenteViewModel
            {
                PartidoAliadoNombre = dto.PartidoMayoristaId == partidoId ? dto.PartidoAliadoNombre : dto.PartidoMayoristaNombre,
            PartidoAliadoSiglas = dto.PartidoMayoristaId == partidoId ? dto.PartidoAliadoSiglas : dto.PartidoMayoristaSiglas
               /* Id = dto.Id,
                // Si mi partido es el mayorista, muestro el nombre del aliado de lo contrario el del mayorista
                PartidoAliadoNombre = dto.PartidoMayoristaId == partidoId.Value ? dto.PartidoAliadoNombre : dto.PartidoMayoristaNombre,
                PartidoAliadoSiglas = dto.PartidoMayoristaId == partidoId.Value ? dto.PartidoAliadoSiglas : dto.PartidoMayoristaSiglas,
                FechaAceptacion = dto.FechaAceptacion.ToString("dd/MM/yyyy hh:mm tt") */
            }).ToList()
        };

        return View(viewModel);
    }

    // Crear Nueva Solicitud (GET / POST)

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        if (await _allianceService.ExisteEleccionActivaAsync())
        {
            TempData["ErrorMessage"] = "No se puede crear una solicitud de alianza mientras exista una elección activa.";
            return RedirectToAction(nameof(Index));
        }

        int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        int? partidoId = await _allianceService.ObtenerPartidoIdPorUsuarioIdAsync(usuarioId);

        // Obtiene partidos disponibles mapeados desde el DTO del servicio
        var partidosDto = await _allianceService.GetPartidosDisponiblesParaAlianzaAsync(partidoId!.Value);

        // Construir el vm para la vista del formulario
        var viewModel = new CrearSolicitudAlianzaViewModel
        {
            PartidosDisponibles = partidosDto.Select(p => new SelectListItem
            {
                Value = p.Value,
                Text = p.Text
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearSolicitudAlianzaViewModel viewModel)
    {
        int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        int? partidoId = await _allianceService.ObtenerPartidoIdPorUsuarioIdAsync(usuarioId);

        // Si la validacion de DataAnnotations del ViewModel falla 
        if (!ModelState.IsValid)
        {
            var partidosDto = await _allianceService.GetPartidosDisponiblesParaAlianzaAsync(partidoId!.Value);
            viewModel.PartidosDisponibles = partidosDto.Select(p => new SelectListItem { Value = p.Value, Text = p.Text }).ToList();
            return View(viewModel);
        }

        // Mapea el ViewModel que vino de la vista a un dto de guardado para el servicio
        var saveDto = new SaveSolicitudAlianzaDto
        {
            PartidoSolicitanteId = partidoId!.Value,
            PartidoReceptorId = viewModel.PartidoReceptorId
        };

        // Ejecuta accion en el servicio y captura si hay errores de negocio
        string? errorNegocio = await _allianceService.CrearSolicitudAsync(saveDto);
        
        if (errorNegocio != null)
        {
            ModelState.AddModelError(string.Empty, errorNegocio);
            var partidosDto = await _allianceService.GetPartidosDisponiblesParaAlianzaAsync(partidoId!.Value);
            viewModel.PartidosDisponibles = partidosDto.Select(p => new SelectListItem { Value = p.Value, Text = p.Text }).ToList();
            return View(viewModel);
        }

        TempData["SuccessMessage"] = "Solicitud de alianza enviada exitosamente.";
        return RedirectToAction(nameof(Index));
    }


    // Acciones de Control

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Aceptar(int id)
    {
        int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        int? partidoId = await _allianceService.ObtenerPartidoIdPorUsuarioIdAsync(usuarioId);

        string? error = await _allianceService.AceptarSolicitudAsync(id, partidoId!.Value);
        if (error != null) TempData["ErrorMessage"] = error;
        else TempData["SuccessMessage"] = "Alianza política aceptada y establecida formalmente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rechazar(int id)
    {
        int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        int? partidoId = await _allianceService.ObtenerPartidoIdPorUsuarioIdAsync(usuarioId);

        string? error = await _allianceService.RechazarSolicitudAsync(id, partidoId!.Value);
        if (error != null) TempData["ErrorMessage"] = error;
        else TempData["SuccessMessage"] = "Solicitud de alianza rechazada.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarSolicitud(int id)
    {
        int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        int? partidoId = await _allianceService.ObtenerPartidoIdPorUsuarioIdAsync(usuarioId);

        string? error = await _allianceService.EliminarSolicitudAsync(id, partidoId!.Value);
        if (error != null) TempData["ErrorMessage"] = error;
        else TempData["SuccessMessage"] = "Solicitud cancelada y retirada con éxito.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarAlianza(int id)
    {
        int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        int? partidoId = await _allianceService.ObtenerPartidoIdPorUsuarioIdAsync(usuarioId);

        string? error = await _allianceService.EliminarAlianzaAsync(id, partidoId!.Value);
        if (error != null) TempData["ErrorMessage"] = error;
        else TempData["SuccessMessage"] = "Alianza política disuelta de manera exitosa.";

        return RedirectToAction(nameof(Index));
    }

}