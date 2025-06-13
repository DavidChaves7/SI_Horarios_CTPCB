using Domain.Entities;
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
        Task<MateriaDto> DeleteMaterias(MateriaDto data);
        Task<IEnumerable<MateriaDto>> GetAllMaterias();
        Task<MateriaDto> GetOneMaterias(MateriaDto data);
        #endregion

        #region Profesor
        Task<ProfesorDto?> AddUpdateProfesor(ProfesorDto data);
        Task<ProfesorDto> DeleteProfesor(ProfesorDto data);
        Task<IEnumerable<ProfesorDto>> GetAllProfesor();
        Task<ProfesorDto> GetOneProfesor(ProfesorDto data);
        #endregion

        #region Profersor_X_Materia
        Task<Profesor_X_MateriaDto> AddUpdateProfesor_X_Materia(Profesor_X_MateriaDto data);
        Task<Profesor_X_MateriaDto> DeleteProfesor_X_Materia(Profesor_X_MateriaDto data);
        Task<IEnumerable<Profesor_X_MateriaDto>> GetAllProfesor_X_Materia();
        Task<Profesor_X_MateriaDto> GetOneProfesor_X_Materias(Profesor_X_MateriaDto data);
        #endregion

        #region Restriccion_Profesor
        Task<Restriccion_ProfesorDto?> AddUpdateRestriccion_Profesor(Restriccion_ProfesorDto data);
        Task<Restriccion_ProfesorDto> DeleteRestriccion_Profesor(Restriccion_ProfesorDto data);
        Task<IEnumerable<Restriccion_ProfesorDto>> GetAllRestriccion_Profesor();
        Task<Restriccion_ProfesorDto> GetOneRestriccion_Profesor(Restriccion_ProfesorDto data);
        #endregion

        #endregion



    }
}
