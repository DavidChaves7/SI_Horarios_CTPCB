using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Horario
    {
        [Key]
        public int Id_Horario { get; set; }
        public string? Dia { get; set; }
        public string? Id_Profesor { get; set; }
        public int Id_Materia { get; set; }
        public int Id_Grupo { get; set; }
        public TimeSpan Hora_Inicio { get; set; }
        public TimeSpan Hora_Fin { get; set; }
        public string? Estado { get; set; }
      
    }
}
