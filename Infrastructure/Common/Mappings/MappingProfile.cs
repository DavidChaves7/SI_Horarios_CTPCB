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
            
            CreateMap<CPC_PUNTAJE_CAPACIDAD, QueryPuntajeCapacidadDto>().ReverseMap();
            CreateMap<Materia, MateriaDto>().ReverseMap();
            CreateMap<Profesor, ProfesorDto>().ReverseMap();
            CreateMap<Profesor_X_Materia, Profesor_X_MateriaDto>().ReverseMap();
        }
    }
}
