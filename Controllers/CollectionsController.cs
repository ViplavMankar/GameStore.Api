using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GameStore.Api.DTOs;
using GameStore.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CollectionsController : ControllerBase
{
    private readonly IGameService _service;
    private readonly ICollectionService _collectionService;

    public CollectionsController(IGameService service, ICollectionService collectionService)
    {
        _service = service;
        _collectionService = collectionService;
    }

    [HttpPost("Add")]
    [Authorize]
    public async Task<IActionResult> Add([FromBody] CollectionCreateDto dto)
    {
        // var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var added = await _collectionService.AddToCollectionAsync(dto.UserId, dto.GameId);

        if (!added) return Conflict("Already added");
        return Ok("Added to collection");
    }

    [HttpDelete("Remove/{id}")]
    [Authorize]
    public async Task<IActionResult> Remove(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var removed = await _collectionService.RemoveFromCollectionAsync(id, userId);

        if (!removed) return Conflict("No game found");
        return Ok("Removed from Collection");
    }

    [HttpGet("GetAll")]
    [Authorize]
    public async Task<IActionResult> GetUserCollection()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        // Console.WriteLine("userId: " + userId);
        var games = await _collectionService.GetUserCollectionAsync(userId);
        return Ok(games);
    }

    [HttpGet("exists/{gameId}")]
    public async Task<IActionResult> IsGameSaved(Guid gameId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        // Console.WriteLine("userId: " + userId);
        bool isSaved = await _collectionService.IsGameSavedAsync(userId, gameId);
        return Ok(new { isSaved });
    }
}
