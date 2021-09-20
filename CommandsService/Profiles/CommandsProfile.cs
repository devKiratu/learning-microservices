using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using PlatformService;

namespace CommandsService.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<PlatformPublishedDto, Platform>()
                .ForMember(dest => dest.ExternalId, o => o.MapFrom(src => src.Id));
            CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(dest => dest.ExternalId, o => o.MapFrom(src => src.PlatformId));
                //.ForMember(dest => dest.Commands, o => o.Ignore());
        }
    }
}
