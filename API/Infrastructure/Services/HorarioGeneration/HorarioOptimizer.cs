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
        /// Los estudiantes NUNCA deben tener huecos entre lecciones (exceptuando recreos)
        /// </summary>
        private static void CompactarHorarioGrupo(List<HorarioDto> horarios, int idGrupo)
        {
            // Definir períodos de recreo
            var periodosRecreo = new List<(TimeSpan inicio, TimeSpan fin)>
            {
                (new TimeSpan(9, 0, 0), new TimeSpan(9, 20, 0)),   // 1er Recreo 
                (new TimeSpan(11, 20, 0), new TimeSpan(12, 0, 0)), // Almuerzo
                (new TimeSpan(14, 0, 0), new TimeSpan(14, 20, 0))  // 2ndo Recreo 
            };

            foreach (var dia in Dias)
            {
                var horariosDelDia = horarios
                    .Where(h => h.Id_Grupo == idGrupo && h.Dia == dia)
                    .OrderBy(h => h.Hora_Inicio)
                    .ToList();

                if (horariosDelDia.Count == 0) continue;

                Console.WriteLine($"Compactando Grupo {idGrupo} - {dia}: {horariosDelDia.Count} lecciones");
                
                // DEBUG: Mostrar horarios ANTES de compactar
                Console.WriteLine($"   ANTES de compactar:");
                for (int i = 0; i < horariosDelDia.Count; i++)
                {
                    var h = horariosDelDia[i];
                    Console.WriteLine($"      [{i}] {h.Hora_Inicio:hh\\:mm}-{h.Hora_Fin:hh\\:mm} (Materia: {h.Id_Materia})");
                }

                // ESTRATEGIA: Compactar respetando los períodos de recreo
                // Asignar bloques consecutivos, saltando automáticamente los recreos
                TimeSpan? horaFinAnterior = null;
                int bloqueIndex = 0;

                foreach (var horario in horariosDelDia)
                {
                    if (bloqueIndex >= Bloques.Count)
                    {
                        Console.WriteLine($"ADVERTENCIA: Grupo {idGrupo} tiene más lecciones de las que caben en {dia}");
                        break;
                    }

                    // Si es la primera lección del día, empezar desde el primer bloque
                    if (horaFinAnterior == null)
                    {
                        var bloque = Bloques[bloqueIndex];
                        Console.WriteLine($"  Primera lección: {bloque.inicio:hh\\:mm}-{bloque.fin:hh\\:mm} (Materia: {horario.Id_Materia})");
                        horario.Hora_Inicio = bloque.inicio;
                        horario.Hora_Fin = bloque.fin;
                        horaFinAnterior = bloque.fin;
                        bloqueIndex++;
                    }
                    else
                    {
                        // Verificar si la hora fin anterior coincide con un período de recreo
                        var recreo = periodosRecreo.FirstOrDefault(r => r.inicio == horaFinAnterior);
                        
                        if (recreo != default)
                        {
                            Console.WriteLine($"  Recreo detectado: {recreo.inicio:hh\\:mm}-{recreo.fin:hh\\:mm}");
                            // Saltar el recreo y buscar el bloque que inicia después del recreo
                            bloqueIndex = Bloques.FindIndex(b => b.inicio == recreo.fin);
                            if (bloqueIndex < 0)
                            {
                                Console.WriteLine($"ERROR: No se encontró bloque después del recreo que termina a las {recreo.fin}");
                                break;
                            }
                        }

                        if (bloqueIndex >= Bloques.Count)
                        {
                            Console.WriteLine($"ADVERTENCIA: Grupo {idGrupo} tiene más lecciones de las que caben en {dia}");
                            break;
                        }

                        var bloque = Bloques[bloqueIndex];
                        
                        // VALIDACIÓN CRÍTICA: Asegurar que el nuevo bloque inicia DESPUÉS de la hora fin anterior
                        if (bloque.inicio < horaFinAnterior)
                        {
                            Console.WriteLine($"ERROR CRÍTICO: Intento de asignar bloque en orden incorrecto para Grupo {idGrupo} en {dia}");
                            Console.WriteLine($"   Hora fin anterior: {horaFinAnterior}, Bloque a asignar: {bloque.inicio}-{bloque.fin}");
                            break;
                        }

                        Console.WriteLine($"  Lección consecutiva: {bloque.inicio:hh\\:mm}-{bloque.fin:hh\\:mm} (Materia: {horario.Id_Materia})");
                        horario.Hora_Inicio = bloque.inicio;
                        horario.Hora_Fin = bloque.fin;
                        horaFinAnterior = bloque.fin;
                        bloqueIndex++;
                    }
                }

                // Verificar que no queden huecos no permitidos
                for (int i = 0; i < horariosDelDia.Count - 1; i++)
                {
                    var actual = horariosDelDia[i];
                    var siguiente = horariosDelDia[i + 1];
                    
                    if (actual.Hora_Fin != siguiente.Hora_Inicio)
                    {
                        // Verificar si es un período de recreo
                        bool esRecreo = periodosRecreo.Any(recreo => 
                            actual.Hora_Fin == recreo.inicio && siguiente.Hora_Inicio == recreo.fin);
                        
                        if (!esRecreo)
                        {
                            Console.WriteLine($"ERROR: Grupo {idGrupo} tiene hueco no permitido en {dia} entre {actual.Hora_Fin} y {siguiente.Hora_Inicio}");
                        }
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
                
                // PENALIZACIÓN ADICIONAL: Si algún día tiene muy pocas lecciones (menos de 4)
                // esto hace que no valga la pena ir al colegio ese día
                var diasConPocasLecciones = leccionesPorDia.Count(l => l > 0 && l < 4);
                uniformidad -= diasConPocasLecciones * 0.3; // Penalizar fuertemente
                
                // BONIFICACIÓN: Si los días tienen carga balanceada (5-8 lecciones por día)
                var diasBalanceados = leccionesPorDia.Count(l => l >= 5 && l <= 8);
                uniformidad += diasBalanceados * 0.2;
                
                totalUniformidad += uniformidad;
                cantidadGrupos++;
            }

            return cantidadGrupos > 0 ? totalUniformidad / cantidadGrupos : 0;
        }
    }
}
