using AutoMapper;
using CareConnect.DTOs.CareTasks;
using CareConnect.Infrastructure.Entities;

namespace CareConnect.Commands.Mapping;

public sealed class CareTaskMappingProfile : Profile
{
    public CareTaskMappingProfile()
    {
        CreateMap<CreateCareTaskDto, CareTask>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

        CreateMap<UpdateCareTaskDto, CareTask>()
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
