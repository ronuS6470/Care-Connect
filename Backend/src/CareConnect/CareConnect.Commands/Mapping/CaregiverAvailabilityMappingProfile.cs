using AutoMapper;
using CareConnect.DTOs.Caregivers;
using CareConnect.Infrastructure.Entities;

namespace CareConnect.Commands.Mapping;

public sealed class CaregiverAvailabilityMappingProfile : Profile
{
    public CaregiverAvailabilityMappingProfile()
    {
        CreateMap<CreateCaregiverAvailabilityDto, CaregiverAvailability>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

        CreateMap<UpdateCaregiverAvailabilityDto, CaregiverAvailability>()
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
