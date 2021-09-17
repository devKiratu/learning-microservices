using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandsService.Controllers
{
    [Route("api/commands/platforms/{platformId}/[controller]")]
    [ApiController]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository repository;
        private readonly IMapper mapper;

        public CommandsController(ICommandRepository repository, IMapper mapper)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CommandReadDto>),200)]
        public IActionResult GetCommands(int platformId)
        {
            Console.WriteLine($"--> Hit GetCommands platform: {platformId}");

            if (!repository.PlatformExists(platformId))
            {
                return BadRequest($"Platform of id {platformId} does not exist");
            }

            var commands = repository.GetCommandsForPlatform((platformId));

            return Ok(mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        [ProducesResponseType(typeof(CommandReadDto), 200)]
        public IActionResult GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Hit GetCommandForPlatform: p{platformId}/c{commandId}");

            if (!repository.PlatformExists(platformId))
            {
                return BadRequest($"Platform of id {platformId} does not exist");
            }

            var command = repository.GetCommand(platformId, commandId);

            if (command == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<CommandReadDto>(command));

        }

        [HttpPost]
        [ProducesResponseType(typeof(CommandReadDto), 200)]
        public IActionResult CreateCommandForPlatorm(int platformId, CommandCreateDto commandCreateDto)
        {
            Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");
            if (!repository.PlatformExists(platformId))
            {
                return BadRequest($"Platform of id {platformId} does not exist");
            }

            var command = mapper.Map<Command>(commandCreateDto);
            repository.CreateCommand(platformId, command);
            repository.SaveChanges();

            var commandReadDto = mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId, commandId = commandReadDto.Id }, commandReadDto);
        }
    }
}
