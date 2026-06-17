using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Evote360.Application.ViewModels.Administrador
{
    public class ResumenEleccionViewModel
    {
        public string NombreEleccion { get; set; } = string.Empty;
        public string FechaRealizacion { get; set; } = string.Empty;
        public int CantidadPartidos { get; set; }
        public int CantidadCandidatosReales { get; set; } // Aplicando Distinct para evitar duplicados por alianzas
        public int CantidadCiudadanosVotaron { get; set; }
    }
}