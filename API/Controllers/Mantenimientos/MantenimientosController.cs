using Infrastructure.DTOs;
using Infrastructure.DTOs.Mantenimientos;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace API.Controllers.CPC
{
    [Route("api/[controller]")]
    [ApiController]
    public class MantenimientosController : ApiControllerBase
    {
        #region Mantenimientos 

        #region Ejemplo
        [HttpGet("GetAllEjemploMant")]
        public async Task<IEnumerable<EjemploMantDto>> GetAllEjemploMantAsync() => await _mantenimientosRepository.GetAllEjemploMant();
        #endregion

        #region Ejemplo 2
        [HttpGet("GetAllPuntajeCapacidad")]
        public async Task<IEnumerable<QueryPuntajeCapacidadDto>> GetAllPuntajeCapacidadAsync() => await _mantenimientosRepository.GetAllPuntajeCapacidad();
        [HttpGet("GetOnePuntajeCapacidad")]
        public async Task<QueryPuntajeCapacidadDto> GetOnePuntajeCapacidadAsync(QueryPuntajeCapacidadDto param) => await _mantenimientosRepository.GetOnePuntajeCapacidad(param);

        [HttpPost("AddUpdatePuntajeCapacidad")]
        public async Task<QueryPuntajeCapacidadDto?> AddUpdatePuntajeCapacidadAsync(QueryPuntajeCapacidadDto param) => await _mantenimientosRepository.AddUpdatePuntajeCapacidad(param);

        [HttpPost("DeletePuntajeCapacidad")]
        public async Task<QueryPuntajeCapacidadDto?> DeletePuntajeCapacidadAsync(QueryPuntajeCapacidadDto param) => await _mantenimientosRepository.DeletePuntajeCapacidad(param);
        #endregion

        #region Materias

        [HttpPost("AddUpdateMateriasDesdeExcel")]

        public async Task<MateriaDto?> AddUpdateMateriasDesdeExcel(MateriaDto param) => await _mantenimientosRepository.AddUpdateMaterias(param);
        [HttpGet("GetAllMaterias")]
        public async Task<IEnumerable<MateriaDto>> GetAllMateriasAsync() => await _mantenimientosRepository.GetAllMaterias();
        [HttpGet("GetOneMaterias")]
        public async Task<MateriaDto> GetOneMateriasAsync(MateriaDto param) => await _mantenimientosRepository.GetOneMaterias(param);

        [HttpPost("AddUpdateMaterias")]
        public async Task<MateriaDto?> AddUpdateMateriasAsync(MateriaDto param) => await _mantenimientosRepository.AddUpdateMaterias(param);

        [HttpPost("DeleteMaterias")]
        public async Task<MateriaDto?> DeleteMateriasAsync(MateriaDto param) => await _mantenimientosRepository.DeleteMaterias(param);


        #endregion

        #region Profesor 2
        [HttpGet("GetAllProfesor")]
        public async Task<IEnumerable<ProfesorDto>> GetAllProfesorAsync() => await _mantenimientosRepository.GetAllProfesor();
        [HttpGet("GetOneProfesor")]
        public async Task<ProfesorDto> GetOneProfesorAsync(ProfesorDto param) => await _mantenimientosRepository.GetOneProfesor(param);

        [HttpPost("AddUpdateProfesor")]
        public async Task<ProfesorDto?> AddUpdateProfesorAsync(ProfesorDto param) => await _mantenimientosRepository.AddUpdateProfesor(param);

        [HttpPost("DeleteProfesor")]
        public async Task<ProfesorDto?> DeleteProfesorAsync(ProfesorDto param) => await _mantenimientosRepository.DeleteProfesor(param);
        #endregion

        #region Profesor_X_Materia

        
        [HttpGet("GetAllProfesor_X_Materia")]
        public async Task<IEnumerable<Profesor_X_MateriaDto>> GetAllProfesor_X_MateriaAsync() => await _mantenimientosRepository.GetAllProfesor_X_Materia();
        [HttpGet("GetOneProfesor_X_Materia")]
        public async Task<Profesor_X_MateriaDto> GetOneProfesor_X_MateriaAsync(Profesor_X_MateriaDto param) => await _mantenimientosRepository.GetOneProfesor_X_Materias(param);

        [HttpPost("AddUpdateProfesor_X_Materia")]
        public async Task<Profesor_X_MateriaDto?> AddUpdateProfesor_X_MateriaAsync(Profesor_X_MateriaDto param) => await _mantenimientosRepository.AddUpdateProfesor_X_Materia(param);

        [HttpPost("DeleteProfesor_X_Materia")]
        public async Task<Profesor_X_MateriaDto?> DeleteProfesor_X_MateriaAsync(Profesor_X_MateriaDto param) => await _mantenimientosRepository.DeleteProfesor_X_Materia(param);

        #endregion

        #endregion
    }
}

