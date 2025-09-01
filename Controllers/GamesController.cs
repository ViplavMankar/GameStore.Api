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
    private readonly IS3Service _s3Service;

    public GamesController(IGameService service, IS3Service s3Service)
    {
        _service = service;
        _s3Service = s3Service;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("Get/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var blog = await _service.GetByIdWithUsernameAsync(id);
        if (blog is null) return NotFound();
        return Ok(blog);
    }

    [HttpPost("Create")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] GameCreateDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        // Console.WriteLine("userId: " + userId);
        var game = await _service.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetAll), new { id = game.Id }, game);
    }

    [HttpPut("Edit/{id}")]
    [Authorize]
    public async Task<IActionResult> Edit([FromBody] GameEditDto dto, Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        // Console.WriteLine("userId: " + userId);
        var game = await _service.EditAsync(id, dto, userId);
        return CreatedAtAction(nameof(GetAll), new { id = game.Id }, game);
    }

    [HttpDelete("Delete/{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        // Console.WriteLine("userId: " + userId);
        // 1️⃣ Fetch the game from the database
        var game = await _service.GetByIdWithUsernameAsync(id);
        if (game == null)
            return NotFound("Game not found.");

        // 2️⃣ Delete the game files from S3
        if (!string.IsNullOrWhiteSpace(game.Title))
        {
            // assuming the S3 folder name is the same as the game title
            await _s3Service.DeleteFolderAsync(game.Title);
        }
        var gameBool = await _service.DeleteAsync(id, userId);
        return NoContent();
    }

    [HttpGet("Mine")]
    [Authorize]
    public async Task<IActionResult> GetMyGames()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _service.GetMyGamesAsync(userId));
    }

    [HttpGet("Mine/{id}")]
    [Authorize]
    public async Task<IActionResult> GetMySingleGame(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var game = await _service.GetMySingleGameAsync(userId, id);

        if (game == null)
            return NotFound(new { message = "Game not found or you are not authorized to access it." });

        return Ok(game);
    }
}
