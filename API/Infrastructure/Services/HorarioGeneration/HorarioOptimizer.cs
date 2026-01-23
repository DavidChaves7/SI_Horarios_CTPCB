using Infrastructure.DTOs.Horarios;
using Infrastructure.DTOs.Mantenimientos;

namespace API.Infrastructure.Services.HorarioGeneration
{
    /// <summary>
    /// Optimizador de horarios para minimizar huecos y mejorar la distribución
    /// </summary>
    public class HorarioOptimizer
    {
        private static readonly string[] Dias = ["L", "K", "M", "J", "V"];
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

        /// <summary>
        /// Compacta los horarios de todos los grupos para minimizar huecos
        /// </summary>
        public static void CompactarHorarios(List<HorarioDto> horarios, List<GrupoDto> grupos)
        {
            foreach (var grupo in grupos.Where(g => g.Estado == "A"))
            {
                CompactarHorarioGrupo(horarios, grupo.Id_Grupo);
            }
        }

        /// <summary>
        /// Compacta el horario de un grupo específico eliminando TODOS los huecos
        /// Los estudiantes NUNCA deben tener huecos entre lecciones
        /// </summary>
        private static void CompactarHorarioGrupo(List<HorarioDto> horarios, int idGrupo)
        {
            foreach (var dia in Dias)
            {
                var horariosDelDia = horarios
                    .Where(h => h.Id_Grupo == idGrupo && h.Dia == dia)
                    .OrderBy(h => h.Hora_Inicio)
                    .ToList();

                if (horariosDelDia.Count == 0) continue;

                // IMPORTANTE: Compactar desde el primer bloque disponible
                // Los estudiantes NO pueden tener huecos
                int bloqueIndex = 0;
                foreach (var horario in horariosDelDia)
                {
                    if (bloqueIndex >= Bloques.Count)
                    {
                        Console.WriteLine($"⚠️  ADVERTENCIA: Grupo {idGrupo} tiene más lecciones de las que caben en {dia}");
                        break;
                    }

                    var bloque = Bloques[bloqueIndex];
                    horario.Hora_Inicio = bloque.inicio;
                    horario.Hora_Fin = bloque.fin;
                    bloqueIndex++;
                }

                // Verificar que no queden huecos
                for (int i = 0; i < horariosDelDia.Count - 1; i++)
                {
                    if (horariosDelDia[i].Hora_Fin != horariosDelDia[i + 1].Hora_Inicio)
                    {
                        Console.WriteLine($"❌ ERROR: Grupo {idGrupo} tiene hueco en {dia} entre {horariosDelDia[i].Hora_Fin} y {horariosDelDia[i + 1].Hora_Inicio}");
                    }
                }
            }
        }

        /// <summary>
        /// Compacta los horarios de profesores para minimizar sus huecos
        /// </summary>
        public static void CompactarHorariosProfesores(
            List<HorarioDto> horarios,
            List<ProfesorDto> profesores,
            List<Restriccion_ProfesorDto> restricciones,
            HorarioValidator validator)
        {
            foreach (var profesor in profesores.Where(p => p.Estado == "A"))
            {
                IntentarCompactarHorarioProfesor(horarios, profesor.Id_Profesor, restricciones, validator);
            }
        }

        /// <summary>
        /// Intenta compactar el horario de un profesor específico
        /// </summary>
        private static void IntentarCompactarHorarioProfesor(
            List<HorarioDto> horarios,
            int idProfesor,
            List<Restriccion_ProfesorDto> restricciones,
            HorarioValidator validator)
        {
            foreach (var dia in Dias)
            {
                var horariosDelProfesor = horarios
                    .Where(h => h.Id_Profesor == idProfesor.ToString() && h.Dia == dia)
                    .OrderBy(h => h.Hora_Inicio)
                    .ToList();

                if (horariosDelProfesor.Count <= 1) continue;

                // Verificar si hay huecos
                bool tieneHuecos = false;
                for (int i = 0; i < horariosDelProfesor.Count - 1; i++)
                {
                    if (horariosDelProfesor[i].Hora_Fin != horariosDelProfesor[i + 1].Hora_Inicio)
                    {
                        tieneHuecos = true;
                        break;
                    }
                }

                if (!tieneHuecos) continue;

                // Intentar reorganizar para eliminar huecos
                var horariosCopia = horarios.Where(h =>
                    !(h.Id_Profesor == idProfesor.ToString() && h.Dia == dia))
                    .ToList();

                List<HorarioDto> nuevosHorarios = new();
                int bloqueIndex = 0;

                foreach (var horarioOriginal in horariosDelProfesor)
                {
                    bool asignado = false;

                    // Buscar el siguiente bloque disponible
                    while (bloqueIndex < Bloques.Count && !asignado)
                    {
                        var bloque = Bloques[bloqueIndex];

                        // Verificar si el bloque está disponible para el grupo
                        if (!validator.TieneColisionGrupo(horariosCopia.Concat(nuevosHorarios).ToList(),
                                horarioOriginal.Id_Grupo, dia, bloque.inicio, bloque.fin) &&
                            !validator.TieneRestriccion(idProfesor, dia, bloque.inicio, bloque.fin))
                        {
                            var nuevoHorario = new HorarioDto
                            {
                                Id_Horario = horarioOriginal.Id_Horario,
                                Dia = dia,
                                Id_Grupo = horarioOriginal.Id_Grupo,
                                Id_Profesor = idProfesor.ToString(),
                                Id_Materia = horarioOriginal.Id_Materia,
                                Hora_Inicio = bloque.inicio,
                                Hora_Fin = bloque.fin,
                                Estado = horarioOriginal.Estado
                            };

                            nuevosHorarios.Add(nuevoHorario);
                            asignado = true;
                        }

                        bloqueIndex++;
                    }

                    if (!asignado)
                    {
                        // No se pudo compactar, mantener el horario original
                        return;
                    }
                }

                // Si se pudo compactar, actualizar los horarios
                foreach (var horarioOriginal in horariosDelProfesor)
                {
                    var nuevoHorario = nuevosHorarios.FirstOrDefault(n => n.Id_Horario == horarioOriginal.Id_Horario);
                    if (nuevoHorario != null)
                    {
                        horarioOriginal.Hora_Inicio = nuevoHorario.Hora_Inicio;
                        horarioOriginal.Hora_Fin = nuevoHorario.Hora_Fin;
                    }
                }
            }
        }

        /// <summary>
        /// Redistribuye materias con muchas lecciones en un solo día
        /// </summary>
        public static void RedistribuirMaterias(
            List<HorarioDto> horarios,
            List<GrupoDto> grupos,
            HorarioValidator validator,
            List<Restriccion_ProfesorDto> restricciones)
        {
            foreach (var grupo in grupos.Where(g => g.Estado == "A"))
            {
                var materiasDelGrupo = horarios
                    .Where(h => h.Id_Grupo == grupo.Id_Grupo)
                    .GroupBy(h => h.Id_Materia)
                    .Select(g => new
                    {
                        IdMateria = g.Key,
                        Lecciones = g.ToList(),
                        CantidadDias = g.Select(h => h.Dia).Distinct().Count(),
                        TotalLecciones = g.Count()
                    })
                    .Where(m => m.CantidadDias == 1 && m.TotalLecciones > 3)
                    .ToList();

                foreach (var materia in materiasDelGrupo)
                {
                    IntentarRedistribuirMateria(horarios, materia.Lecciones, grupo, validator, restricciones);
                }
            }
        }

        /// <summary>
        /// Intenta redistribuir las lecciones de una materia en múltiples días
        /// </summary>
        private static void IntentarRedistribuirMateria(
            List<HorarioDto> horarios,
            List<HorarioDto> leccionesMateria,
            GrupoDto grupo,
            HorarioValidator validator,
            List<Restriccion_ProfesorDto> restricciones)
        {
            if (leccionesMateria.Count <= 3) return;

            var diaActual = leccionesMateria.First().Dia;
            var diasDisponibles = Dias.Where(d => d != diaActual).ToList();

            // Intentar mover algunas lecciones a otros días
            int leccionesAMover = leccionesMateria.Count / 2;
            var leccionesParaMover = leccionesMateria.Take(leccionesAMover).ToList();

            foreach (var leccion in leccionesParaMover)
            {
                bool movida = false;

                foreach (var nuevoDia in diasDisponibles)
                {
                    // Buscar un bloque disponible en el nuevo día
                    foreach (var bloque in Bloques)
                    {
                        var horariosSinEstaLeccion = horarios.Where(h => h.Id_Horario != leccion.Id_Horario).ToList();

                        if (!validator.TieneColisionGrupo(horariosSinEstaLeccion, grupo.Id_Grupo, nuevoDia, bloque.inicio, bloque.fin) &&
                            !validator.TieneColisionProfesor(horariosSinEstaLeccion, int.Parse(leccion.Id_Profesor ?? "0"), nuevoDia, bloque.inicio, bloque.fin) &&
                            !validator.TieneRestriccion(int.Parse(leccion.Id_Profesor ?? "0"), nuevoDia, bloque.inicio, bloque.fin))
                        {
                            // Mover la lección
                            leccion.Dia = nuevoDia;
                            leccion.Hora_Inicio = bloque.inicio;
                            leccion.Hora_Fin = bloque.fin;
                            movida = true;
                            break;
                        }
                    }

                    if (movida) break;
                }
            }
        }

        /// <summary>
        /// Calcula una puntuación de calidad del horario (mayor es mejor)
        /// </summary>
        public static double CalcularCalidadHorario(List<HorarioDto> horarios, List<GrupoDto> grupos, List<ProfesorDto> profesores)
        {
            double puntuacion = 1000.0;

            // Penalizar por huecos en horarios de estudiantes
            int huecosEstudiantes = ContarHuecosEstudiantes(horarios, grupos);
            puntuacion -= huecosEstudiantes * 10;

            // Penalizar por huecos en horarios de profesores
            int huecosProfesores = ContarHuecosProfesores(horarios, profesores);
            puntuacion -= huecosProfesores * 5;

            // Penalizar materias concentradas en un día
            int materiasConcentradas = ContarMateriasConcentradas(horarios);
            puntuacion -= materiasConcentradas * 3;

            // Bonificar por distribución uniforme de lecciones por día
            double uniformidad = CalcularUniformidadDistribucion(horarios, grupos);
            puntuacion += uniformidad * 20;

            return Math.Max(0, puntuacion);
        }

        /// <summary>
        /// Cuenta los huecos en horarios de grupos (método público para verificación)
        /// </summary>
        public static int ContarHuecosGrupos(List<HorarioDto> horarios, List<GrupoDto> grupos)
        {
            return ContarHuecosEstudiantes(horarios, grupos);
        }

        private static int ContarHuecosEstudiantes(List<HorarioDto> horarios, List<GrupoDto> grupos)
        {
            int totalHuecos = 0;

            foreach (var grupo in grupos.Where(g => g.Estado == "A"))
            {
                foreach (var dia in Dias)
                {
                    var horariosDelDia = horarios
                        .Where(h => h.Id_Grupo == grupo.Id_Grupo && h.Dia == dia)
                        .OrderBy(h => h.Hora_Inicio)
                        .ToList();

                    for (int i = 0; i < horariosDelDia.Count - 1; i++)
                    {
                        if (horariosDelDia[i].Hora_Fin != horariosDelDia[i + 1].Hora_Inicio)
                        {
                            totalHuecos++;
                        }
                    }
                }
            }

            return totalHuecos;
        }

        private static int ContarHuecosProfesores(List<HorarioDto> horarios, List<ProfesorDto> profesores)
        {
            int totalHuecos = 0;

            foreach (var profesor in profesores.Where(p => p.Estado == "A"))
            {
                foreach (var dia in Dias)
                {
                    var horariosDelDia = horarios
                        .Where(h => h.Id_Profesor == profesor.Id_Profesor.ToString() && h.Dia == dia)
                        .OrderBy(h => h.Hora_Inicio)
                        .ToList();

                    for (int i = 0; i < horariosDelDia.Count - 1; i++)
                    {
                        if (horariosDelDia[i].Hora_Fin != horariosDelDia[i + 1].Hora_Inicio)
                        {
                            totalHuecos++;
                        }
                    }
                }
            }

            return totalHuecos;
        }

        private static int ContarMateriasConcentradas(List<HorarioDto> horarios)
        {
            return horarios
                .GroupBy(h => new { h.Id_Grupo, h.Id_Materia })
                .Count(g => g.Select(h => h.Dia).Distinct().Count() == 1 && g.Count() > 3);
        }

        private static double CalcularUniformidadDistribucion(List<HorarioDto> horarios, List<GrupoDto> grupos)
        {
            double totalUniformidad = 0;
            int cantidadGrupos = 0;

            foreach (var grupo in grupos.Where(g => g.Estado == "A"))
            {
                var leccionesPorDia = Dias
                    .Select(d => horarios.Count(h => h.Id_Grupo == grupo.Id_Grupo && h.Dia == d))
                    .ToList();

                if (leccionesPorDia.Sum() == 0) continue;

                var promedio = leccionesPorDia.Average();
                var varianza = leccionesPorDia.Sum(l => Math.Pow(l - promedio, 2)) / leccionesPorDia.Count;
                var desviacion = Math.Sqrt(varianza);

                // Menor desviación = mayor uniformidad
                var uniformidad = 1.0 / (1.0 + desviacion);
                totalUniformidad += uniformidad;
                cantidadGrupos++;
            }

            return cantidadGrupos > 0 ? totalUniformidad / cantidadGrupos : 0;
        }
    }
}
