using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Grupo
    {
        [Key]
        public int Id_Grupo { get; set; }
        public string? Nombre { get; set; }
        public int Id_Nivel_Academico { get; set; }
        public string? Seccion { get; set; }
        public string? Id_Profesor_Guia { get; set; }
        public string? Estado { get; set; }
    }
}
