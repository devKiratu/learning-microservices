using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IMapper mapper;

        public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                
            }

        }

        private static EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining event");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform Published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could not determine event type");
                    return EventType.Undetermined;
            }

        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

            try
            {
                var platform = mapper.Map<Platform>(platformPublishedDto);
                if (!repository.ExternalPlatformExists(platform.ExternalId))
                {
                    repository.CreatePlatform(platform);
                    repository.SaveChanges();
                    Console.WriteLine($"--> Platform already added! : {platform}");
                }
                else
                {
                    Console.WriteLine("--> Platform already exists");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not add platform to Db : {ex.Message}");
            }
        }

    }
        enum EventType
        {
            PlatformPublished,
            Undetermined
        }
}
