using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Restriccion_Profesor
    {
        [Key]
        public int Id_Restriccion { get; set; }
        public int Id_Profesor { get; set; }
        public string? Razon { get; set; }
        public string? Dia { get; set; }
        public string? Hora_Inicio { get; set; }
        public string? Hora_Fin { get; set; }
        public string? Estado { get; set; }
    }
}
