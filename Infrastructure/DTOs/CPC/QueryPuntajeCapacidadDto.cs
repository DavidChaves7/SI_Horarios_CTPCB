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
    public class QueryPuntajeCapacidadDto
    {
        //[Required]
        //[DefaultValue("0001")]
        //public string COD_COMPANIA { get; set; } = null!;
        //[NotNull]
        //[Required]
       
        //public short ID { get; set; }
     
        public long? PUNTAJE { get; set; }

        public long? NUM_DIAS_MAX_INICIAL { get; set; }

        public long? NUM_DIAS_MAX_FINAL { get; set; }

        public long? NUM_DIAS_MEDIO_INICIAL { get; set; }

        public long? NUM_DIAS_MEDIO_FINAL { get; set; }
    }
}
