using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Materia_X_Nivel
    {
        [Key]
        public int Id_Mat_X_Nivel { get; set; }
        public int Id_Materia { get; set; }
        public int Id_Nivel_Academico { get; set; }
        public int? Prioridad { get; set; }
        public int? Carga_Horaria { get; set; }
        public string? Estado { get; set; }
    }
}
