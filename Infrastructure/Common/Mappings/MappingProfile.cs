using AutoMapper;
using Domain.Entities;
using Infrastructure.DTOs;
using Infrastructure.DTOs.Horarios;
using Infrastructure.DTOs.Mantenimientos;

namespace Infrastructure.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Grupo, GrupoDto>().ReverseMap();
            CreateMap<Materia, MateriaDto>().ReverseMap();
            CreateMap<Materia_X_Nivel, MateriaXNivelDto>().ReverseMap();
            CreateMap<Profesor, ProfesorDto>().ReverseMap();
            CreateMap<Nivel_Academico, NivelAcademicoDto>().ReverseMap();
            CreateMap<Profesor_X_Materia, Profesor_X_MateriaDto>().ReverseMap();
            CreateMap<Restriccion_Profesor, Restriccion_ProfesorDto>().ReverseMap();
            CreateMap<Seguridad, SeguridadDto>().ReverseMap();
        }
    }
}
