using System;
using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory,
                IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    addPlatform(message);
                    break;
                default:
                    break;
            }

        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> DetermineEvent");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType?.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform_Published detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Undetermined detected");
                    return EventType.Undetermined;

            }
        }

        private void addPlatform(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                try
                {
                    var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
                    var platform = _mapper.Map<Platform>(platformPublishedDto);
                    if (!repo.ExternalPlatformExists(platform.ExternalID))
                    {
                        repo.CreatePlatform(platform);
                        repo.SaveChanges ();
                        Console.WriteLine("--> Platform added!");
                    }
                    else
                    {
                        Console.WriteLine($"--> addPlatform already erists {platform.ExternalID}");
                    }

                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"--> addPlatform error {ex.Message}");
                }

            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}