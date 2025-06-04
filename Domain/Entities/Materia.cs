using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Materia
    {
        [Key]
        public int Id_Materia { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Color { get; set; }
        public string Estado { get; set; }
    }
}
