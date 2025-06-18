using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.Mantenimientos
{
    public class NivelAcademicoDto 
    {
        public int Id_Nivel_Academico { get; set; }
        public string? Nombre { get; set; }
        public string? Carga_Horaria { get; set; }
        public string? Estado { get; set; }
    }
}
