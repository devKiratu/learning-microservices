using AutoMapper;
using Grpc.Core;
using PlatformService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepository repository;
        private readonly IMapper mapper;

        public GrpcPlatformService(IPlatformRepository repository, IMapper mapper)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            var response = new PlatformResponse();

            var platforms = repository.GetPlatforms();
            foreach (var p in platforms)
            {
                response.Platform.Add(mapper.Map<GrpcPlatformModel>(p));
            }

            return Task.FromResult(response);
        }
    }
}
