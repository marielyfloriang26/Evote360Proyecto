using System;
using System.Collections.Generic;

namespace Evote360.Application.DTOs
{
    public class ResultadoOpcionDto
    {
        public string CandidatoNombre { get; set; } = null!;
        public string PartidoNombre { get; set; } = null!;
        public string PartidoSiglas { get; set; } = null!;
        public int CantidadVotos { get; set; }
        public double PorcentajeVotos { get; set; }
        public bool EsGanador { get; set; }
    }
}