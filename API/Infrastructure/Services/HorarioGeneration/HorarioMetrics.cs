using Infrastructure.DTOs.Horarios;
using Infrastructure.DTOs.Mantenimientos;

namespace API.Infrastructure.Services.HorarioGeneration
{
    /// <summary>
    /// Analizador de métricas para evaluar la calidad de los horarios generados
    /// </summary>
    public class HorarioMetrics
    {
        public class ResultadoGeneracion
        {
            public bool Exitoso { get; set; }
            public List<string> Errores { get; set; } = new();
            public List<string> Advertencias { get; set; } = new();
            public Dictionary<string, object> Metricas { get; set; } = new();
        }

        public static ResultadoGeneracion AnalizarHorarios(
            List<HorarioDto> horarios,
            List<GrupoDto> grupos,
            List<ProfesorDto> profesores,
            List<MateriaXNivelDto> materiasXNivel)
        {
            var resultado = new ResultadoGeneracion { Exitoso = true };

            // Validar carga horaria de profesores
            ValidarCargaHorariaProfesores(horarios, profesores, resultado);

            // Validar carga horaria de materias
            ValidarCargaHorariaMaterias(horarios, grupos, materiasXNivel, resultado);

            // Calcular huecos en horarios de estudiantes
            CalcularHuecosEstudiantes(horarios, grupos, resultado);

            // Calcular huecos en horarios de profesores
            CalcularHuecosProfesores(horarios, profesores, resultado);

            // Calcular distribución de materias
            CalcularDistribucionMaterias(horarios, resultado);

            // Verificar bloques consecutivos excesivos
            VerificarBloquesConsecutivos(horarios, resultado);

            return resultado;
        }

        private static void ValidarCargaHorariaProfesores(
            List<HorarioDto> horarios,
            List<ProfesorDto> profesores,
            ResultadoGeneracion resultado)
        {
            var leccionesPorProfesor = horarios
                .Where(h => h.Estado == "A" && h.Id_Profesor != null)
                .GroupBy(h => h.Id_Profesor)
                .ToDictionary(g => g.Key, g => g.Count());

            int profesoresConCargaIncorrecta = 0;
            int profesoresConSubcarga = 0;
            int profesoresConSobrecarga = 0;
            int leccionesSinProfesor = horarios.Count(h => h.Id_Profesor == null);

            foreach (var profesor in profesores.Where(p => p.Estado == "A"))
            {
                var leccionesAsignadas = leccionesPorProfesor.ContainsKey(profesor.Id_Profesor.ToString())
                    ? leccionesPorProfesor[profesor.Id_Profesor.ToString()]
                    : 0;

                if (leccionesAsignadas != profesor.Carga_Horaria)
                {
                    profesoresConCargaIncorrecta++;

                    if (leccionesAsignadas < profesor.Carga_Horaria)
                    {
                        profesoresConSubcarga++;
                        resultado.Advertencias.Add(
                            $"Profesor {profesor.Nombre} {profesor.Apellido}: " +
                            $"{leccionesAsignadas}/{profesor.Carga_Horaria} lecciones " +
                            $"(Faltantes: {profesor.Carga_Horaria - leccionesAsignadas})");
                    }
                    else
                    {
                        profesoresConSobrecarga++;
                        resultado.Errores.Add(
                            $"Profesor {profesor.Nombre} {profesor.Apellido}: " +
                            $"{leccionesAsignadas}/{profesor.Carga_Horaria} lecciones " +
                            $"(Exceso: {leccionesAsignadas - profesor.Carga_Horaria})");
                        resultado.Exitoso = false;
                    }
                }
            }

            resultado.Metricas["ProfesoresConCargaIncorrecta"] = profesoresConCargaIncorrecta;
            resultado.Metricas["ProfesoresConSubcarga"] = profesoresConSubcarga;
            resultado.Metricas["ProfesoresConSobrecarga"] = profesoresConSobrecarga;
            resultado.Metricas["TotalProfesores"] = profesores.Count(p => p.Estado == "A");
            resultado.Metricas["LeccionesSinProfesorAsignado"] = leccionesSinProfesor;

            if (leccionesSinProfesor > 0)
            {
                resultado.Advertencias.Add($"{leccionesSinProfesor} lecciones están sin profesor asignado (requieren asignación manual)");
            }
        }

        private static void ValidarCargaHorariaMaterias(
            List<HorarioDto> horarios,
            List<GrupoDto> grupos,
            List<MateriaXNivelDto> materiasXNivel,
            ResultadoGeneracion resultado)
        {
            int materiasConCargaIncorrecta = 0;
            int materiasConSubcarga = 0;

            foreach (var grupo in grupos.Where(g => g.Estado == "A"))
            {
                var materiasDelNivel = materiasXNivel
                    .Where(m => m.Id_Nivel_Academico == grupo.Id_Nivel_Academico && m.Estado == "A")
                    .ToList();

                foreach (var materia in materiasDelNivel)
                {
                    var leccionesAsignadas = horarios.Count(h =>
                        h.Id_Grupo == grupo.Id_Grupo &&
                        h.Id_Materia == materia.Id_Materia);

                    if (leccionesAsignadas != materia.Carga_Horaria)
                    {
                        materiasConCargaIncorrecta++;

                        if (leccionesAsignadas < (materia.Carga_Horaria ?? 0))
                        {
                            materiasConSubcarga++;
                            resultado.Advertencias.Add(
                                $"Grupo {grupo.Nombre} - Materia {materia.Desc_Materia}: " +
                                $"{leccionesAsignadas}/{materia.Carga_Horaria} lecciones " +
                                $"(Faltantes: {materia.Carga_Horaria - leccionesAsignadas})");
                        }
                        else
                        {
                            resultado.Errores.Add(
                                $"Grupo {grupo.Nombre} - Materia {materia.Desc_Materia}: " +
                                $"{leccionesAsignadas}/{materia.Carga_Horaria} lecciones " +
                                $"(Exceso: {leccionesAsignadas - materia.Carga_Horaria})");
                            resultado.Exitoso = false;
                        }
                    }
                }
            }

            resultado.Metricas["MateriasConCargaIncorrecta"] = materiasConCargaIncorrecta;
            resultado.Metricas["MateriasConSubcarga"] = materiasConSubcarga;
        }

        private static void CalcularHuecosEstudiantes(
            List<HorarioDto> horarios,
            List<GrupoDto> grupos,
            ResultadoGeneracion resultado)
        {
            var diasSemana = new[] { "L", "K", "M", "J", "V" };
            int totalHuecos = 0;
            int gruposConHuecos = 0;
            var detalleHuecos = new Dictionary<string, int>();

            foreach (var grupo in grupos.Where(g => g.Estado == "A"))
            {
                int huecosGrupo = 0;

                foreach (var dia in diasSemana)
                {
                    var horariosDelDia = horarios
                        .Where(h => h.Id_Grupo == grupo.Id_Grupo && h.Dia == dia)
                        .OrderBy(h => h.Hora_Inicio)
                        .ToList();

                    if (horariosDelDia.Count <= 1) continue;

                    for (int i = 0; i < horariosDelDia.Count - 1; i++)
                    {
                        var actual = horariosDelDia[i];
                        var siguiente = horariosDelDia[i + 1];

                        if (actual.Hora_Fin != siguiente.Hora_Inicio)
                        {
                            huecosGrupo++;
                            totalHuecos++;
                            
                            // ESTO ES UN ERROR CRÍTICO - Los estudiantes NUNCA deben tener huecos
                            resultado.Errores.Add(
                                $"❌ CRÍTICO - Grupo {grupo.Nombre} ({dia}): Hueco entre " +
                                $"{actual.Hora_Fin:hh\\:mm} y {siguiente.Hora_Inicio:hh\\:mm}");
                            resultado.Exitoso = false;
                        }
                    }
                }

                if (huecosGrupo > 0)
                {
                    gruposConHuecos++;
                    detalleHuecos[grupo.Nombre ?? $"Grupo {grupo.Id_Grupo}"] = huecosGrupo;
                }
            }

            resultado.Metricas["TotalHuecosEstudiantes"] = totalHuecos;
            resultado.Metricas["GruposConHuecos"] = gruposConHuecos;
            resultado.Metricas["DetalleHuecosEstudiantes"] = detalleHuecos;

            if (totalHuecos > 0)
            {
                Console.WriteLine($"❌ CRÍTICO: {totalHuecos} huecos detectados en horarios de estudiantes (ESTO NO DEBE OCURRIR)");
                Console.WriteLine($"   Grupos afectados: {string.Join(", ", detalleHuecos.Keys)}");
            }
            else
            {
                Console.WriteLine($"✅ Todos los grupos tienen horarios compactos sin huecos");
            }
        }

        private static void CalcularHuecosProfesores(
            List<HorarioDto> horarios,
            List<ProfesorDto> profesores,
            ResultadoGeneracion resultado)
        {
            var diasSemana = new[] { "L", "K", "M", "J", "V" };
            int totalHuecos = 0;
            int profesoresConHuecos = 0;
            var detalleHuecos = new Dictionary<string, int>();

            foreach (var profesor in profesores.Where(p => p.Estado == "A"))
            {
                int huecosProfesor = 0;

                foreach (var dia in diasSemana)
                {
                    var horariosDelDia = horarios
                        .Where(h => h.Id_Profesor == profesor.Id_Profesor.ToString() && h.Dia == dia)
                        .OrderBy(h => h.Hora_Inicio)
                        .ToList();

                    if (horariosDelDia.Count <= 1) continue;

                    for (int i = 0; i < horariosDelDia.Count - 1; i++)
                    {
                        var actual = horariosDelDia[i];
                        var siguiente = horariosDelDia[i + 1];

                        if (actual.Hora_Fin != siguiente.Hora_Inicio)
                        {
                            huecosProfesor++;
                            totalHuecos++;
                        }
                    }
                }

                if (huecosProfesor > 0)
                {
                    profesoresConHuecos++;
                    detalleHuecos[$"{profesor.Nombre} {profesor.Apellido}"] = huecosProfesor;
                }
            }

            resultado.Metricas["TotalHuecosProfesores"] = totalHuecos;
            resultado.Metricas["ProfesoresConHuecos"] = profesoresConHuecos;
            resultado.Metricas["DetalleHuecosProfesores"] = detalleHuecos;

            if (totalHuecos > 0)
            {
                resultado.Advertencias.Add($"Se detectaron {totalHuecos} huecos en horarios de profesores");
            }
        }

        private static void CalcularDistribucionMaterias(
            List<HorarioDto> horarios,
            ResultadoGeneracion resultado)
        {
            var distribuccion = horarios
                .GroupBy(h => new { h.Id_Grupo, h.Id_Materia })
                .Select(g => new
                {
                    g.Key.Id_Grupo,
                    g.Key.Id_Materia,
                    Lecciones = g.Count(),
                    Dias = g.Select(h => h.Dia).Distinct().Count()
                })
                .ToList();

            var materiasConcentradas = distribuccion.Count(d => d.Dias == 1 && d.Lecciones > 2);

            resultado.Metricas["MateriasConcentradasEnUnDia"] = materiasConcentradas;

            if (materiasConcentradas > 0)
            {
                resultado.Advertencias.Add(
                    $"{materiasConcentradas} materias tienen todas sus lecciones concentradas en un solo día");
            }
        }

        private static void VerificarBloquesConsecutivos(
            List<HorarioDto> horarios,
            ResultadoGeneracion resultado)
        {
            var diasSemana = new[] { "L", "K", "M", "J", "V" };
            int bloquesExcesivos = 0;

            var porGrupoDiaTodos = horarios
                .GroupBy(h => new { h.Id_Grupo, h.Dia, h.Id_Materia });

            foreach (var grupo in porGrupoDiaTodos)
            {
                var ordenados = grupo.OrderBy(h => h.Hora_Inicio).ToList();
                int consecutivos = 1;
                TimeSpan? horaFinAnterior = null;

                foreach (var horario in ordenados)
                {
                    if (horaFinAnterior == null)
                    {
                        horaFinAnterior = horario.Hora_Fin;
                    }
                    else if (horario.Hora_Inicio == horaFinAnterior)
                    {
                        consecutivos++;
                        horaFinAnterior = horario.Hora_Fin;

                        if (consecutivos > 3)
                        {
                            bloquesExcesivos++;
                            break;
                        }
                    }
                    else
                    {
                        consecutivos = 1;
                        horaFinAnterior = horario.Hora_Fin;
                    }
                }
            }

            resultado.Metricas["BloquesConsecutivosExcesivos"] = bloquesExcesivos;

            if (bloquesExcesivos > 0)
            {
                resultado.Advertencias.Add(
                    $"{bloquesExcesivos} materias tienen más de 3 lecciones consecutivas");
            }
        }
    }
}
