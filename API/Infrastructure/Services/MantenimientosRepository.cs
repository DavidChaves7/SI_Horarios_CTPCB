using Application.Contexts;
using AutoMapper;
using Azure.Core;
using Domain.Entities;
using Infrastructure.DTOs;
using Infrastructure.DTOs.Mantenimientos;
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

        public async Task<IEnumerable<EjemploMantDto>> GetAllEjemploMant()
        {
            //var parameters = await _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>()
            //             .Select(s => _mapper.Map<QueryPuntajeCapacidadDto>(s)).ToListAsync();
            var entity = new EjemploMantDto()
            {
                ID = 1,
                DESCRIPCION = "Ejemplo",
                TEST = "TEST"

            };
            var parameters = new List<EjemploMantDto>().Append(entity);
            return parameters;
        }

        #endregion

        #region Ejemplo 2

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

        #region Materias
        public async Task<MateriaDto> AddUpdateMaterias(MateriaDto data)
        {
            try
            {
                if (data is null)
                {
                    throw new Exception("Debe de establecer los campos para el objeto");
                }
                var param = _mapper.Map<Materia>(data);

                var dbObject = _unitOfWork.Set<Materia>().FirstOrDefault(x => x.Id_Materia == param.Id_Materia);
                if (dbObject != null)
                {
                    _unitOfWork.Set<Materia>().Entry(dbObject).CurrentValues.SetValues(data);
                    _unitOfWork.Set<Materia>().Update(dbObject);
                }
                else
                {
                    var test1 = await _unitOfWork.Set<Materia>().AddAsync(param);
                }
                var test2 = await _unitOfWork.SaveChangesAsync();

                return data;
            }
            catch (Exception ex)
            {
                return new MateriaDto();
            }

        }
        #endregion

        #region Profesores

        public async Task<ProfesorDto?> AddUpdateProfesor(ProfesorDto data)
        {
            try
            {
                if (data is null)
                {
                    throw new Exception("Debe de establecer los campos para el objeto ");
                }
                var param = _mapper.Map<Profesor>(data);

                var dbObject = _unitOfWork.Set<Profesor>().FirstOrDefault(x => x.Id_Profesor == param.Id_Profesor);
                if (dbObject != null)
                {
                    _unitOfWork.Set<Profesor>().Entry(dbObject).CurrentValues.SetValues(data);
                    _unitOfWork.Set<Profesor>().Update(dbObject);
                }
                else
                {
                    await _unitOfWork.Set<Profesor>().AddAsync(param);
                }
                await _unitOfWork.SaveChangesAsync();

                return data;
            }
            catch (Exception ex)
            {
                return new ProfesorDto();
            }

        }

        public async Task<ProfesorDto> DeleteProfesor(ProfesorDto data)
        {
            if (data is null)
            {
                throw new Exception("Debe de establecer los campos para el objeto ");
            }
            var param = _mapper.Map<Profesor>(data);

            var dbObject = _unitOfWork.Set<Profesor>().AsNoTracking().FirstOrDefault(x => x.Id_Profesor == param.Id_Profesor);
            if (dbObject != null)
            {
                _unitOfWork.Set<Profesor>().Entry(dbObject).CurrentValues.SetValues(data);
                dbObject.Estado = "I";
                _unitOfWork.Set<Profesor>().Update(dbObject);
            }
            else
            {
                throw new Exception("No se encontro el Parámetro a eliminar");
            }
            await _unitOfWork.SaveChangesAsync();

            return data;
        }

        public async Task<IEnumerable<ProfesorDto>> GetAllProfesor()
        {
            var parameters = await _unitOfWork.Set<Profesor>()
                         .Select(s => _mapper.Map<ProfesorDto>(s)).ToListAsync();
            return parameters;
        }

        public async Task<ProfesorDto> GetOneProfesor(ProfesorDto data)
        {
            if (data is null)
            {
                throw new Exception("Debe de establecer los campos para el objeto ");
            }

            var param = _mapper.Map<Profesor>(data);
            var dbObject = await _unitOfWork.Set<Profesor>().FirstOrDefaultAsync(x => x.Id_Profesor == param.Id_Profesor);
            if (dbObject != null)
            {
                var res = _mapper.Map<ProfesorDto>(dbObject);
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
