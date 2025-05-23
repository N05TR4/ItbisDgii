using AutoMapper;
using ItbisDgii.Application.DTOs;
using ItbisDgii.Application.Wrappers;
using ItbisDgii.Domain.Entities;

namespace ItbisDgii.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo de Contribuyente a ContribuyenteDto
            CreateMap<Contribuyente, ContribuyenteDto>()
                .ForMember(d => d.Tipo, opt => opt.MapFrom(s => s.Tipo.ToString()))
                .ForMember(d => d.Estatus, opt => opt.MapFrom(s => s.Estatus.ToString()))
                .ForMember(d => d.TotalITBIS, opt => opt.MapFrom(s => s.CalcularTotalITBIS()));


            CreateMap<ComprobanteFiscal, ComprobanteFiscalDto>();

            
        }
    }
}
