using Infrastructure.DTOs;
using Infrastructure.DTOs.Mantenimientos;
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

        #region Ejemplo
        Task<IEnumerable<EjemploMantDto>> GetAllEjemploMant();
        #endregion

        #region Ejemplo 2
        Task<QueryPuntajeCapacidadDto?> AddUpdatePuntajeCapacidad(QueryPuntajeCapacidadDto data);
        Task<QueryPuntajeCapacidadDto> DeletePuntajeCapacidad(QueryPuntajeCapacidadDto data);
        Task<IEnumerable<QueryPuntajeCapacidadDto>> GetAllPuntajeCapacidad();
        Task<QueryPuntajeCapacidadDto> GetOnePuntajeCapacidad(QueryPuntajeCapacidadDto data);
        #endregion

        #region Materias
        Task<MateriaDto> AddUpdateMaterias(MateriaDto data);
        #endregion


        #region Profesor
        Task<ProfesorDto?> AddUpdateProfesor(ProfesorDto data);
        Task<ProfesorDto> DeleteProfesor(ProfesorDto data);
        Task<IEnumerable<ProfesorDto>> GetAllProfesor();
        Task<ProfesorDto> GetOneProfesor(ProfesorDto data);
        #endregion

        #endregion



    }
}
