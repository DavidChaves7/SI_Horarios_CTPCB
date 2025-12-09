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

        #region Horarios

        [HttpPost("GenHorario")]

        public async Task<List<HorarioDto>?> GenHorarioAsync(int grupoId) => await _horariosRepository.GenHorario(grupoId);

        [HttpGet("GetOneHorario")]

        public async Task<List<HorarioDto>?> GetOneHorarioAsync(HorarioDto data) => await _horariosRepository.GetOneHorario(data);

        [HttpGet("UpdateHorario")]
        public async Task<HorarioDto?> UpdateHorarioAsync(HorarioDto param) => await _horariosRepository.UpdateHorario(param);


        #endregion




    }
}

