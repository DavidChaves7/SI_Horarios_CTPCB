using Infrastructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IMantenimientosRepository
    {
        #region Mantenimientos

        #region Ejempplo
        Task<IEnumerable<EjemploMantDto>> GetAllEjemploMant();
        #endregion
        Task<QueryPuntajeCapacidadDto?> AddUpdatePuntajeCapacidad(QueryPuntajeCapacidadDto data);
        Task<QueryPuntajeCapacidadDto> DeletePuntajeCapacidad(QueryPuntajeCapacidadDto data);
        Task<IEnumerable<QueryPuntajeCapacidadDto>> GetAllPuntajeCapacidad();
        Task<QueryPuntajeCapacidadDto> GetOnePuntajeCapacidad(QueryPuntajeCapacidadDto data);
       


        #endregion

    }
}
