using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepository _platformRepository;
    private readonly IMapper _mapper;

    public PlatformsController(IPlatformRepository platformRepository, IMapper mapper)
    {
        _platformRepository = platformRepository;
        _mapper = mapper;
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
    public ActionResult<PlatformReadDto> CreatePlatform([FromBody] PlatformCreateDto request)
    {
        Console.WriteLine("--> Creating Platforms...");

        try
        {
            var data = _mapper.Map<Platform>(request);

            _platformRepository.CreatePlatform(data);

            bool isSuccess = _platformRepository.SaveChanges();

            if (isSuccess)
            {
                var result = _mapper.Map<PlatformReadDto>(data);
                return CreatedAtRoute(nameof(GetPlatformById), new { Id = result.Id }, result);
            }
            else
                return BadRequest("Wasn't possible to create a new plataform, check your input data.");

        }
        catch (Exception ex)
        {
            return BadRequest($"Unexpected error, more details: {ex.Message}");
        }

    }
}
