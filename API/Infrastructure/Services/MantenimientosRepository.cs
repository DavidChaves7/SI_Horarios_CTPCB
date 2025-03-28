using Application.Contexts;
using AutoMapper;
using Azure.Core;
using Domain.Entities;
using Infrastructure.DTOs;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Infrastructure.Services
{
    public class MantenimientosRepository : IMantenimientosRepository
    {
        public readonly AppDbContext _unitOfWork;
        public readonly IMapper _mapper;

        public MantenimientosRepository(IMapper mapper, AppDbContext unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        #region Mantenimientos

        #region Ejemplo

        public async Task<QueryPuntajeCapacidadDto?> AddUpdatePuntajeCapacidad(QueryPuntajeCapacidadDto data)
        {
            try
            {
                if (data is null)
                {
                    throw new Exception("Debe de establecer los campos para el objeto ");
                }
                var param = _mapper.Map<CPC_PUNTAJE_CAPACIDAD>(data);

                var dbObject = _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>().FirstOrDefault(x => x.PUNTAJE == param.PUNTAJE);
                if (dbObject != null)
                {
                    _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>().Entry(dbObject).CurrentValues.SetValues(data);
                    _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>().Update(dbObject);
                }
                else
                {
                    await _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>().AddAsync(param);
                }
                await _unitOfWork.SaveChangesAsync();

                return data;
            }
            catch (Exception ex)
            {
                return new QueryPuntajeCapacidadDto();
            }

        }
        public async Task<QueryPuntajeCapacidadDto> DeletePuntajeCapacidad(QueryPuntajeCapacidadDto data)
        {
            if (data is null)
            {
                throw new Exception("Debe de establecer los campos para el objeto ");
            }
            var param = _mapper.Map<CPC_PUNTAJE_CAPACIDAD>(data);

            var dbObject = _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>().FirstOrDefault(x => x.PUNTAJE == param.PUNTAJE);
            if (dbObject != null)
            {
                _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>().Entry(dbObject).CurrentValues.SetValues(data);
                _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>().Remove(dbObject);
            }
            else
            {
                throw new Exception("No se encontro el Parámetro a eliminar");
            }
            await _unitOfWork.SaveChangesAsync();

            return data;
        }
        public async Task<IEnumerable<QueryPuntajeCapacidadDto>> GetAllPuntajeCapacidad()
        {
            var parameters = await _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>()
                         .Select(s => _mapper.Map<QueryPuntajeCapacidadDto>(s)).ToListAsync();
            return parameters;
        }

        public async Task<QueryPuntajeCapacidadDto> GetOnePuntajeCapacidad(QueryPuntajeCapacidadDto data)
        {
            if (data is null)
            {
                throw new Exception("Debe de establecer los campos para el objeto ");
            }

            var param = _mapper.Map<CPC_PUNTAJE_CAPACIDAD>(data);
            var dbObject = await _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>().FirstOrDefaultAsync(x => x.PUNTAJE == param.PUNTAJE);
            if (dbObject != null)
            {
                var res = _mapper.Map<QueryPuntajeCapacidadDto>(dbObject);
                return res;
            }
            else
            {
                throw new Exception("No se encontro el Parámetro");
            }

        }



        #endregion

        #endregion
    }
}
