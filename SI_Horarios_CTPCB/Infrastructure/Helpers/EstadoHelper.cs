namespace SI_Horarios_CTPCB.Infrastructure.Helpers
{
    public static class EstadoHelper
    {
        public class Estados
        {
            public string? Value { get; set; } = string.Empty;
            public string? Desc { get; set; } = string.Empty;
        }

        public static List<Estados>? listaActivoInactivo { get; } = new()
        {
            new() { Desc = "Seleccione", Value = "" },
            new() { Desc = "Activo", Value = "A" },
            new() { Desc = "Inactivo", Value = "I" }
        };

        public static List<Estados>? listaTipoMateria { get; } = new()
        {
            new() { Desc = "Seleccione", Value = "" },
            new() { Desc = "Tecnica", Value = "T" },
            new() { Desc = "Academica", Value = "A" }
        };

        public static List<Estados>? listaDiasSemana { get; } = new()
        {
            new() { Desc = "Seleccione", Value = "" },
            new() { Desc = "Lunes", Value = "L" },
            new() { Desc = "Martes", Value = "K" },
            new() { Desc = "Miercoles", Value = "M" },
            new() { Desc = "Jueves", Value = "J" },
            new() { Desc = "Viernes", Value = "V" },
        };
    }




}
