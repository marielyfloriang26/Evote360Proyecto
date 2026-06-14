using System;
using System.Collections.Generic;

namespace Evote360.Application.DTOs
{
    public class EleccionCreateDto
    {
        public string Nombre { get; set; } = null!;
        public DateTime FechaRealizacion { get; set; } 
    }
    
}