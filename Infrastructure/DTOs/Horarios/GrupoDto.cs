using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.Horarios
{
    public class GrupoDto
    {
        public int Id_Grupo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Id_Nivel_Academico { get; set; }
        public string Seccion { get; set; } = string.Empty;
        public string Id_Profesor_Guia { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

    }
    
}
