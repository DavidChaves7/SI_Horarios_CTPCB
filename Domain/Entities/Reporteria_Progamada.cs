using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Reporteria_Progamada
    {
        [Key]
        public int Id_Rep_Programada { get; set; }
        public string? Correos { get; set; }
        public string? Frecuencia { get; set; }
        public DateTime? Fecha_Hora_Envio { get; set; }
        public string? Estado { get; set; }
    }
}
