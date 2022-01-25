using AutoMapper;
using CommandsService.Dtos;
using PlatformService;

namespace CommandsService.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            // src -> tgt
            CreateMap<Models.Platform, PlatformReadDto>();
            CreateMap<PlatformPublishedDto, Models.Platform>()
                .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.Id));
            
            CreateMap<Models.Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Models.Command>();

            CreateMap<GrpcPlatformModel, Models.Platform>()
                .ForMember (dest => dest.ExternalID, opt => opt.MapFrom(src => src.PlatformId))
                // 2 lines below will be done by AutoMapper automatically. Just overkill. No need them
                .ForMember (dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember (dest => dest.Commands, opt => opt.Ignore())
                ;
        }
    }
}