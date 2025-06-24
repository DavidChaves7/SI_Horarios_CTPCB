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
        //[HttpGet("GetAllPuntajeCapacidad")]
        //public async Task<IEnumerable<QueryPuntajeCapacidadDto>> GetAllPuntajeCapacidadAsync() => await _mantenimientosRepository.GetAllPuntajeCapacidad();
        //[HttpGet("GetOnePuntajeCapacidad")]
        //public async Task<QueryPuntajeCapacidadDto> GetOnePuntajeCapacidadAsync(QueryPuntajeCapacidadDto param) => await _mantenimientosRepository.GetOnePuntajeCapacidad(param);

        //[HttpPost("AddUpdatePuntajeCapacidad")]
        //public async Task<QueryPuntajeCapacidadDto?> AddUpdatePuntajeCapacidadAsync(QueryPuntajeCapacidadDto param) => await _mantenimientosRepository.AddUpdatePuntajeCapacidad(param);

        //[HttpPost("DeletePuntajeCapacidad")]
        //public async Task<QueryPuntajeCapacidadDto?> DeletePuntajeCapacidadAsync(QueryPuntajeCapacidadDto param) => await _mantenimientosRepository.DeletePuntajeCapacidad(param);
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

        #region Nivel Academico
        [HttpGet("GetAllNivelAcademico")]
        public async Task<IEnumerable<NivelAcademicoDto>> GetAllNivelAcademicoAsync() => await _mantenimientosRepository.GetAllNivelAcademico();
        [HttpGet("GetOneNivelAcademico")]
        public async Task<NivelAcademicoDto> GetOneNivelAcademicoAsync(NivelAcademicoDto param) => await _mantenimientosRepository.GetOneNivelAcademico(param);

        [HttpPost("AddUpdateNivelAcademico")]
        public async Task<NivelAcademicoDto?> AddUpdateNivelAcademicoAsync(NivelAcademicoDto param) => await _mantenimientosRepository.AddUpdateNivelAcademico(param);

        [HttpPost("DeleteNivelAcademico")]
        public async Task<NivelAcademicoDto?> DeleteNivelAcademicoAsync(NivelAcademicoDto param) => await _mantenimientosRepository.DeleteNivelAcademico(param);
        #endregion

        #region Materia X Nivel Academico

        [HttpGet("GetAllMateriaXNivel")]
        public async Task<IEnumerable<MateriaXNivelDto>> GetAllMateriaXNivelAsync() => await _mantenimientosRepository.GetAllMateriaXNivel();
        [HttpGet("GetOneMateriaXNivel")]
        public async Task<MateriaXNivelDto> GetOneMateriaXNivelAsync(MateriaXNivelDto param) => await _mantenimientosRepository.GetOneMateriaXNivel(param);
        [HttpPost("AddUpdateMateriaXNivel")]
        public async Task<MateriaXNivelDto?> AddUpdateMateriaXNivelAsync(MateriaXNivelDto param) => await _mantenimientosRepository.AddUpdateMateriaXNivel(param);
        [HttpPost("DeleteMateriaXNivel")]
        public async Task<MateriaXNivelDto?> DeleteMateriaXNivelAsync(MateriaXNivelDto param) => await _mantenimientosRepository.DeleteMateriaXNivel(param);


        #endregion

        #region Profesor
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

        #region Restriccion_Profesor 
        [HttpGet("GetAllRestriccion_Profesor")]
        public async Task<IEnumerable<Restriccion_ProfesorDto>> GetAllRestriccion_ProfesorAsync() => await _mantenimientosRepository.GetAllRestriccion_Profesor();
        [HttpGet("GetOneRestriccion_Profesor")]
        public async Task<Restriccion_ProfesorDto> GetOneRestriccion_ProfesorAsync(Restriccion_ProfesorDto param) => await _mantenimientosRepository.GetOneRestriccion_Profesor(param);

        [HttpPost("AddUpdateRestriccion_Profesor")]
        public async Task<Restriccion_ProfesorDto?> AddUpdateRestriccion_ProfesorAsync(Restriccion_ProfesorDto param) => await _mantenimientosRepository.AddUpdateRestriccion_Profesor(param);

        [HttpPost("DeleteRestriccion_Profesor")]
        public async Task<Restriccion_ProfesorDto?> DeleteRestriccion_ProfesorAsync(Restriccion_ProfesorDto param) => await _mantenimientosRepository.DeleteRestriccion_Profesor(param);
        #endregion

        #endregion
    }
}

