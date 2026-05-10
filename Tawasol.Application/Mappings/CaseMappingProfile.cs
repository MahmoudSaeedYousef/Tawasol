using AutoMapper;
using Tawasol.Application.DTOs.Cases;
using Tawasol.Domain.Entities;

namespace Tawasol.Application.Mappings;

public class CaseMappingProfile : Profile
{
    public CaseMappingProfile()
    {
        CreateMap<Case, CaseResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ExtraDetails, opt => opt.MapFrom(src => src.ExtraDetails));
    }
}
