using AutoMapper;
using CareConnect.DTOs.Clients;
using CareConnect.Infrastructure.Entities;

namespace CareConnect.Commands.Mapping;

public sealed class ClientMappingProfile : Profile
{
    public ClientMappingProfile()
    {
        CreateMap<UpdateClientDto, Client>()
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
