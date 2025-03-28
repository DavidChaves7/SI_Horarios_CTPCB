using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;


namespace Domain.Entities
{
    public class CPC_PUNTAJE_CAPACIDAD
    {
        //[NotNull]
        //[Required]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public short ID { get; set; }
    
        public long? PUNTAJE { get; set; }

        public long? NUM_DIAS_MAX_INICIAL { get; set; }

        public long? NUM_DIAS_MAX_FINAL { get; set; }

        public long? NUM_DIAS_MEDIO_INICIAL { get; set; }

        public long? NUM_DIAS_MEDIO_FINAL { get; set; }
    }
}
