using Application.Contexts;
using AutoMapper;
using Azure.Core;
using Domain.Entities;
using Infrastructure.DTOs;
using Infrastructure.DTOs.Horarios;
using Infrastructure.DTOs.Mantenimientos;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Infrastructure.Services
{
    public class HorariosRepository : IHorariosRepository
    {
        public readonly AppDbContext _unitOfWork;
        public readonly IMapper _mapper;

        public HorariosRepository(IMapper mapper, AppDbContext unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        private static readonly string[] Dias = ["L", "K", "M", "J", "V"];
        private static readonly List<(TimeSpan inicio, TimeSpan fin)> Bloques =
        [
            (new(7, 0, 0), new(7, 40, 0)),
            (new(7, 40, 0), new(8, 20, 0)),
            (new(8, 20, 0), new(9, 0, 0)),  
            // Excluir horario de recreo 9:00am - 9:20am  
            (new(9, 20, 0), new(10, 0, 0)),
            (new(10, 0, 0), new(10, 40, 0)),
            (new(10, 40, 0), new(11, 20, 0)),  
            // Excluir horario de almuerzo 11:20am - 12:00pm  
            (new(12, 0, 0), new(12, 40, 0)),
            (new(12, 40, 0), new(13, 20, 0)),
            (new(13, 20, 0), new(14, 0, 0)),  
            // Excluir horario de recreo 2:00pm - 2:20pm  
            (new(14, 20, 0), new(15, 0, 0)),
            (new(15, 0, 0), new(15, 40, 0)),
            (new(15, 40, 0), new(16, 20, 0)),
        ];

        public async Task<List<HorarioDto>?> GenHorario(int grupoId)
        {
            try
            {
                var delete = await _unitOfWork.Set<Horario>().ExecuteDeleteAsync();

                var materiasXNivel = await _unitOfWork.Set<Materia_X_Nivel>()
                    .Where(s => s.Estado == "A")
                    .Select(s => _mapper.Map<MateriaXNivelDto>(s))
                    .ToListAsync();
                var profesores = await _unitOfWork.Set<Profesor>()
                    .Where(s => s.Estado == "A")
                    .Select(s => _mapper.Map<ProfesorDto>(s))
                    .ToListAsync();
                var grupos = await _unitOfWork.Set<Grupo>()
                    .Where(s => s.Estado == "A")
                    .Select(s => _mapper.Map<GrupoDto>(s))
                    .ToListAsync();
                var profXMateria = await _unitOfWork.Set<Profesor_X_Materia>()
                    .Where(s => s.Estado == "A")
                    .Select(s => _mapper.Map<Profesor_X_MateriaDto>(s))
                    .ToListAsync();
                var restricciones = await _unitOfWork.Set<Restriccion_Profesor>()
                    .Where(s => s.Estado == "A")
                    .Select(s => _mapper.Map<Restriccion_ProfesorDto>(s))
                    .ToListAsync();

                var horariosGenerados = new List<HorarioDto>();

                if (grupos is null || grupos.Count == 0)
                {
                    throw new Exception("No hay grupos disponibles para generar horarios.");
                }

                if (materiasXNivel is null || materiasXNivel.Count == 0)
                {
                    throw new Exception("No hay materias disponibles para generar horarios.");
                }

                if (profesores is null || profesores.Count == 0)
                {
                    throw new Exception("No hay profesores disponibles para generar horarios.");
                }

                if (profXMateria is null || profXMateria.Count == 0)
                {
                    throw new Exception("No hay asignaciones de profesores a materias disponibles.");
                }


                foreach (var grupo in grupos)
                {
                    var nivel = grupo.Id_Nivel_Academico;
                    var materiasDelNivel = materiasXNivel.Where(m => m.Id_Nivel_Academico == nivel).ToList();

                    foreach (var mat in materiasDelNivel)
                    {
                        var profe = profXMateria.FirstOrDefault(x => x.Id_Materia == mat.Id_Materia)?.Id_Profesor;
                        if (profe == null) continue;

                        var leccionesRestantes = mat.Carga_Horaria;
                        var diasDisponibles = Dias.ToList();

                        while (leccionesRestantes >= 3 && diasDisponibles.Any())
                        {
                            var diaSeleccionado = diasDisponibles.First();
                            diasDisponibles.Remove(diaSeleccionado);

                            var bloque = BuscarBloqueDisponible(restricciones, horariosGenerados, grupo.Id_Grupo, profe.Value, mat.Id_Materia ?? 0, 3, diaSeleccionado);
                            if (bloque != null)
                            {
                                horariosGenerados.AddRange(bloque);
                                leccionesRestantes -= 3;
                            }
                        }

                        while (leccionesRestantes > 0 && diasDisponibles.Any())
                        {
                            var diaSeleccionado = diasDisponibles.First();
                            diasDisponibles.Remove(diaSeleccionado);

                            var bloque = BuscarBloqueDisponible(restricciones, horariosGenerados, grupo.Id_Grupo, profe.Value, mat.Id_Materia ?? 0, 1, diaSeleccionado);
                            if (bloque != null)
                            {
                                horariosGenerados.AddRange(bloque);
                                leccionesRestantes -= 1;
                            }
                        }
                    }
                }

                if (horariosGenerados is null)
                {
                    throw new Exception("Debe de establecer los campos para el objeto");
                }

                var horariosDB = _mapper.Map<List<Horario>>(horariosGenerados);
               
                await _unitOfWork.Set<Horario>().AddRangeAsync(horariosDB);
                var test = await _unitOfWork.SaveChangesAsync();

                return horariosGenerados;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar el horario", ex);
            }
        }
        private static List<HorarioDto>? BuscarBloqueDisponible(List<Restriccion_ProfesorDto> restricciones, List<HorarioDto> existentes, int idGrupo, int idProfesor, int idMateria, int cantidad, string dia)
        {
            for (int i = 0; i <= Bloques.Count - cantidad; i++)
            {
                var rango = Bloques.Skip(i).Take(cantidad).ToList();
                var colisionGrupo = existentes.Any(x => x.Id_Grupo == idGrupo && x.Dia == dia && rango.Any(b => b.inicio == x.Hora_Inicio));
                var colisionProfe = existentes.Any(x => x.Id_Profesor == idProfesor.ToString() && x.Dia == dia && rango.Any(b => b.inicio == x.Hora_Inicio));

                var restriccion = restricciones.Any(r => r.Id_Profesor == idProfesor
                                                        && r.Dia == dia
                                                        && rango.Any(b => b.inicio >= TimeSpan.Parse(r.Hora_Inicio ?? "")
                                                        && b.fin <= TimeSpan.Parse(r.Hora_Fin ?? "")));

                if (!colisionGrupo && !colisionProfe && !restriccion)
                {
                    return rango.Select(b => new HorarioDto
                    {
                        Dia = dia,
                        Id_Grupo = idGrupo,
                        Id_Profesor = idProfesor.ToString(),
                        Id_Materia = idMateria,
                        Hora_Inicio = b.inicio,
                        Hora_Fin = b.fin,
                        Estado = "A"
                    }).ToList();
                }
            }
            return null;
        }

        public async Task<List<HorarioDto>?> GetOneHorario(HorarioDto data)
        {
            var param = _mapper.Map<Horario>(data);
            var dbObject = await _unitOfWork.Set<Horario>().Where(x => x.Id_Grupo == param.Id_Grupo).ToListAsync();
            if (dbObject != null)
            {
                var res = _mapper.Map<List<HorarioDto>?>(dbObject);
                return res;
            }
            else
            {
                throw new Exception("No se encontro el Parámetro");
            }
        }

       
        public async Task<HorarioDto?> UpdateHorario(HorarioDto data)
        {
            try
            {
                if (data is null)
                {
                    throw new Exception("Debe de establecer los campos para el objeto");
                }
                var param = _mapper.Map<Horario>(data);

                var dbObject = _unitOfWork.Set<Horario>().FirstOrDefault(x => x.Id_Horario == param.Id_Horario);
                if (dbObject != null)
                {
                    _unitOfWork.Set<Horario>().Entry(dbObject).CurrentValues.SetValues(data);
                    _unitOfWork.Set<Horario>().Update(dbObject);
                }
                else
                {
                    param.Id_Horario = 0;
                    var test1 = await _unitOfWork.Set<Horario>().AddAsync(param);
                }
                var test2 = await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<HorarioDto>(dbObject ?? param);
            }
            catch (Exception ex)
            {
                return new HorarioDto();
            }

        }

    }
}
