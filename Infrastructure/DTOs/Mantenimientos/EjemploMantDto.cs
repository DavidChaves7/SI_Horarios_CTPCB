using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DTOs
{
    public class EjemploMantDto
    {
        //[Required]
        //[DefaultValue("0001")]
        //public string COD_COMPANIA { get; set; } = null!;
        //[NotNull]
        //[Required]
       
        //public short ID { get; set; }
     
        public long? ID { get; set; }

        public string? DESCRIPCION { get; set; }
        public string? TEST { get; set; }

       
    }
}
