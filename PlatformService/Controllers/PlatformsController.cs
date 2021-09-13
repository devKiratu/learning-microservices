using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository repository;
        private readonly IMapper mapper;
        private readonly ICommandDataClient client;

        public PlatformsController(IPlatformRepository repository, IMapper mapper, ICommandDataClient client)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        [HttpGet("{id}", Name = "GetPlatform")]
        [ProducesResponseType(typeof(PlatformReadDto), 200)]
        public IActionResult GetPlatform([FromRoute]int id)
        {
            var platform = repository.GetPlatform(id);
            if (platform == null)
            {
                return Problem(title: "Invalid Id",
                               detail: $"Requested platform of Id {id} does not exist",
                               statusCode: 400);
            }

            return Ok(mapper.Map<PlatformReadDto>(platform));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PlatformReadDto>), 200)]
        public IActionResult GetPlatforms()
        {
            var platforms = repository.GetPlatforms();

            if (!platforms.Any())
            {
                return NoContent();
            }

            return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        [HttpPost]
        [ProducesResponseType(typeof(PlatformReadDto),200)]
        public async Task<IActionResult> CreatePlatform([FromBody] PlatformCreateDto model)
        {
            var platform = mapper.Map<Platform>(model);

            repository.CreatePlatform(platform);
            repository.SaveChanges();

            var platformReadDto = mapper.Map<PlatformReadDto>(platform);

            try
            {

                await client.SendPlatformToCommand(platformReadDto);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Could not send command: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatform), new { id = platformReadDto.Id }, platformReadDto);
        }
    }
}
