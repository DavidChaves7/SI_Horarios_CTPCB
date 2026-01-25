using Infrastructure.DTOs.Horarios;
using Infrastructure.DTOs.Mantenimientos;

namespace API.Infrastructure.Services.HorarioGeneration
{
    /// <summary>
    /// Validador de restricciones para la generación de horarios
    /// </summary>
    public class HorarioValidator
    {
        private readonly List<Restriccion_ProfesorDto> _restricciones;
        private readonly List<ProfesorDto> _profesores;

        public HorarioValidator(
            List<Restriccion_ProfesorDto> restricciones,
            List<ProfesorDto> profesores)
        {
            _restricciones = restricciones;
            _profesores = profesores;
        }

        /// <summary>
        /// Verifica si un profesor tiene restricción en un horario específico
        /// </summary>
        public bool TieneRestriccion(int idProfesor, string dia, TimeSpan horaInicio, TimeSpan horaFin)
        {
            return _restricciones.Any(r =>
                r.Id_Profesor == idProfesor &&
                r.Dia == dia &&
                r.Estado == "A" &&
                HorariosSeSuperponen(
                    TimeSpan.Parse(r.Hora_Inicio ?? "00:00"),
                    TimeSpan.Parse(r.Hora_Fin ?? "23:59"),
                    horaInicio,
                    horaFin));
        }

        /// <summary>
        /// Verifica si hay colisión de horario para un grupo
        /// </summary>
        public bool TieneColisionGrupo(List<HorarioDto> horariosExistentes, int idGrupo, string dia, TimeSpan horaInicio, TimeSpan horaFin)
        {
            return horariosExistentes.Any(h =>
                h.Id_Grupo == idGrupo &&
                h.Dia == dia &&
                h.Estado == "A" &&
                HorariosSeSuperponen(h.Hora_Inicio, h.Hora_Fin, horaInicio, horaFin));
        }

        /// <summary>
        /// Verifica si hay colisión de horario para un profesor
        /// </summary>
        public bool TieneColisionProfesor(List<HorarioDto> horariosExistentes, int idProfesor, string dia, TimeSpan horaInicio, TimeSpan horaFin)
        {
            return horariosExistentes.Any(h =>
                h.Id_Profesor == idProfesor.ToString() &&
                h.Dia == dia &&
                h.Estado == "A" &&
                HorariosSeSuperponen(h.Hora_Inicio, h.Hora_Fin, horaInicio, horaFin));
        }

        /// <summary>
        /// Verifica si dos rangos de horarios se superponen
        /// </summary>
        private bool HorariosSeSuperponen(TimeSpan inicio1, TimeSpan fin1, TimeSpan inicio2, TimeSpan fin2)
        {
            return inicio1 < fin2 && inicio2 < fin1;
        }

        /// <summary>
        /// Calcula cuántas lecciones tiene asignadas un profesor en los horarios existentes
        /// </summary>
        public int ObtenerLeccionesAsignadas(List<HorarioDto> horariosExistentes, int idProfesor)
        {
            return horariosExistentes.Count(h => h.Id_Profesor == idProfesor.ToString());
        }

        /// <summary>
        /// Verifica si el profesor puede asumir más lecciones según su carga horaria
        /// </summary>
        public bool PuedeAceptarMasLecciones(List<HorarioDto> horariosExistentes, int idProfesor, int cantidadAAgregar = 1)
        {
            var profesor = _profesores.FirstOrDefault(p => p.Id_Profesor == idProfesor);
            if (profesor == null) return false;

            var leccionesActuales = ObtenerLeccionesAsignadas(horariosExistentes, idProfesor);
            return (leccionesActuales + cantidadAAgregar) <= profesor.Carga_Horaria;
        }

        /// <summary>
        /// Valida si se puede asignar un bloque completo de lecciones
        /// </summary>
        public bool PuedeAsignarBloque(
            List<HorarioDto> horariosExistentes,
            int idGrupo,
            int idProfesor,
            string dia,
            List<(TimeSpan inicio, TimeSpan fin)> bloques)
        {
            foreach (var bloque in bloques)
            {
                if (TieneRestriccion(idProfesor, dia, bloque.inicio, bloque.fin) ||
                    TieneColisionGrupo(horariosExistentes, idGrupo, dia, bloque.inicio, bloque.fin) ||
                    TieneColisionProfesor(horariosExistentes, idProfesor, dia, bloque.inicio, bloque.fin))
                {
                    return false;
                }
            }

            return PuedeAceptarMasLecciones(horariosExistentes, idProfesor, bloques.Count);
        }

        /// <summary>
        /// Cuenta la cantidad de bloques consecutivos en un día para un grupo
        /// </summary>
        public int ContarBloquesConsecutivos(List<HorarioDto> horarios, int idGrupo, string dia, TimeSpan desde)
        {
            var horariosDelDia = horarios
                .Where(h => h.Id_Grupo == idGrupo && h.Dia == dia && h.Hora_Inicio >= desde)
                .OrderBy(h => h.Hora_Inicio)
                .ToList();

            int consecutivos = 0;
            TimeSpan? horaFinEsperada = null;

            foreach (var horario in horariosDelDia)
            {
                if (horaFinEsperada == null || horario.Hora_Inicio == horaFinEsperada)
                {
                    consecutivos++;
                    horaFinEsperada = horario.Hora_Fin;
                }
                else
                {
                    break;
                }
            }

            return consecutivos;
        }

        /// <summary>
        /// Verifica si una materia ya tiene el máximo de lecciones consecutivas permitidas
        /// </summary>
        public bool ExcedeBloquesConsecutivos(
            List<HorarioDto> horarios,
            int idGrupo,
            int idMateria,
            string dia,
            TimeSpan horaInicio,
            int maxConsecutivos = 3)
        {
            var leccionesConsecutivas = horarios
                .Where(h => h.Id_Grupo == idGrupo && h.Dia == dia && h.Id_Materia == idMateria)
                .OrderBy(h => h.Hora_Inicio)
                .ToList();

            if (!leccionesConsecutivas.Any()) return false;

            // Buscar bloques consecutivos que incluyan o estén cerca del nuevo horario
            int consecutivos = 1;
            TimeSpan? ultimaHoraFin = null;

            foreach (var leccion in leccionesConsecutivas)
            {
                if (ultimaHoraFin == null)
                {
                    ultimaHoraFin = leccion.Hora_Fin;
                }
                else if (leccion.Hora_Inicio == ultimaHoraFin)
                {
                    consecutivos++;
                    ultimaHoraFin = leccion.Hora_Fin;

                    if (consecutivos >= maxConsecutivos)
                        return true;
                }
                else
                {
                    consecutivos = 1;
                    ultimaHoraFin = leccion.Hora_Fin;
                }
            }

            // Verificar si el nuevo horario extendería un bloque existente
            if (leccionesConsecutivas.Any(l => l.Hora_Fin == horaInicio))
            {
                var bloqueActual = ContarBloquesConsecutivos(horarios, idGrupo, dia, 
                    leccionesConsecutivas.First(l => l.Hora_Fin == horaInicio).Hora_Inicio);
                return bloqueActual >= maxConsecutivos;
            }

            return false;
        }

        /// <summary>
        /// Obtiene las lecciones faltantes de una materia para un grupo específico
        /// </summary>
        public int ObtenerLeccionesFaltantes(
            List<HorarioDto> horarios,
            int idGrupo,
            int idMateria,
            int cargaHorariaRequerida)
        {
            var leccionesAsignadas = horarios.Count(h =>
                h.Id_Grupo == idGrupo &&
                h.Id_Materia == idMateria &&
                h.Estado == "A");

            return Math.Max(0, cargaHorariaRequerida - leccionesAsignadas);
        }

        /// <summary>
        /// Verifica si agregar lecciones excedería la carga horaria de la materia para el grupo
        /// </summary>
        public bool ExcedeCargaHorariaMateria(
            List<HorarioDto> horarios,
            int idGrupo,
            int idMateria,
            int cargaHorariaRequerida,
            int cantidadAAgregar)
        {
            var leccionesAsignadas = horarios.Count(h =>
                h.Id_Grupo == idGrupo &&
                h.Id_Materia == idMateria &&
                h.Estado == "A");

            return (leccionesAsignadas + cantidadAAgregar) > cargaHorariaRequerida;
        }

        /// <summary>
        /// Obtiene todas las materias con lecciones faltantes
        /// </summary>
        public Dictionary<(int idGrupo, int idMateria), int> ObtenerTodasLeccionesFaltantes(
            List<HorarioDto> horarios,
            List<GrupoDto> grupos,
            List<MateriaXNivelDto> materiasXNivel)
        {
            var faltantes = new Dictionary<(int idGrupo, int idMateria), int>();

            foreach (var grupo in grupos.Where(g => g.Estado == "A"))
            {
                var materiasDelNivel = materiasXNivel
                    .Where(m => m.Id_Nivel_Academico == grupo.Id_Nivel_Academico && m.Estado == "A")
                    .ToList();

                foreach (var materia in materiasDelNivel)
                {
                    var leccionesFaltantes = ObtenerLeccionesFaltantes(
                        horarios,
                        grupo.Id_Grupo,
                        materia.Id_Materia ?? 0,
                        materia.Carga_Horaria ?? 0);

                    if (leccionesFaltantes > 0)
                    {
                        faltantes[(grupo.Id_Grupo, materia.Id_Materia ?? 0)] = leccionesFaltantes;
                    }
                }
            }

            return faltantes;
        }
    }
}
