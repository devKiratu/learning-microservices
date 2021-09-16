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
    [Route("api/commands/[controller]")]
    [ApiController]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepository repository;
        private readonly IMapper mapper;

        public PlatformsController(ICommandRepository repository, IMapper mapper)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Platform>), 200)]
        public IActionResult GetPlatforms()
        {
            Console.WriteLine("--> Getting platforms from Commands Service");
            var platforms = repository.GetAllPlatforms();

            return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        [HttpPost]
        public IActionResult TestInboundConnection()
        {
            Console.WriteLine("--> Inbound POST command from platforms controller.");

            return Ok("Inbound POST Command Service");
        }
    }
}
