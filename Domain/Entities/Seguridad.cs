using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Seguridad
    {
        [Key]
        public int Id_Usuario { get; set; }
        public string? Cedula { get; set; }
        public string? Correo { get; set; }
        public string? Estado { get; set; }
    }

}
