using AutoMapper;
using CareConnect.DTOs.Caregivers;
using CareConnect.Infrastructure.Entities;

namespace CareConnect.Commands.Mapping;

public sealed class CaregiverMappingProfile : Profile
{
    public CaregiverMappingProfile()
    {
        CreateMap<UpdateCaregiverDto, Caregiver>()
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
