using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepository _platformRepository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;

    public PlatformsController(
        IPlatformRepository platformRepository,
        IMapper mapper,
        ICommandDataClient commandDataClient)
    {
        _platformRepository = platformRepository;
        _mapper = mapper;
        _commandDataClient = commandDataClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
    {
        Console.WriteLine("--> Getting Platforms...");
        var data = _platformRepository.GetAllPlatforms();

        if (!data.Any())
            return NoContent();

        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(data));
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById([FromRoute] int id)
    {
        Console.WriteLine("--> Getting Platform by Id...");
        var data = _platformRepository.GetPlatformById(id);

        if (data is null)
            return NotFound();

        return Ok(_mapper.Map<PlatformReadDto>(data));
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform([FromBody] PlatformCreateDto request)
    {
        Console.WriteLine("--> Creating Platforms...");

        var data = _mapper.Map<Platform>(request);

        _platformRepository.CreatePlatform(data);

        bool isSuccess = _platformRepository.SaveChanges();

        if (isSuccess)
        {
            var result = _mapper.Map<PlatformReadDto>(data);

            try
            {
                await _commandDataClient.SendPlatformToCommand(result);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { Id = result.Id }, result);
        }
        else
            return BadRequest("Wasn't possible to create a new plataform, check your input data.");


    }
}
