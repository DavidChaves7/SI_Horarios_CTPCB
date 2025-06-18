using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.Mantenimientos
{
    public class MateriaDto
    {
        public int Id_Materia { get; set; }
        public string? Nombre { get; set; }
        public string? Tipo { get; set; }
        public string? Color { get; set; }
        public string? Estado { get; set; }
    }
}
