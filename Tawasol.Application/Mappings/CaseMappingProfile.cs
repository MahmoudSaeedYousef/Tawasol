using AutoMapper;
using Tawasol.Application.DTOs.Cases;
using Tawasol.Domain.Entities;

namespace Tawasol.Application.Mappings;

public class CaseMappingProfile : Profile
{
    public CaseMappingProfile()
    {
        CreateMap<CaseItem, CaseItemDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

        CreateMap<Case, CaseResponseDto>()
            .ForMember(dest => dest.AttachmentUrls, opt => 
                opt.MapFrom(src => src.Attachments.Select(a => a.FilePath).ToList()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ExtraDetails, opt => opt.MapFrom(src => src.ExtraDetails))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.ApprovedBy))
            .ForMember(dest => dest.ApprovedAt, opt => opt.MapFrom(src => src.ApprovedAt))
            .ForMember(dest => dest.RejectedBy, opt => opt.MapFrom(src => src.RejectedBy))
            .ForMember(dest => dest.RejectedAt, opt => opt.MapFrom(src => src.RejectedAt))
            .ForMember(dest => dest.ClosedBy, opt => opt.MapFrom(src => src.ClosedBy))
            .ForMember(dest => dest.ClosedAt, opt => opt.MapFrom(src => src.ClosedAt))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
    }
}
