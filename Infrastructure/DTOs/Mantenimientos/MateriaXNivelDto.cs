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
        [Required(ErrorMessage = "La Materia no puede estar vacia !")]
        public int? Id_Materia { get; set; } = null!;
        public string? Desc_Materia { get; set; }
        [Required(ErrorMessage ="El Nivel Academico no puede estar vacio !")]
        public int? Id_Nivel_Academico { get; set; } = null!;
        public string? Desc_Nivel_Academico { get; set; }
        public int? Prioridad { get; set; }

        public int? Carga_Horaria { get; set; }
        public string? Estado { get; set; }

    }
}
