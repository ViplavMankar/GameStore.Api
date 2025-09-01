using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GameStore.Api.Models;
using GameStore.Api.Data;
using System.Security.Claims;
using GameStore.Api.DTOs;

namespace GameStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly GameStoreDbContext _db;
        private readonly HttpClient _authClient;

        public SessionsController(GameStoreDbContext db)
        {
            _db = db;
        }

        [HttpPost("start-session")]
        public async Task<IActionResult> StartSession([FromBody] StartSessionDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            // Console.WriteLine("User ID " + userId);
            // if (!Guid.TryParse(userIdStr, out var userId))
            //     return Unauthorized();
            var session = new GameSession
            {
                GameId = dto.GameId,
                UserId = userId, // extension from claims
                StartedAt = DateTime.UtcNow
            };
            _db.GameSessions.Add(session);
            await _db.SaveChangesAsync();
            return Ok(new { session.Id });
        }

        [HttpPost("end-session")]
        public async Task<IActionResult> EndSession([FromBody] EndSessionDto dto)
        {
            var session = await _db.GameSessions.FindAsync(dto.SessionId);
            if (session == null) return NotFound();

            session.EndedAt = DateTime.UtcNow;
            TimeSpan? duration = session.EndedAt - session.StartedAt;
            session.DurationSeconds = duration.HasValue
                ? (int)duration.Value.TotalSeconds
                : (int?)null;
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
