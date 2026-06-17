using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Evote360.Application.ViewModels.AlianzaPolitica;

public class CrearSolicitudAlianzaViewModel
{
    [Required(ErrorMessage = "El partido político es requerido.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un partido político válido.")]
    public int PartidoReceptorId { get; set; }

    // Propiedad para llenar el elemento select en la vista con los partidos permitidos
    public List<SelectListItem>? PartidosDisponibles { get; set; }
}