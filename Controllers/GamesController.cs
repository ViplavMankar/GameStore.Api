using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GameStore.Api.DTOs;
using GameStore.Api.Services;

namespace GameStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _service;

    public GamesController(IGameService service)
    {
        _service = service;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost("Create")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] GameCreateDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        Console.WriteLine("userId: " + userId);
        var game = await _service.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetAll), new { id = game.Id }, game);
    }
}
