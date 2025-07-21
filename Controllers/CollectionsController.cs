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

    public CollectionsController(IGameService service)
    {
        _service = service;
    }

    [HttpPost("Add")]
    [Authorize]
    public async Task<IActionResult> Add(CollectionCreateDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var added = await _service.AddToCollectionAsync(userId, dto.GameId);

        if (!added) return Conflict("Already added");
        return Ok("Added to collection");
    }

    [HttpDelete("Remove/{id}")]
    [Authorize]
    public async Task<IActionResult> Remove(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var removed = await _service.RemoveFromCollectionAsync(id, userId);

        if (!removed) return Conflict("No game found");
        return Ok("Removed from Collection");
    }
}
