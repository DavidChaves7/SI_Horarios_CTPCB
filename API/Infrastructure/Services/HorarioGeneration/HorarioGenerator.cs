using Infrastructure.DTOs.Horarios;
using Infrastructure.DTOs.Mantenimientos;

namespace API.Infrastructure.Services.HorarioGeneration
{
    /// <summary>
    /// Motor principal de generación de horarios con algoritmo optimizado
    /// Implementa múltiples pasadas, backtracking y validación exhaustiva de restricciones
    /// </summary>
    public class HorarioGenerator
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

        private readonly HorarioValidator _validator;
        private readonly List<Restriccion_ProfesorDto> _restricciones;
        private readonly List<ProfesorDto> _profesores;
        private readonly List<GrupoDto> _grupos;
        private readonly List<MateriaXNivelDto> _materiasXNivel;
        private readonly List<Profesor_X_MateriaDto> _profXMateria;
        private readonly int _idMateriaGuia;
        private readonly Dictionary<int, string> _tiposMaterias;

        public HorarioGenerator(
            List<Restriccion_ProfesorDto> restricciones,
            List<ProfesorDto> profesores,
            List<GrupoDto> grupos,
            List<MateriaXNivelDto> materiasXNivel,
            List<Profesor_X_MateriaDto> profXMateria,
            int idMateriaGuia,
            Dictionary<int, string> tiposMaterias)
        {
            _restricciones = restricciones;
            _profesores = profesores;
            _grupos = grupos;
            _materiasXNivel = materiasXNivel;
            _profXMateria = profXMateria;
            _idMateriaGuia = idMateriaGuia;
            _tiposMaterias = tiposMaterias;
            _validator = new HorarioValidator(restricciones, profesores);
        }

        /// <summary>
        /// Clase auxiliar para rastrear asignaciones pendientes de materias
        /// </summary>
        private class AsignacionMateria
        {
            public int IdGrupo { get; set; }
            public int IdMateria { get; set; }
            public int? IdProfesor { get; set; }
            public int LeccionesRestantes { get; set; }
            public int Prioridad { get; set; }
            public string? DescMateria { get; set; }
        }

        /// <summary>
        /// Genera horarios para todos los grupos utilizando estrategia optimizada de 5 pasadas
        /// </summary>
        public List<HorarioDto> GenerarHorarios()
        {
            var horarios = new List<HorarioDto>();

            // Paso 1: Preparar lista de asignaciones ordenadas por prioridad
            var asignaciones = PrepararAsignaciones();

            // Paso 2: Primera pasada - Asignar materias prioritarias con bloques de 3
            Console.WriteLine("===== FASE 1: Bloques Grandes =====");
            PrimeraFase_BloquesGrandes(horarios, asignaciones);

            // Paso 3: Segunda pasada - Asignar lecciones restantes
            Console.WriteLine("\n===== FASE 2: Lecciones Restantes =====");
            SegundaFase_LeccionesRestantes(horarios, asignaciones);

            // Paso 4: Tercera pasada - Asignar materia GUIA
            Console.WriteLine("\n===== FASE 3: Materia GUIA =====");
            TerceraFase_AsignarGuia(horarios);

            // Paso 5: Cuarta pasada - Asignar lecciones faltantes con profesor -1
            Console.WriteLine("\n===== FASE 4: Lecciones Sin Profesor =====");
            CuartaFase_AsignarLeccionesSinProfesor(horarios, asignaciones);

            // Paso 6: Quinta pasada - Completar cargas horarias de profesores (solo si no excede)
            Console.WriteLine("\n===== FASE 5: Completar Cargas Profesores =====");
            QuintaFase_CompletarCargasProfesores(horarios);

            // Paso 7: Optimización - Compactar AGRESIVAMENTE horarios de grupos (eliminar todos los huecos)
            Console.WriteLine("\n===== OPTIMIZACIÓN FINAL - ELIMINACIÓN DE HUECOS EN GRUPOS =====");
            HorarioOptimizer.CompactarHorarios(horarios, _grupos);
            
            // Verificar que no queden huecos en grupos
            var huecosRestantes = HorarioOptimizer.ContarHuecosGrupos(horarios, _grupos);
            if (huecosRestantes > 0)
            {
                Console.WriteLine($"⚠️  ADVERTENCIA: Aún quedan {huecosRestantes} huecos en horarios de grupos");
            }
            else
            {
                Console.WriteLine("✅ Todos los grupos tienen horarios sin huecos");
            }
            
            // Redistribuir materias solo si no afecta la continuidad de grupos
            // DESHABILITADO: Esta funcionalidad está causando colisiones y huecos
            // HorarioOptimizer.RedistribuirMaterias(horarios, _grupos, _validator, _restricciones);
            
            // Compactar profesores es MENOS prioritario (pueden tener huecos)
            // HorarioOptimizer.CompactarHorariosProfesores(horarios, _profesores, _restricciones, _validator);

            // Paso 8: Verificación final
            VerificarCompletitud(horarios);

            return horarios;
        }

        /// <summary>
        /// Prepara la lista de asignaciones (sin prioridades, solo por carga horaria)
        /// FILTRA materias técnicas según la especialidad del grupo
        /// </summary>
        private List<AsignacionMateria> PrepararAsignaciones()
        {
            var asignaciones = new List<AsignacionMateria>();

            foreach (var grupo in _grupos.Where(g => g.Estado == "A"))
            {
                var materiasDelNivel = _materiasXNivel
                    .Where(m => m.Id_Nivel_Academico == grupo.Id_Nivel_Academico && m.Estado == "A")
                    .OrderByDescending(m => m.Carga_Horaria ?? 0) // Solo ordenar por carga horaria
                    .ToList();

                foreach (var materia in materiasDelNivel)
                {
                    // FILTRO IMPORTANTE: Saltar materia GUIA aquí - se asignará en TerceraFase_AsignarGuia
                    if (materia.Id_Materia == _idMateriaGuia)
                    {
                        Console.WriteLine($"⏭️  Grupo {grupo.Nombre} - GUIA se asignará en fase específica");
                        continue;
                    }
                    
                    // FILTRO CRÍTICO: Si es materia técnica (Tipo = "T"), solo asignarla si coincide con la especialidad del grupo
                    var tipoMateria = _tiposMaterias.GetValueOrDefault(materia.Id_Materia ?? 0, "A");
                    
                    if (tipoMateria.Equals("T", StringComparison.OrdinalIgnoreCase))
                    {
                        // Si el grupo NO tiene especialidad definida, saltar esta materia técnica
                        if (!grupo.Especialidad.HasValue || grupo.Especialidad.Value <= 0)
                        {
                            Console.WriteLine($"⚠️  Grupo {grupo.Nombre} no tiene especialidad definida - se omite materia técnica {materia.Desc_Materia}");
                            continue;
                        }

                        // Si la materia técnica NO coincide con la especialidad del grupo, saltarla
                        if (materia.Id_Materia != grupo.Especialidad.Value)
                        {
                            Console.WriteLine($"⏭️  Grupo {grupo.Nombre} (Especialidad: {grupo.Especialidad}) - se omite materia técnica {materia.Desc_Materia} (ID: {materia.Id_Materia})");
                            continue;
                        }

                        Console.WriteLine($"✅ Grupo {grupo.Nombre} - asignando materia técnica {materia.Desc_Materia}");
                    }

                    // Obtener profesor asignado
                    int? idProfesor = null;

                    // Si es GUIA, asignar profesor guía del grupo
                    if (materia.Id_Materia == _idMateriaGuia && grupo.Id_Profesor_Guia != null && grupo.Id_Profesor_Guia != -1)
                    {
                        idProfesor = Convert.ToInt32(grupo.Id_Profesor_Guia);
                    }
                    else
                    {
                        var profMateria = _profXMateria.FirstOrDefault(x => x.Id_Materia == materia.Id_Materia);
                        idProfesor = profMateria?.Id_Profesor;

                        // Si no hay profesor, buscar "Sin Asignar"
                        if (idProfesor == null)
                        {
                            idProfesor = _profesores
                                .FirstOrDefault(p => p.Cedula == "000000000" || (p.Nombre ?? "").Contains("Sin Asignar"))
                                ?.Id_Profesor;
                        }
                    }

                    asignaciones.Add(new AsignacionMateria
                    {
                        IdGrupo = grupo.Id_Grupo,
                        IdMateria = materia.Id_Materia ?? 0,
                        IdProfesor = idProfesor,
                        LeccionesRestantes = materia.Carga_Horaria ?? 0,
                        Prioridad = 0, // Ya no se usa
                        DescMateria = materia.Desc_Materia
                    });
                }
            }

            return asignaciones;
        }

        /// <summary>
        /// Primera fase: Asigna bloques grandes (3 lecciones consecutivas) distribuyendo entre días
        /// </summary>
        private void PrimeraFase_BloquesGrandes(List<HorarioDto> horarios, List<AsignacionMateria> asignaciones)
        {
            // Filtrar materias que no sean GUIA y ordenar por carga horaria
            var materiasParaBloques = asignaciones
                .Where(a => a.IdMateria != _idMateriaGuia && a.LeccionesRestantes >= 3)
                .OrderByDescending(a => a.LeccionesRestantes)
                .ToList();

            foreach (var asignacion in materiasParaBloques)
            {
                if (asignacion.IdProfesor == null) continue;

                int intentos = 0;
                const int maxIntentos = 10;

                while (asignacion.LeccionesRestantes >= 3 && intentos < maxIntentos)
                {
                    // Obtener carga horaria requerida para esta materia
                    var materiaXNivel = _materiasXNivel.FirstOrDefault(m => 
                        m.Id_Materia == asignacion.IdMateria && 
                        m.Id_Nivel_Academico == _grupos.First(g => g.Id_Grupo == asignacion.IdGrupo).Id_Nivel_Academico);
                    
                    int cargaRequerida = materiaXNivel?.Carga_Horaria ?? 0;
                    
                    // VALIDACIÓN CRÍTICA: Verificar que no exceda la carga horaria de la materia
                    if (_validator.ExcedeCargaHorariaMateria(horarios, asignacion.IdGrupo, asignacion.IdMateria, cargaRequerida, 3))
                    {
                        break;
                    }

                    var bloque = BuscarMejorBloqueDisponible(
                        horarios,
                        asignacion.IdGrupo,
                        asignacion.IdProfesor.Value,
                        asignacion.IdMateria,
                        3);

                    if (bloque != null && bloque.Any())
                    {
                        horarios.AddRange(bloque);
                        asignacion.LeccionesRestantes -= 3;
                    }
                    else
                    {
                        break;
                    }

                    intentos++;
                }
            }
        }

        /// <summary>
        /// Segunda fase: Asigna lecciones restantes (bloques de 1 o 2) distribuyendo entre días
        /// </summary>
        private void SegundaFase_LeccionesRestantes(List<HorarioDto> horarios, List<AsignacionMateria> asignaciones)
        {
            var materiasConLeccionesRestantes = asignaciones
                .Where(a => a.LeccionesRestantes > 0 && a.IdMateria != _idMateriaGuia)
                .OrderByDescending(a => a.LeccionesRestantes)
                .ToList();

            foreach (var asignacion in materiasConLeccionesRestantes)
            {
                if (asignacion.IdProfesor == null) continue;

                int intentos = 0;
                const int maxIntentos = 20;

                while (asignacion.LeccionesRestantes > 0 && intentos < maxIntentos)
                {
                    // Obtener carga horaria requerida para esta materia
                    var materiaXNivel = _materiasXNivel.FirstOrDefault(m => 
                        m.Id_Materia == asignacion.IdMateria && 
                        m.Id_Nivel_Academico == _grupos.First(g => g.Id_Grupo == asignacion.IdGrupo).Id_Nivel_Academico);
                    
                    int cargaRequerida = materiaXNivel?.Carga_Horaria ?? 0;
                    
                    // Intentar asignar bloques de 2 si es posible
                    int tamañoBloque = asignacion.LeccionesRestantes >= 2 ? 2 : 1;
                    
                    // VALIDACIÓN CRÍTICA: Verificar que no exceda la carga horaria de la materia
                    if (_validator.ExcedeCargaHorariaMateria(horarios, asignacion.IdGrupo, asignacion.IdMateria, cargaRequerida, tamañoBloque))
                    {
                        break;
                    }

                    var bloque = BuscarMejorBloqueDisponible(
                        horarios,
                        asignacion.IdGrupo,
                        asignacion.IdProfesor.Value,
                        asignacion.IdMateria,
                        tamañoBloque);

                    if (bloque != null && bloque.Any())
                    {
                        horarios.AddRange(bloque);
                        asignacion.LeccionesRestantes -= tamañoBloque;
                    }
                    else if (tamañoBloque == 2)
                    {
                        // VALIDACIÓN antes de intentar con 1
                        if (_validator.ExcedeCargaHorariaMateria(horarios, asignacion.IdGrupo, asignacion.IdMateria, cargaRequerida, 1))
                        {
                            break;
                        }
                        
                        // Si no se pudo asignar bloque de 2, intentar con 1
                        bloque = BuscarMejorBloqueDisponible(
                            horarios,
                            asignacion.IdGrupo,
                            asignacion.IdProfesor.Value,
                            asignacion.IdMateria,
                            1);

                        if (bloque != null && bloque.Any())
                        {
                            horarios.AddRange(bloque);
                            asignacion.LeccionesRestantes -= 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }

                    intentos++;
                }
            }
        }

        /// <summary>
        /// Tercera fase: Asigna lecciones de GUIA para cada grupo
        /// </summary>
        private void TerceraFase_AsignarGuia(List<HorarioDto> horarios)
        {
            var asignacionesGuia = _grupos
                .Where(g => g.Estado == "A" && g.Id_Profesor_Guia != null && g.Id_Profesor_Guia != -1)
                .Select(g => new
                {
                    Grupo = g,
                    IdProfesorGuia = Convert.ToInt32(g.Id_Profesor_Guia)
                })
                .ToList();

            foreach (var asignacion in asignacionesGuia)
            {
                // VALIDACIÓN CRÍTICA: Verificar cuántas lecciones de GUIA ya tiene este grupo
                var leccionesGuiaAsignadas = horarios.Count(h =>
                    h.Id_Grupo == asignacion.Grupo.Id_Grupo &&
                    h.Id_Materia == _idMateriaGuia &&
                    h.Estado == "A");

                // Obtener carga horaria requerida de GUIA para este nivel
                var materiaGuiaXNivel = _materiasXNivel.FirstOrDefault(m => 
                    m.Id_Materia == _idMateriaGuia && 
                    m.Id_Nivel_Academico == asignacion.Grupo.Id_Nivel_Academico);
                
                int cargaGuiaRequerida = materiaGuiaXNivel?.Carga_Horaria ?? 1;

                // Si ya tiene la cantidad correcta o más, no asignar
                if (leccionesGuiaAsignadas >= cargaGuiaRequerida)
                {
                    Console.WriteLine($"Grupo {asignacion.Grupo.Nombre} ya tiene {leccionesGuiaAsignadas} lecciones de GUIA (Requerido: {cargaGuiaRequerida})");
                    continue;
                }

                // Buscar un bloque disponible para GUIA
                var bloque = BuscarMejorBloqueDisponible(
                    horarios,
                    asignacion.Grupo.Id_Grupo,
                    asignacion.IdProfesorGuia,
                    _idMateriaGuia,
                    1);

                if (bloque != null && bloque.Any())
                {
                    horarios.AddRange(bloque);
                    Console.WriteLine($"Asignada lección de GUIA al grupo {asignacion.Grupo.Nombre}");
                }
                else
                {
                    Console.WriteLine($"⚠️  No se pudo asignar GUIA al grupo {asignacion.Grupo.Nombre}");
                }
            }
        }

        /// <summary>
        /// Cuarta fase: Asigna lecciones faltantes con profesor -1 (Sin Asignar)
        /// Esto garantiza que todas las materias tengan sus lecciones completas
        /// </summary>
        private void CuartaFase_AsignarLeccionesSinProfesor(List<HorarioDto> horarios, List<AsignacionMateria> asignaciones)
        {
            // Obtener todas las materias que aún tienen lecciones faltantes
            var materiasConFaltantes = asignaciones
                .Where(a => a.LeccionesRestantes > 0)
                .OrderByDescending(a => a.LeccionesRestantes)
                .ToList();

            foreach (var asignacion in materiasConFaltantes)
            {
                int intentos = 0;
                const int maxIntentos = 50;

                while (asignacion.LeccionesRestantes > 0 && intentos < maxIntentos)
                {
                    // Intentar asignar con profesor -1 (Sin Asignar)
                    var bloque = BuscarBloqueDisponibleParaGrupo(
                        horarios,
                        asignacion.IdGrupo,
                        asignacion.IdMateria,
                        1);

                    if (bloque != null && bloque.Any())
                    {
                        // Asignar con profesor null (Sin Asignar)
                        foreach (var h in bloque)
                        {
                            h.Id_Profesor = null;
                        }
                        horarios.AddRange(bloque);
                        asignacion.LeccionesRestantes -= bloque.Count;

                        Console.WriteLine($"Asignado sin profesor: Grupo {asignacion.IdGrupo}, Materia {asignacion.DescMateria}, Faltantes: {asignacion.LeccionesRestantes}");
                    }
                    else
                    {
                        break;
                    }

                    intentos++;
                }

                if (asignacion.LeccionesRestantes > 0)
                {
                    Console.WriteLine($"⚠️  ADVERTENCIA: Grupo {asignacion.IdGrupo}, Materia {asignacion.DescMateria} aún tiene {asignacion.LeccionesRestantes} lecciones sin asignar");
                }
            }
        }

        /// <summary>
        /// Quinta fase: Completa cargas horarias de profesores SOLO reasignando lecciones sin profesor
        /// NO agrega lecciones nuevas para evitar exceder la carga horaria de materias
        /// </summary>
        private void QuintaFase_CompletarCargasProfesores(List<HorarioDto> horarios)
        {
            foreach (var profesor in _profesores.Where(p => p.Estado == "A"))
            {
                var leccionesAsignadas = _validator.ObtenerLeccionesAsignadas(horarios, profesor.Id_Profesor);
                var leccionesFaltantes = profesor.Carga_Horaria - leccionesAsignadas;

                if (leccionesFaltantes <= 0) continue;

                Console.WriteLine($"Profesor {profesor.Nombre} {profesor.Apellido}: {leccionesAsignadas}/{profesor.Carga_Horaria} - Faltantes: {leccionesFaltantes}");

                // Buscar materias que el profesor puede enseñar
                var materiasProfesor = _profXMateria
                    .Where(pm => pm.Id_Profesor == profesor.Id_Profesor && pm.Estado == "A")
                    .Select(pm => pm.Id_Materia)
                    .ToList();

                if (!materiasProfesor.Any()) continue;

                int leccionesAgregadas = 0;

                // SOLO intentar reemplazar lecciones con profesor null (sin asignar)
                // NO agregar lecciones nuevas para evitar exceder cargas de materias
                var leccionesConProfesorFaltante = horarios
                    .Where(h => h.Id_Profesor == null && materiasProfesor.Contains(h.Id_Materia))
                    .ToList();

                foreach (var leccion in leccionesConProfesorFaltante)
                {
                    if (leccionesAgregadas >= leccionesFaltantes) break;

                    // VALIDACIÓN CRÍTICA: Verificar que asignar esta lección NO exceda la carga del profesor
                    if (!_validator.PuedeAceptarMasLecciones(horarios, profesor.Id_Profesor, 1))
                    {
                        Console.WriteLine($"  ⚠️  Profesor {profesor.Nombre} {profesor.Apellido} ya alcanzó su carga máxima");
                        break;
                    }

                    // Verificar que el profesor pueda tomar esta lección (sin restricciones ni colisiones)
                    if (!_validator.TieneRestriccion(profesor.Id_Profesor, leccion.Dia ?? "", leccion.Hora_Inicio, leccion.Hora_Fin) &&
                        !_validator.TieneColisionProfesor(
                            horarios.Where(h => h.Id_Horario != leccion.Id_Horario).ToList(),
                            profesor.Id_Profesor,
                            leccion.Dia ?? "",
                            leccion.Hora_Inicio,
                            leccion.Hora_Fin))
                    {
                        leccion.Id_Profesor = profesor.Id_Profesor.ToString();
                        leccionesAgregadas++;
                        Console.WriteLine($"  → Asignado: {leccion.Dia} {leccion.Hora_Inicio}-{leccion.Hora_Fin}");
                    }
                }

                // NO agregar nuevas lecciones - esto causaba el exceso
                // Si el profesor queda con carga incompleta, es porque:
                // 1. No hay lecciones sin profesor de sus materias
                // 2. Todas las lecciones ya están asignadas correctamente
                // Esto es CORRECTO y no debe considerarse error
            }
        }

        /// <summary>
        /// Busca el mejor bloque disponible PRIORIZANDO LA CONTINUIDAD para el grupo
        /// Limita a máximo 6 lecciones consecutivas de la misma materia por día
        /// </summary>
        private List<HorarioDto>? BuscarMejorBloqueDisponible(
            List<HorarioDto> horariosExistentes,
            int idGrupo,
            int idProfesor,
            int idMateria,
            int cantidad)
        {
            List<HorarioDto>? mejorBloque = null;
            double mejorPuntuacion = double.MinValue;

            // Evaluar cada día de la semana
            foreach (var dia in Dias)
            {
                // IMPORTANTE: Verificar cuántas lecciones ya tiene esta materia en este día
                var leccionesEnEsteDia = horariosExistentes.Count(h =>
                    h.Id_Grupo == idGrupo &&
                    h.Dia == dia &&
                    h.Id_Materia == idMateria);

                // Si ya tiene 6 o más lecciones en este día, saltar al siguiente día
                if (leccionesEnEsteDia >= 6)
                    continue;

                // Ajustar cantidad si excedería el límite de 6
                int cantidadPermitida = Math.Min(cantidad, 6 - leccionesEnEsteDia);
                if (cantidadPermitida <= 0)
                    continue;

                // PRIORIDAD 1: Buscar bloques que extiendan lecciones existentes del grupo (sin crear huecos)
                var horariosGrupoEnDia = horariosExistentes
                    .Where(h => h.Id_Grupo == idGrupo && h.Dia == dia)
                    .OrderBy(h => h.Hora_Inicio)
                    .ToList();

                if (horariosGrupoEnDia.Any())
                {
                    // Intentar agregar al final del último bloque
                    var ultimaHora = horariosGrupoEnDia.Max(h => h.Hora_Fin);
                    var indiceUltimaHora = Bloques.FindIndex(b => b.inicio == ultimaHora);

                    if (indiceUltimaHora >= 0 && indiceUltimaHora + cantidadPermitida <= Bloques.Count)
                    {
                        var bloquesConsecutivos = Bloques.Skip(indiceUltimaHora).Take(cantidadPermitida).ToList();

                        // VALIDACIÓN CRÍTICA: Verificar que los bloques están en orden cronológico
                        if (bloquesConsecutivos.First().inicio >= ultimaHora &&
                            _validator.PuedeAsignarBloque(horariosExistentes, idGrupo, idProfesor, dia, bloquesConsecutivos) &&
                            !_validator.ExcedeBloquesConsecutivos(horariosExistentes, idGrupo, idMateria, dia, bloquesConsecutivos.First().inicio, 6))
                        {
                            var horariosTemporal = bloquesConsecutivos.Select(b => new HorarioDto
                            {
                                Dia = dia,
                                Id_Grupo = idGrupo,
                                Id_Profesor = idProfesor.ToString(),
                                Id_Materia = idMateria,
                                Hora_Inicio = b.inicio,
                                Hora_Fin = b.fin,
                                Estado = "A"
                            }).ToList();

                            return horariosTemporal;
                        }
                    }

                    // Intentar agregar al inicio del primer bloque
                    var primeraHora = horariosGrupoEnDia.Min(h => h.Hora_Inicio);
                    var indicePrimeraHora = Bloques.FindIndex(b => b.fin == primeraHora);

                    if (indicePrimeraHora >= cantidadPermitida - 1)
                    {
                        var bloquesAnteriores = Bloques.Skip(indicePrimeraHora - cantidadPermitida + 1).Take(cantidadPermitida).ToList();

                        // VALIDACIÓN CRÍTICA: Verificar que los bloques están ANTES cronológicamente
                        if (bloquesAnteriores.Any() && bloquesAnteriores.Last().fin <= primeraHora &&
                            _validator.PuedeAsignarBloque(horariosExistentes, idGrupo, idProfesor, dia, bloquesAnteriores) &&
                            !_validator.ExcedeBloquesConsecutivos(horariosExistentes, idGrupo, idMateria, dia, bloquesAnteriores.First().inicio, 6))
                        {
                            var horariosTemporal = bloquesAnteriores.Select(b => new HorarioDto
                            {
                                Dia = dia,
                                Id_Grupo = idGrupo,
                                Id_Profesor = idProfesor.ToString(),
                                Id_Materia = idMateria,
                                Hora_Inicio = b.inicio,
                                Hora_Fin = b.fin,
                                Estado = "A"
                            }).ToList();

                            return horariosTemporal;
                        }
                    }
                }

                // PRIORIDAD 2: Si no hay lecciones en el día, evaluar todos los bloques posibles
                for (int i = 0; i <= Bloques.Count - cantidadPermitida; i++)
                {
                    var bloquesEvaluados = Bloques.Skip(i).Take(cantidadPermitida).ToList();

                    // Verificar que el bloque completo esté disponible
                    if (!_validator.PuedeAsignarBloque(horariosExistentes, idGrupo, idProfesor, dia, bloquesEvaluados))
                        continue;

                    // Verificar límite de bloques consecutivos (máximo 6 lecciones seguidas)
                    if (_validator.ExcedeBloquesConsecutivos(horariosExistentes, idGrupo, idMateria, dia, bloquesEvaluados.First().inicio, 6))
                        continue;

                    // Crear horarios temporales
                    var horariosTemporal = bloquesEvaluados.Select(b => new HorarioDto
                    {
                        Dia = dia,
                        Id_Grupo = idGrupo,
                        Id_Profesor = idProfesor.ToString(),
                        Id_Materia = idMateria,
                        Hora_Inicio = b.inicio,
                        Hora_Fin = b.fin,
                        Estado = "A"
                    }).ToList();

                    // Calcular puntuación de este bloque
                    var horariosConNuevo = horariosExistentes.Concat(horariosTemporal).ToList();
                    double puntuacion = EvaluarCalidadAsignacion(horariosConNuevo, idGrupo, idProfesor, dia);

                    // BONIFICAR distribución en múltiples días (evitar concentración)
                    var diasConEstaMateria = horariosConNuevo
                        .Where(h => h.Id_Grupo == idGrupo && h.Id_Materia == idMateria)
                        .Select(h => h.Dia)
                        .Distinct()
                        .Count();
                    
                    puntuacion += diasConEstaMateria * 20; // Bonificar por distribuir en más días

                    if (puntuacion > mejorPuntuacion)
                    {
                        mejorPuntuacion = puntuacion;
                        mejorBloque = horariosTemporal;
                    }
                }
            }

            return mejorBloque;
        }

        /// <summary>
        /// Busca un bloque disponible solo para el grupo (sin considerar profesor) PRIORIZANDO CONTINUIDAD
        /// Se usa para asignar lecciones con profesor null (sin asignar)
        /// Limita a máximo 6 lecciones de la misma materia por día
        /// </summary>
        private List<HorarioDto>? BuscarBloqueDisponibleParaGrupo(
            List<HorarioDto> horariosExistentes,
            int idGrupo,
            int idMateria,
            int cantidad)
        {
            // Evaluar cada día de la semana
            foreach (var dia in Dias)
            {
                // IMPORTANTE: Verificar cuántas lecciones ya tiene esta materia en este día
                var leccionesEnEsteDia = horariosExistentes.Count(h =>
                    h.Id_Grupo == idGrupo &&
                    h.Dia == dia &&
                    h.Id_Materia == idMateria);

                // Si ya tiene 6 o más lecciones en este día, saltar al siguiente día
                if (leccionesEnEsteDia >= 6)
                    continue;

                // Ajustar cantidad si excedería el límite de 6
                int cantidadPermitida = Math.Min(cantidad, 6 - leccionesEnEsteDia);
                if (cantidadPermitida <= 0)
                    continue;

                // PRIORIDAD 1: Intentar extender lecciones existentes del grupo
                var horariosGrupoEnDia = horariosExistentes
                    .Where(h => h.Id_Grupo == idGrupo && h.Dia == dia)
                    .OrderBy(h => h.Hora_Inicio)
                    .ToList();

                if (horariosGrupoEnDia.Any())
                {
                    // Intentar agregar al final
                    var ultimaHora = horariosGrupoEnDia.Max(h => h.Hora_Fin);
                    var indiceUltimaHora = Bloques.FindIndex(b => b.inicio == ultimaHora);

                    if (indiceUltimaHora >= 0 && indiceUltimaHora + cantidadPermitida <= Bloques.Count)
                    {
                        var bloquesConsecutivos = Bloques.Skip(indiceUltimaHora).Take(cantidadPermitida).ToList();
                        
                        // VALIDACIÓN CRÍTICA: Verificar orden cronológico
                        if (bloquesConsecutivos.First().inicio < ultimaHora)
                        {
                            // Los bloques no están en orden correcto, saltar
                        }
                        else
                        {
                            bool disponible = true;
                            foreach (var bloque in bloquesConsecutivos)
                            {
                                if (_validator.TieneColisionGrupo(horariosExistentes, idGrupo, dia, bloque.inicio, bloque.fin))
                                {
                                    disponible = false;
                                    break;
                                }
                            }

                            if (disponible && !_validator.ExcedeBloquesConsecutivos(horariosExistentes, idGrupo, idMateria, dia, bloquesConsecutivos.First().inicio, 6))
                            {
                                return bloquesConsecutivos.Select(b => new HorarioDto
                                {
                                    Dia = dia,
                                    Id_Grupo = idGrupo,
                                    Id_Profesor = null,
                                    Id_Materia = idMateria,
                                    Hora_Inicio = b.inicio,
                                    Hora_Fin = b.fin,
                                    Estado = "A"
                                }).ToList();
                            }
                        }
                    }

                    // Intentar agregar al inicio
                    var primeraHora = horariosGrupoEnDia.Min(h => h.Hora_Inicio);
                    var indicePrimeraHora = Bloques.FindIndex(b => b.fin == primeraHora);

                    if (indicePrimeraHora >= cantidadPermitida - 1)
                    {
                        var bloquesAnteriores = Bloques.Skip(indicePrimeraHora - cantidadPermitida + 1).Take(cantidadPermitida).ToList();
                        
                        // VALIDACIÓN CRÍTICA: Verificar que los bloques están ANTES cronológicamente
                        if (bloquesAnteriores.Any() && bloquesAnteriores.Last().fin > primeraHora)
                        {
                            // Los bloques no están en orden correcto, saltar
                        }
                        else if (bloquesAnteriores.Any())
                        {
                            bool disponible = true;
                            foreach (var bloque in bloquesAnteriores)
                            {
                                if (_validator.TieneColisionGrupo(horariosExistentes, idGrupo, dia, bloque.inicio, bloque.fin))
                                {
                                    disponible = false;
                                    break;
                                }
                            }

                            if (disponible && !_validator.ExcedeBloquesConsecutivos(horariosExistentes, idGrupo, idMateria, dia, bloquesAnteriores.First().inicio, 6))
                            {
                                return bloquesAnteriores.Select(b => new HorarioDto
                                {
                                    Dia = dia,
                                    Id_Grupo = idGrupo,
                                    Id_Profesor = null,
                                    Id_Materia = idMateria,
                                    Hora_Inicio = b.inicio,
                                    Hora_Fin = b.fin,
                                    Estado = "A"
                                }).ToList();
                            }
                        }
                    }
                }

                // PRIORIDAD 2: Si no hay lecciones en el día, evaluar cualquier bloque disponible
                for (int i = 0; i <= Bloques.Count - cantidadPermitida; i++)
                {
                    var bloquesEvaluados = Bloques.Skip(i).Take(cantidadPermitida).ToList();

                    // Verificar que el bloque completo esté disponible SOLO para el grupo
                    bool disponible = true;
                    foreach (var bloque in bloquesEvaluados)
                    {
                        if (_validator.TieneColisionGrupo(horariosExistentes, idGrupo, dia, bloque.inicio, bloque.fin))
                        {
                            disponible = false;
                            break;
                        }
                    }

                    if (!disponible) continue;

                    // Verificar límite de bloques consecutivos (máximo 6 lecciones seguidas)
                    if (_validator.ExcedeBloquesConsecutivos(horariosExistentes, idGrupo, idMateria, dia, bloquesEvaluados.First().inicio, 6))
                        continue;

                    // Crear horarios con profesor null (sin asignar)
                    var horariosTemporal = bloquesEvaluados.Select(b => new HorarioDto
                    {
                        Dia = dia,
                        Id_Grupo = idGrupo,
                        Id_Profesor = null,
                        Id_Materia = idMateria,
                        Hora_Inicio = b.inicio,
                        Hora_Fin = b.fin,
                        Estado = "A"
                    }).ToList();

                    return horariosTemporal;
                }
            }

            return null;
        }

        /// <summary>
        /// Evalúa la calidad de una asignación PRIORIZANDO ELIMINACIÓN DE HUECOS EN GRUPOS
        /// </summary>
        private double EvaluarCalidadAsignacion(List<HorarioDto> horarios, int idGrupo, int idProfesor, string dia)
        {
            double puntuacion = 100.0;

            // Factor 1: PRIORIDAD MÁXIMA - Penalizar FUERTEMENTE huecos en horario del grupo
            var horariosGrupoEnDia = horarios.Where(h => h.Id_Grupo == idGrupo && h.Dia == dia).OrderBy(h => h.Hora_Inicio).ToList();
            if (horariosGrupoEnDia.Any())
            {
                // Contar huecos en el horario del grupo
                int huecosGrupo = 0;
                for (int i = 0; i < horariosGrupoEnDia.Count - 1; i++)
                {
                    if (horariosGrupoEnDia[i].Hora_Fin != horariosGrupoEnDia[i + 1].Hora_Inicio)
                    {
                        huecosGrupo++;
                    }
                }

                // PENALIZACIÓN MUY FUERTE por huecos en grupos (los estudiantes NO deben tener huecos)
                puntuacion -= huecosGrupo * 1000;

                // Bonificar FUERTEMENTE bloques consecutivos para el grupo
                int consecutivosGrupo = 1;
                TimeSpan? horaFinAnterior = null;

                foreach (var h in horariosGrupoEnDia)
                {
                    if (horaFinAnterior == null || h.Hora_Inicio == horaFinAnterior)
                    {
                        consecutivosGrupo++;
                        horaFinAnterior = h.Hora_Fin;
                    }
                    else
                    {
                        horaFinAnterior = h.Hora_Fin;
                    }
                }

                puntuacion += consecutivosGrupo * 50; // Bonificación fuerte por continuidad
            }

            // Factor 2: Preferir días con menos carga para el grupo (distribución uniforme)
            var leccionesPorDiaGrupo = Dias.Select(d => horarios.Count(h => h.Id_Grupo == idGrupo && h.Dia == d)).ToList();
            var desviacionGrupo = leccionesPorDiaGrupo.Any() ? leccionesPorDiaGrupo.Max() - leccionesPorDiaGrupo.Min() : 0;
            puntuacion -= desviacionGrupo * 10;

            // Factor 3: Preferir días con menos carga para el profesor (menor prioridad)
            var leccionesPorDiaProfesor = Dias.Select(d => horarios.Count(h => h.Id_Profesor == idProfesor.ToString() && h.Dia == d)).ToList();
            var desviacionProfesor = leccionesPorDiaProfesor.Any() ? leccionesPorDiaProfesor.Max() - leccionesPorDiaProfesor.Min() : 0;
            puntuacion -= desviacionProfesor * 1;

            // Factor 4: Bonificar LIGERAMENTE bloques que no generan huecos en el horario del profesor
            // (pero esto es MUCHO menos importante que evitar huecos en grupos)
            var horariosProfesorEnDia = horarios.Where(h => h.Id_Profesor == idProfesor.ToString() && h.Dia == dia).OrderBy(h => h.Hora_Inicio).ToList();
            bool generaHuecoProfesor = false;

            if (horariosProfesorEnDia.Count >= 2)
            {
                for (int i = 0; i < horariosProfesorEnDia.Count - 1; i++)
                {
                    if (horariosProfesorEnDia[i].Hora_Fin != horariosProfesorEnDia[i + 1].Hora_Inicio)
                    {
                        generaHuecoProfesor = true;
                        break;
                    }
                }
            }

            if (!generaHuecoProfesor)
                puntuacion += 5; // Bonificación menor - no es prioridad

            return puntuacion;
        }

        /// <summary>
        /// Verifica que todas las materias tengan sus lecciones completas asignadas
        /// </summary>
        private void VerificarCompletitud(List<HorarioDto> horarios)
        {
            Console.WriteLine("\n===== VERIFICACIÓN DE COMPLETITUD =====");
            
            var faltantes = _validator.ObtenerTodasLeccionesFaltantes(horarios, _grupos, _materiasXNivel);

            if (faltantes.Any())
            {
                Console.WriteLine($"⚠️  Se encontraron {faltantes.Count} materias con lecciones faltantes:");
                foreach (var (key, cantidad) in faltantes)
                {
                    var grupo = _grupos.First(g => g.Id_Grupo == key.idGrupo);
                    var materia = _materiasXNivel.First(m => m.Id_Materia == key.idMateria);
                    Console.WriteLine($"   - Grupo {grupo.Nombre}, Materia {materia.Desc_Materia}: {cantidad} lecciones faltantes");
                }
            }
            else
            {
                Console.WriteLine("✅ Todas las materias tienen sus lecciones completas");
            }

            // Verificar profesores con carga incompleta
            Console.WriteLine("\n===== VERIFICACIÓN DE CARGAS PROFESORES =====");
            foreach (var profesor in _profesores.Where(p => p.Estado == "A"))
            {
                var leccionesAsignadas = _validator.ObtenerLeccionesAsignadas(horarios, profesor.Id_Profesor);
                if (leccionesAsignadas != profesor.Carga_Horaria)
                {
                    var diferencia = profesor.Carga_Horaria - leccionesAsignadas;
                    var simbolo = diferencia > 0 ? "⚠️ " : "❌";
                    Console.WriteLine($"{simbolo} {profesor.Nombre} {profesor.Apellido}: {leccionesAsignadas}/{profesor.Carga_Horaria} ({(diferencia > 0 ? "-" : "+")}{Math.Abs(diferencia)})");
                }
            }
        }
    }
}
