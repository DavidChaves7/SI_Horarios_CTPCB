using System;
using System.Collections.Generic;

namespace Infrastructure.DTOs.Horarios
{
    public class ResultadoGeneracionDto
    {
        public bool Exitoso { get; set; }
        public List<string> Errores { get; set; } = new();
        public List<string> Advertencias { get; set; } = new();
        public Dictionary<string, string> Metricas { get; set; } = new();
        public List<HorarioDto>? Horarios { get; set; }
    }
}
