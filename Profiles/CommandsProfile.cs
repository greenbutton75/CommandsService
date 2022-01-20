using AutoMapper;
using CommandsService.Dtos;

namespace CommandsService.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            // src -> tgt
            CreateMap<Models.Platform, PlatformReadDto>();
            
            CreateMap<Models.Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Models.Command>();
        }
    }
}