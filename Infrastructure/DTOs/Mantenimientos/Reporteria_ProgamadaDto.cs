using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DTOs
{
    public class Reporteria_ProgamadaDto
    {
        public int Id_Rep_Programada { get; set; }
        public string? Correos { get; set; }
        public string? Frecuencia { get; set; }
        public DateTime? Fecha_Hora_Envio { get; set; }
        public string? Estado { get; set; }


    }
}
