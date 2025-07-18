﻿using Application.Contexts;
using AutoMapper;
using Azure.Core;
using Domain.Entities;
using Infrastructure.DTOs;
using Infrastructure.DTOs.Mantenimientos;
using Infrastructure.DTOs.Horarios;
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

        //public async Task<QueryPuntajeCapacidadDto?> AddUpdatePuntajeCapacidad(QueryPuntajeCapacidadDto data)
        //{
        //    try
        //    {
        //        if (data is null)
        //        {
        //            throw new Exception("Debe de establecer los campos para el objeto ");
        //        }
        //        var param = _mapper.Map<CPC_PUNTAJE_CAPACIDAD>(data);

        //        var dbObject = _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>().FirstOrDefault(x => x.PUNTAJE == param.PUNTAJE);
        //        if (dbObject != null)
        //        {
        //            _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>().Entry(dbObject).CurrentValues.SetValues(data);
        //            _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>().Update(dbObject);
        //        }
        //        else
        //        {
        //            await _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>().AddAsync(param);
        //        }
        //        await _unitOfWork.SaveChangesAsync();

        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new QueryPuntajeCapacidadDto();
        //    }

        //}
        //public async Task<QueryPuntajeCapacidadDto> DeletePuntajeCapacidad(QueryPuntajeCapacidadDto data)
        //{
        //    if (data is null)
        //    {
        //        throw new Exception("Debe de establecer los campos para el objeto ");
        //    }
        //    var param = _mapper.Map<CPC_PUNTAJE_CAPACIDAD>(data);

        //    var dbObject = _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>().FirstOrDefault(x => x.PUNTAJE == param.PUNTAJE);
        //    if (dbObject != null)
        //    {
        //        _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>().Entry(dbObject).CurrentValues.SetValues(data);
        //        _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>().Remove(dbObject);
        //    }
        //    else
        //    {
        //        throw new Exception("No se encontro el Parámetro a eliminar");
        //    }
        //    await _unitOfWork.SaveChangesAsync();

        //    return data;
        //}
        //public async Task<IEnumerable<QueryPuntajeCapacidadDto>> GetAllPuntajeCapacidad()
        //{
        //    var parameters = await _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>()
        //                 .Select(s => _mapper.Map<QueryPuntajeCapacidadDto>(s)).ToListAsync();
        //    return parameters;
        //}
        //public async Task<QueryPuntajeCapacidadDto> GetOnePuntajeCapacidad(QueryPuntajeCapacidadDto data)
        //{
        //    if (data is null)
        //    {
        //        throw new Exception("Debe de establecer los campos para el objeto ");
        //    }

        //    var param = _mapper.Map<CPC_PUNTAJE_CAPACIDAD>(data);
        //    var dbObject = await _unitOfWork.Set<CPC_PUNTAJE_CAPACIDAD>().FirstOrDefaultAsync(x => x.PUNTAJE == param.PUNTAJE);
        //    if (dbObject != null)
        //    {
        //        var res = _mapper.Map<QueryPuntajeCapacidadDto>(dbObject);
        //        return res;
        //    }
        //    else
        //    {
        //        throw new Exception("No se encontro el Parámetro");
        //    }

        //}

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
                    param.Id_Materia = 0;
                    var test1 = await _unitOfWork.Set<Materia>().AddAsync(param);
                }
                var test2 = await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<MateriaDto>(dbObject ?? param);
            }
            catch (Exception ex)
            {
                return new MateriaDto();
            }

        }

        public async Task<MateriaDto> DeleteMaterias(MateriaDto data)
        {
            if (data is null)
            {
                throw new Exception("Debe de establecer los campos para el objeto ");
            }
            var param = _mapper.Map<Materia>(data);

            var dbObject = _unitOfWork.Set<Materia>().FirstOrDefault(x => x.Id_Materia == param.Id_Materia);
            if (dbObject != null)
            {
                _unitOfWork.Set<Materia>().Entry(dbObject).CurrentValues.SetValues(data);
                _unitOfWork.Set<Materia>().Remove(dbObject);
            }
            else
            {
                throw new Exception("No se encontro el Parámetro a eliminar");
            }
            await _unitOfWork.SaveChangesAsync();

            return data;
        }
        public async Task<IEnumerable<MateriaDto>> GetAllMaterias()
        {
            var parameters = await _unitOfWork.Set<Materia>()
                         .Select(s => _mapper.Map<MateriaDto>(s)).ToListAsync();
            return parameters;
        }
        public async Task<MateriaDto> GetOneMaterias(MateriaDto data)
        {
            if (data is null)
            {
                throw new Exception("Debe de establecer los campos para el objeto ");
            }

            var param = _mapper.Map<Materia>(data);
            var dbObject = await _unitOfWork.Set<Materia>().FirstOrDefaultAsync(x => x.Id_Materia == param.Id_Materia);
            if (dbObject != null)
            {
                var res = _mapper.Map<MateriaDto>(dbObject);
                return res;
            }
            else
            {
                throw new Exception("No se encontro el Parámetro");
            }

        }
        #endregion

        #region Nivel Academico

        public async Task<NivelAcademicoDto?> AddUpdateNivelAcademico(NivelAcademicoDto data)
        {
            try
            {
                if (data is null)
                {
                    throw new Exception("Debe de establecer los campos para el objeto ");
                }
                var param = _mapper.Map<Nivel_Academico>(data);
                var dbObject = _unitOfWork.Set<Nivel_Academico>().FirstOrDefault(x => x.Id_Nivel_Academico == param.Id_Nivel_Academico);
                if (dbObject != null)
                {
                    _unitOfWork.Set<Nivel_Academico>().Entry(dbObject).CurrentValues.SetValues(data);
                    _unitOfWork.Set<Nivel_Academico>().Update(dbObject);
                }
                else
                {
                    await _unitOfWork.Set<Nivel_Academico>().AddAsync(param);
                }
                await _unitOfWork.SaveChangesAsync();
                return data;
            }
            catch (Exception ex)
            {
                return new NivelAcademicoDto();
            }

        }

        public async Task<NivelAcademicoDto> DeleteNivelAcademico(NivelAcademicoDto data)
        {
            if (data is null)
            {
                throw new Exception("Debe de establecer los campos para el objeto ");
            }
            var param = _mapper.Map<Nivel_Academico>(data);
            var dbObject = _unitOfWork.Set<Nivel_Academico>().FirstOrDefault(x => x.Id_Nivel_Academico == param.Id_Nivel_Academico);
            if (dbObject != null)
            {
                //todo:
                /* hacer verificacion:
                 * 
                 * En caso de que no tenga materias ni docentes asignados, cuando se presione 'Eliminar' el sistema lo elimina y actualiza el listado.
                 */

                // var MateriasXLeccion = var parameters = await _unitOfWork.Set<MateriaX>()
                // .Select(s => _mapper.Map<NivelAcademicoDto>(s)).ToListAsync();


                _unitOfWork.Set<Nivel_Academico>().Entry(dbObject).CurrentValues.SetValues(data);
                _unitOfWork.Set<Nivel_Academico>().Remove(dbObject);


            }
            else
            {
                throw new Exception("No se encontro el Parámetro a eliminar");
            }
            await _unitOfWork.SaveChangesAsync();
            return data;

        }

        public async Task<IEnumerable<NivelAcademicoDto>> GetAllNivelAcademico()
        {
            var parameters = await _unitOfWork.Set<Nivel_Academico>()
                         .Select(s => _mapper.Map<NivelAcademicoDto>(s)).ToListAsync();
            return parameters;
        }

        public async Task<NivelAcademicoDto> GetOneNivelAcademico(NivelAcademicoDto data)
        {
            if (data is null)
            {
                throw new Exception("Debe de establecer los campos para el objeto ");
            }
            var param = _mapper.Map<Nivel_Academico>(data);
            var dbObject = await _unitOfWork.Set<Nivel_Academico>().FirstOrDefaultAsync(x => x.Id_Nivel_Academico == param.Id_Nivel_Academico);
            if (dbObject != null)
            {
                var res = _mapper.Map<NivelAcademicoDto>(dbObject);
                return res;
            }
            else
            {
                throw new Exception("No se encontro el Parámetro");
            }
        }

        #endregion

        #region MateriaXNivel

        public async Task<MateriaXNivelDto?> AddUpdateMateriaXNivel(MateriaXNivelDto data)
        {
            try
            {
                if (data is null)
                {
                    throw new Exception("Debe de establecer los campos para el objeto ");
                }
                var param = _mapper.Map<Materia_X_Nivel>(data);
                var dbObject = _unitOfWork.Set<Materia_X_Nivel>().FirstOrDefault(x => x.Id_Mat_X_Nivel == param.Id_Mat_X_Nivel);
                if (dbObject != null)
                {
                    _unitOfWork.Set<Materia_X_Nivel>().Entry(dbObject).CurrentValues.SetValues(data);
                    _unitOfWork.Set<Materia_X_Nivel>().Update(dbObject);
                }
                else
                {
                    await _unitOfWork.Set<Materia_X_Nivel>().AddAsync(param);
                }
                await _unitOfWork.SaveChangesAsync();
                return data;
            }
            catch (Exception ex)
            {
                return new MateriaXNivelDto();
            }
        }

        public async Task<MateriaXNivelDto> DeleteMateriaXNivel(MateriaXNivelDto data)
        {
            if (data is null)
            {
                throw new Exception("Debe de establecer los campos para el objeto ");
            }
            var param = _mapper.Map<Materia_X_Nivel>(data);
            var dbObject = _unitOfWork.Set<Materia_X_Nivel>().FirstOrDefault(x => x.Id_Mat_X_Nivel == param.Id_Mat_X_Nivel);
            if (dbObject != null)
            {
                _unitOfWork.Set<Materia_X_Nivel>().Entry(dbObject).CurrentValues.SetValues(data);
                _unitOfWork.Set<Materia_X_Nivel>().Remove(dbObject);
            }
            else
            {
                throw new Exception("No se encontro el Parámetro a eliminar");
            }
            await _unitOfWork.SaveChangesAsync();
            return data;
        }

        public async Task<IEnumerable<MateriaXNivelDto>> GetAllMateriaXNivel()
        {
            var parameters = await _unitOfWork.Set<Materia_X_Nivel>().Select(s => _mapper.Map<MateriaXNivelDto>(s)).ToListAsync();

            var materias = await _unitOfWork.Set<Materia>().Select(s => _mapper.Map<MateriaDto>(s)).ToListAsync();
            var nivelesAcademicos = await _unitOfWork.Set<Nivel_Academico>().Select(s => _mapper.Map<NivelAcademicoDto>(s)).ToListAsync();

            foreach (var item in parameters)
            {
                var materia = materias.FirstOrDefault(m => m.Id_Materia == item.Id_Materia);
                if (materia != null)
                    item.Desc_Materia = materia.Nombre;

                var nivelAcademico = nivelesAcademicos.FirstOrDefault(n => n.Id_Nivel_Academico == item.Id_Nivel_Academico);
                if (nivelAcademico != null)
                    item.Desc_Nivel_Academico = nivelAcademico.Nombre;
            }
            return parameters;
        }

        public async Task<MateriaXNivelDto> GetOneMateriaXNivel(MateriaXNivelDto data)
        {
            if (data is null)
            {
                throw new Exception("Debe de establecer los campos para el objeto ");
            }
            var param = _mapper.Map<Materia_X_Nivel>(data);
            var dbObject = await _unitOfWork.Set<Materia_X_Nivel>().FirstOrDefaultAsync(x => x.Id_Mat_X_Nivel == param.Id_Mat_X_Nivel);
            if (dbObject != null)
            {
                var res = _mapper.Map<MateriaXNivelDto>(dbObject);
                
                return res;
            }
            else
            {
                throw new Exception("No se encontro el Parámetro");
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

                var dbObject = _unitOfWork.Set<Profesor>().FirstOrDefault(x => x.Cedula == param.Cedula);
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

            var dbObject = _unitOfWork.Set<Profesor>().AsNoTracking().FirstOrDefault(x => x.Cedula == param.Cedula);
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
            var dbObject = await _unitOfWork.Set<Profesor>().FirstOrDefaultAsync(x => x.Cedula == param.Cedula);
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

        #region Profesor_X_Materia
        public async Task<Profesor_X_MateriaDto> AddUpdateProfesor_X_Materia(Profesor_X_MateriaDto data)
        {
            try
            {
                if (data is null)
                {
                    throw new Exception("Debe de establecer los campos para el objeto");
                }
                var param = _mapper.Map<Profesor_X_Materia>(data);

                var dbObject = _unitOfWork.Set<Profesor_X_Materia>().FirstOrDefault(x => x.Id_Prof_Materia == param.Id_Prof_Materia);
                if (dbObject != null)
                {
                    _unitOfWork.Set<Profesor_X_Materia>().Entry(dbObject).CurrentValues.SetValues(data);
                    _unitOfWork.Set<Profesor_X_Materia>().Update(dbObject);
                }
                else
                {
                    param.Id_Materia = 0;
                    var test1 = await _unitOfWork.Set<Profesor_X_Materia>().AddAsync(param);
                }
                var test2 = await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<Profesor_X_MateriaDto>(dbObject ?? param);
            }
            catch (Exception ex)
            {
                return new Profesor_X_MateriaDto();
            }

        }

        public async Task<Profesor_X_MateriaDto> DeleteProfesor_X_Materia(Profesor_X_MateriaDto data)
        {
            if (data is null)
            {
                throw new Exception("Debe de establecer los campos para el objeto ");
            }
            var param = _mapper.Map<Profesor_X_Materia>(data);

            var dbObject = _unitOfWork.Set<Profesor_X_Materia>().FirstOrDefault(x => x.Id_Prof_Materia == param.Id_Prof_Materia);
            if (dbObject != null)
            {
                _unitOfWork.Set<Profesor_X_Materia>().Entry(dbObject).CurrentValues.SetValues(data);
                _unitOfWork.Set<Profesor_X_Materia>().Remove(dbObject);
            }
            else
            {
                throw new Exception("No se encontro el Parámetro a eliminar");
            }
            await _unitOfWork.SaveChangesAsync();

            return data;
        }
        public async Task<IEnumerable<Profesor_X_MateriaDto>> GetAllProfesor_X_Materia()
        {
            var parameters = await _unitOfWork.Set<Profesor_X_Materia>()
                         .Select(s => _mapper.Map<Profesor_X_MateriaDto>(s)).ToListAsync();
            return parameters;
        }
        public async Task<Profesor_X_MateriaDto> GetOneProfesor_X_Materias(Profesor_X_MateriaDto data)
        {
            if (data is null)
            {
                throw new Exception("Debe de establecer los campos para el objeto ");
            }

            var param = _mapper.Map<Profesor_X_Materia>(data);
            var dbObject = await _unitOfWork.Set<Profesor_X_Materia>().FirstOrDefaultAsync(x => x.Id_Prof_Materia == param.Id_Prof_Materia);
            if (dbObject != null)
            {
                var res = _mapper.Map<Profesor_X_MateriaDto>(dbObject);
                return res;
            }
            else
            {
                throw new Exception("No se encontro el Parámetro");
            }

        }
        #endregion

        #region Restriccion_Profesor

        public async Task<Restriccion_ProfesorDto?> AddUpdateRestriccion_Profesor(Restriccion_ProfesorDto data)
        {
            try
            {
                if (data is null)
                {
                    throw new Exception("Debe de establecer los campos para el objeto ");
                }
                
                var param = _mapper.Map<Restriccion_Profesor>(data);

                var dbObject = _unitOfWork.Set<Restriccion_Profesor>().FirstOrDefault(x => x.Id_Restriccion == param.Id_Restriccion);
                if (dbObject != null)
                {
                    _unitOfWork.Set<Restriccion_Profesor>().Entry(dbObject).CurrentValues.SetValues(data);
                    _unitOfWork.Set<Restriccion_Profesor>().Update(dbObject);
                }
                else
                {
                    await _unitOfWork.Set<Restriccion_Profesor>().AddAsync(param);
                }
                await _unitOfWork.SaveChangesAsync();

                return data;
            }
            catch (Exception ex)
            {
                return new Restriccion_ProfesorDto();
            }
        }

        public async Task<Restriccion_ProfesorDto> DeleteRestriccion_Profesor(Restriccion_ProfesorDto data)
        {
            
            var param = _mapper.Map<Restriccion_Profesor>(data);

            var dbObject = _unitOfWork.Set<Restriccion_Profesor>().AsNoTracking().FirstOrDefault(x => x.Id_Restriccion == param.Id_Restriccion);
            if (dbObject != null)
            {
                _unitOfWork.Set<Restriccion_Profesor>().Entry(dbObject).CurrentValues.SetValues(data);
                dbObject.Estado = "I";
                _unitOfWork.Set<Restriccion_Profesor>().Update(dbObject);
            }
            else
            {
                throw new Exception("No se encontro el Parámetro a eliminar");
            }
            await _unitOfWork.SaveChangesAsync();
            return data;
        }

        

        

        public async Task<IEnumerable<Restriccion_ProfesorDto>> GetAllRestriccion_Profesor()
        {
            var parameters = await _unitOfWork.Set<Restriccion_Profesor>()
                         .Select(s => _mapper.Map<Restriccion_ProfesorDto>(s)).ToListAsync();
            return parameters;
        }

        public async Task<Restriccion_ProfesorDto> GetOneRestriccion_Profesor(Restriccion_ProfesorDto data)
        {
            if (data is null)
            {
                throw new Exception("Debe de establecer los campos para el objeto ");
            }
            var param = _mapper.Map<Restriccion_Profesor>(data);
            var dbObject = await _unitOfWork.Set<Restriccion_Profesor>().FirstOrDefaultAsync(x => x.Id_Restriccion == param.Id_Restriccion);
            if (dbObject != null)
            {
                var res = _mapper.Map<Restriccion_ProfesorDto>(dbObject);
                return res;
            }
            else
            {
                throw new Exception("No se encontro el Parámetro");
            }
        }



        #endregion

        #region Seguridad
        public async Task<SeguridadDto?> AddUpdateSeguridad(SeguridadDto data)
        {
            try
            {
                if (data is null)
                    throw new Exception("Debe establecer los campos para el objeto");

                var param = _mapper.Map<Seguridad>(data);

                var dbObject = _unitOfWork.Set<Seguridad>().FirstOrDefault(x => x.Id_Usuario == param.Id_Usuario);
                if (dbObject != null)
                {
                    _unitOfWork.Set<Seguridad>().Entry(dbObject).CurrentValues.SetValues(data);
                    _unitOfWork.Set<Seguridad>().Update(dbObject);
                }
                else
                {
                    param.Id_Usuario = 0;
                    await _unitOfWork.Set<Seguridad>().AddAsync(param);
                }

                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<SeguridadDto>(dbObject ?? param);
            }
            catch (Exception)
            {
                return new SeguridadDto();
            }
        }

        public async Task<SeguridadDto> EnableDisable(SeguridadDto data)
        {
            if (data is null)
            {
                throw new Exception("Debe de establecer los campos para el objeto ");
            }
            var param = _mapper.Map<Seguridad>(data);

            var dbObject = _unitOfWork.Set<Seguridad>().AsNoTracking().FirstOrDefault(x => x.Id_Usuario == param.Id_Usuario);
            if (dbObject != null)
            {
                _unitOfWork.Set<Seguridad>().Entry(dbObject).CurrentValues.SetValues(data);
                if (dbObject.Estado == "A")
                    dbObject.Estado = "I";
                else 
                    dbObject.Estado = "A";

                    _unitOfWork.Set<Seguridad>().Update(dbObject);
            }
            else
            {
                throw new Exception("No se encontro el Parámetro a cambiar estado");
            }
            await _unitOfWork.SaveChangesAsync();

            return data;
        }

        public async Task<IEnumerable<SeguridadDto>> GetAllSeguridad()
        {
            var parameters = await _unitOfWork.Set<Seguridad>()
                         .Select(s => _mapper.Map<SeguridadDto>(s)).ToListAsync();
            return parameters;
        }

        public async Task<SeguridadDto> GetOneSeguridad(SeguridadDto data)
        {
            if (data is null)
            {
                throw new Exception("Debe de establecer los campos para el objeto ");
            }

            var param = _mapper.Map<Seguridad>(data);
            var dbObject = await _unitOfWork.Set<Seguridad>().FirstOrDefaultAsync(x => x.Id_Usuario == param.Id_Usuario);
            if (dbObject != null)
            {
                var res = _mapper.Map<SeguridadDto>(dbObject);
                return res;
            }
            else
            {
                throw new Exception("No se encontro el Parámetro");
            }

        }
        #endregion

        #region Grupos
        public async Task<IEnumerable<GrupoDto>> GetAllGrupos()
        {
            var parameters = await _unitOfWork.Set<Grupo>()
                         .Select(s => _mapper.Map<GrupoDto>(s)).ToListAsync();
            return parameters;
        }

        #endregion

        #endregion

    }
}
