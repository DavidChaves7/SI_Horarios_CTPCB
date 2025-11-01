using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.Mantenimientos
{
    public class SeguridadDto
    {
        public int Id_Usuario { get; set; }
        [Required]
        [StringLength(9, ErrorMessage = "Ingrese una Cedula Valida")]
        public  string Cedula { get; set; } = string.Empty;
        [Required]
        [EmailAddress(ErrorMessage = "Correo Invalido")]
        public  string Correo { get; set; } = string.Empty;
        [Required]
        public  string Estado { get; set; } = string.Empty;
    }

}
