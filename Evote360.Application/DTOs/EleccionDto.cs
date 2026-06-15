using System;
using System.Collections.Generic;

namespace Evote360.Application.DTOs
{
    public class EleccionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string EstadoElectoral { get; set; } = null!;
        public int CantidadPartidos { get; set; }
        public int CantidadPuestos { get; set; }
        public int CantidadCiudadanosVotaron { get; set; }
    }

}