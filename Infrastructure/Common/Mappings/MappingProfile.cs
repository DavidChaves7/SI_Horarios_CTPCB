using AutoMapper;
using Domain.Entities;
using Infrastructure.DTOs;
using Infrastructure.DTOs.Mantenimientos;

namespace Infrastructure.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
            CreateMap<Materia, MateriaDto>().ReverseMap();
            CreateMap<Profesor, ProfesorDto>().ReverseMap();
            CreateMap<Nivel_Academico, NivelAcademicoDto>().ReverseMap();
        }
    }
}
