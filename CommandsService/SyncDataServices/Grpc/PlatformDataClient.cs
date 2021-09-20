using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using PlatformService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration config;
        private readonly IMapper mapper;

        public PlatformDataClient(IConfiguration config, IMapper mapper)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            Console.WriteLine($"--> Calling Grpc service: {config["GrpcPlatform"]}");
            using var channel = GrpcChannel.ForAddress(config["GrpcPlatform"]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);

            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not call Grpc Server: {ex.Message}");
                return null;
            }
            
        }
    }
}
