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
using API.Infrastructure.Services.HorarioGeneration;

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


        /// <summary>
        /// Genera horarios optimizados para todos los grupos utilizando el nuevo motor de generación
        /// </summary>
        public async Task<List<HorarioDto>?> GenHorario()
        {
            try
            {
                // Eliminar horarios existentes
                var delete = await _unitOfWork.Set<Horario>().ExecuteDeleteAsync();

                // Cargar datos necesarios con joins explícitos
                var materiasXNivel = await (
                    from mxn in _unitOfWork.Set<Materia_X_Nivel>()
                    join m in _unitOfWork.Set<Materia>() on mxn.Id_Materia equals m.Id_Materia into materiaJoin
                    from materia in materiaJoin.DefaultIfEmpty()
                    join n in _unitOfWork.Set<Nivel_Academico>() on mxn.Id_Nivel_Academico equals n.Id_Nivel_Academico into nivelJoin
                    from nivel in nivelJoin.DefaultIfEmpty()
                    where (mxn.Estado ?? "I").Equals("A")
                    select new MateriaXNivelDto
                    {
                        Id_Mat_X_Nivel = mxn.Id_Mat_X_Nivel,
                        Id_Materia = mxn.Id_Materia,
                        Desc_Materia = materia != null ? materia.Nombre : null,
                        Id_Nivel_Academico = mxn.Id_Nivel_Academico,
                        Desc_Nivel_Academico = nivel != null ? nivel.Nombre : null,
                        Prioridad = mxn.Prioridad,
                        Carga_Horaria = mxn.Carga_Horaria,
                        Estado = mxn.Estado
                    }
                ).ToListAsync();

                var profesores = await _unitOfWork.Set<Profesor>()
                    .Where(s => (s.Estado ?? "I").Equals("A"))
                    .Select(s => _mapper.Map<ProfesorDto>(s))
                    .ToListAsync();

                var grupos = await _unitOfWork.Set<Grupo>()
                    .Where(s => (s.Estado ?? "I").Equals("A"))
                    .Select(s => _mapper.Map<GrupoDto>(s))
                    .ToListAsync();

                var profXMateria = await _unitOfWork.Set<Profesor_X_Materia>()
                    .Where(s => (s.Estado ?? "I").Equals("A"))
                    .Select(s => _mapper.Map<Profesor_X_MateriaDto>(s))
                    .ToListAsync();

                var restricciones = await _unitOfWork.Set<Restriccion_Profesor>()
                    .Where(s => (s.Estado ?? "I").Equals("A"))
                    .Select(s => _mapper.Map<Restriccion_ProfesorDto>(s))
                    .ToListAsync();

                // Validaciones iniciales
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

                // Obtener ID de materia GUIA
                var materiaGuia = await _unitOfWork.Set<Materia>()
                    .FirstOrDefaultAsync(m => (m.Nombre ?? "").Contains("GUIA") && m.Estado == "A");
                int idMateriaGuia = materiaGuia?.Id_Materia ?? 0;

                // Crear generador de horarios con el nuevo algoritmo
                var generator = new HorarioGenerator(
                    restricciones,
                    profesores,
                    grupos,
                    materiasXNivel,
                    profXMateria,
                    idMateriaGuia
                );

                // Generar horarios optimizados
                var horariosGenerados = generator.GenerarHorarios();

                if (horariosGenerados is null || horariosGenerados.Count == 0)
                {
                    throw new Exception("No se pudieron generar horarios con los parámetros actuales.");
                }

                // Analizar calidad de los horarios generados
                var metricas = HorarioMetrics.AnalizarHorarios(
                    horariosGenerados,
                    grupos,
                    profesores,
                    materiasXNivel
                );

                // Registrar advertencias y errores en consola/logs
                if (metricas.Advertencias.Any())
                {
                    Console.WriteLine("=== ADVERTENCIAS EN GENERACIÓN DE HORARIOS ===");
                    foreach (var advertencia in metricas.Advertencias)
                    {
                        Console.WriteLine($"⚠️  {advertencia}");
                    }
                }

                if (metricas.Errores.Any())
                {
                    Console.WriteLine("=== ERRORES EN GENERACIÓN DE HORARIOS ===");
                    foreach (var error in metricas.Errores)
                    {
                        Console.WriteLine($"❌ {error}");
                    }
                }

                Console.WriteLine("\n=== MÉTRICAS DE HORARIOS GENERADOS ===");
                foreach (var metrica in metricas.Metricas)
                {
                    Console.WriteLine($"📊 {metrica.Key}: {metrica.Value}");
                }

                // Guardar en base de datos
                var horariosDB = _mapper.Map<List<Horario>>(horariosGenerados);
                await _unitOfWork.Set<Horario>().AddRangeAsync(horariosDB);
                await _unitOfWork.SaveChangesAsync();

                return horariosGenerados;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar el horario: " + ex.Message, ex);
            }
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
