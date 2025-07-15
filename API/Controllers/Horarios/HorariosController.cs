using API.Infrastructure.Services;
using Infrastructure.DTOs;
using Infrastructure.DTOs.Horarios;

//using Infrastructure.DTOs.Horarios;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace API.Controllers.CPC
{
    [Route("api/[controller]")]
    [ApiController]
    public class HorariosController : ApiControllerBase
    {

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

        [HttpPost("GenHorario")]

        public async Task<List<HorarioDto>?> GenHorarioAsync(int grupoId) => await _horariosRepository.GenHorario(grupoId);
        
        

        #endregion



    }
}

