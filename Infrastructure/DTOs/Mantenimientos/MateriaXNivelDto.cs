using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.Mantenimientos
{
    public class MateriaXNivelDto
    {
        public int Id_Mat_X_Nivel { get; set; }
        [Required]
        public int Id_Materia { get; set; }
        public string? Desc_Materia { get; set; }
        [Required]
        public int Id_Nivel_Academico { get; set; }
        public string? Desc_Nivel_Academico { get; set; }
        public int? Prioridad { get; set; }

        public int? Carga_Horaria { get; set; }
        public string? Estado { get; set; }

    }
}
