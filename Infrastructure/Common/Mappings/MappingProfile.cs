using AutoMapper;
using Domain.Entities;
using Infrastructure.DTOs;

namespace Infrastructure.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
            CreateMap<CPC_PUNTAJE_CAPACIDAD, QueryPuntajeCapacidadDto>().ReverseMap();
            
        }
    }
}
