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

        #endregion
    }
}

