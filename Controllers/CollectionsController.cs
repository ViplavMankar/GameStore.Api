using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GameStore.Api.DTOs;
using GameStore.Api.Services;

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

    [HttpPost]
    public async Task<IActionResult> Add(CollectionCreateDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var added = await _service.AddToCollectionAsync(userId, dto.GameId);

        if (!added) return Conflict("Already added");
        return Ok("Added to collection");
    }
}
