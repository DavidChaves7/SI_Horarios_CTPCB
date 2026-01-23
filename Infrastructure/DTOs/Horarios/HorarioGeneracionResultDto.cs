using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.Horarios
{
    /// <summary>
    /// DTO para retornar el resultado de la generación de horarios con métricas
    /// </summary>
    public class HorarioGeneracionResultDto
    {
        public bool Exitoso { get; set; }
        public List<HorarioDto> Horarios { get; set; } = new();
        public List<string> Errores { get; set; } = new();
        public List<string> Advertencias { get; set; } = new();
        public Dictionary<string, object> Metricas { get; set; } = new();
        public string Mensaje { get; set; } = string.Empty;
    }
}
