using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.Mantenimientos
{
    public class Profesor_X_MateriaDto
    {
        public int Id_Prof_Materia { get; set; }
        public int Id_Materia { get; set; }
        public string? Desc_Materia { get; set; } = "";
        public int Id_Profesor { get; set; }
        public string? Desc_Profesor { get; set; } = "";
        public string Estado { get; set; }
    }
}
