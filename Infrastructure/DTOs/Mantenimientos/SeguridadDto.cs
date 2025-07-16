using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.Mantenimientos
{
    public class SeguridadDto
    {
        public int Id_Usuario { get; set; }
        public string? Cedula { get; set; }
        public string? Correo { get; set; }
        public string? Estado { get; set; }
    }

}
