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

        private static readonly string[] Dias = ["Lunes", "Martes", "Miércoles", "Jueves", "Viernes"];
        private static readonly List<(TimeSpan inicio, TimeSpan fin)> Bloques =
        [
            (new(7, 0, 0), new(7, 40, 0)),
            (new(7, 40, 0), new(8, 20, 0)),
            (new(8, 20, 0), new(9, 0, 0)),
            (new(9, 20, 0), new(10, 0, 0)),
            (new(10, 0, 0), new(10, 40, 0)),
            (new(10, 40, 0), new(11, 20, 0)),
            (new(12, 0, 0), new(12, 40, 0)),
            (new(12, 40, 0), new(13, 20, 0)),
            (new(13, 20, 0), new(14, 0, 0)),
            (new(14, 20, 0), new(15, 0, 0)),
            (new(15, 0, 0), new(15, 40, 0)),
            (new(15, 40, 0), new(16, 20, 0)),
        ];

        #region Materias

        public async Task<List<HorarioDto>?> GenHorario(int grupoId)
        {
            try
            {

                var materiasXNivel = await _unitOfWork.Set<Materia_X_Nivel>().Select(s => _mapper.Map<MateriaXNivelDto>(s)).ToListAsync();
                var profesores = await _unitOfWork.Set<Profesor>().Select(s => _mapper.Map<ProfesorDto>(s)).ToListAsync();
                var grupos = await _unitOfWork.Set<Grupo>().Select(s => _mapper.Map<GrupoDto>(s)).ToListAsync();
                var profXMateria = await _unitOfWork.Set<Profesor_X_Materia>().Select(s => _mapper.Map<Profesor_X_MateriaDto>(s)).ToListAsync();
                var restricciones = await _unitOfWork.Set<Restriccion_Profesor>().Select(s => _mapper.Map<Restriccion_ProfesorDto>(s)).ToListAsync();

                var horariosGenerados = new List<HorarioDto>();

                foreach (var grupo in grupos)
                {
                    var nivel = grupo.Id_Nivel_Academico;
                    var materiasDelNivel = materiasXNivel.Where(m => m.Id_Nivel_Academico == nivel).ToList();

                    foreach (var mat in materiasDelNivel)
                    {
                        var profe = profXMateria.FirstOrDefault(x => x.Id_Materia == mat.Id_Materia)?.Id_Profesor;
                        if (profe == null) continue;

                        var leccionesRestantes = mat.Carga_Horaria;

                        while (leccionesRestantes >= 3)
                        {
                            var bloque = BuscarBloqueDisponible(restricciones, horariosGenerados, grupo.Id_Grupo, profe.Value, mat.Id_Materia ?? 0, 3);
                            if (bloque != null)
                            {
                                horariosGenerados.AddRange(bloque);
                                leccionesRestantes -= 3;
                            }
                            else break;
                        }

                        while (leccionesRestantes > 0)
                        {
                            var bloque = BuscarBloqueDisponible(restricciones, horariosGenerados, grupo.Id_Grupo, profe.Value, mat.Id_Materia ?? 0, 1);
                            if (bloque != null)
                            {
                                horariosGenerados.AddRange(bloque);
                                leccionesRestantes -= 1;
                            }
                            else break;
                        }
                    }
                }


                //si los horarios Generados estan vacios, retornar null
                if (horariosGenerados is null)
                {
                    throw new Exception("Debe de establecer los campos para el objeto");
                }
                //mapea los horarios generados a la entidad Horario
                var horariosDB = _mapper.Map<List<Horario>>(horariosGenerados);
                var delete = await _unitOfWork.Set<Horario>().ExecuteDeleteAsync(); // Elimina todos los horarios existentes
                await _unitOfWork.Set<Horario>().AddRangeAsync(horariosDB); // Agrega los nuevos horarios generados
                await _unitOfWork.SaveChangesAsync();// Guarda los cambios en la base de datos

                return horariosGenerados; // Retorna la lista de horarios generados al frontend
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar el horario", ex);
            }


        }
        private static List<HorarioDto>? BuscarBloqueDisponible(List<Restriccion_ProfesorDto> restricciones, List<HorarioDto> existentes, int idGrupo, int idProfesor, int idMateria, int cantidad)
        {
            foreach (var dia in Dias)
            {
                for (int i = 0; i <= Bloques.Count - cantidad; i++)
                {
                    var rango = Bloques.Skip(i).Take(cantidad).ToList();
                    var colisionGrupo = existentes.Any(x => x.Id_Grupo == idGrupo && x.Dia == dia[0].ToString() && rango.Any(b => b.inicio == x.Hora_Inicio));
                    var colisionProfe = existentes.Any(x => x.Id_Profesor == idProfesor.ToString() && x.Dia == dia[0].ToString() && rango.Any(b => b.inicio == x.Hora_Inicio));

                    var restriccion = restricciones.Any(r => r.Id_Profesor == idProfesor 
                                                        && r.Dia == dia[0].ToString() 
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
            }
            return null;
        }



        #endregion





    }
}
