using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.Horarios
{
    public class HorarioGenDto
    {
        public string Dia { get; set; } = string.Empty; // "L", "K", etc.
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string NombreMateria { get; set; } = string.Empty;
        public string Profesor { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }
}
