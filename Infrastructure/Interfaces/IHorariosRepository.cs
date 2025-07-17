using Domain.Entities;
using Infrastructure.DTOs;
using Infrastructure.DTOs.Horarios;
using Infrastructure.DTOs.Mantenimientos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IHorariosRepository
    {


        #region Horarios
        Task<List<HorarioDto>?> GenHorario(int grupoId);
        Task<List<HorarioDto>?> GetOneHorario(HorarioDto data);
        #endregion
        Task<HorarioDto?> UpdateHorario(HorarioDto data);


    }
}
