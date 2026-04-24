using System.Security.Claims;
using GameStore.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardsController : ControllerBase
    {
        private readonly GameStoreDbContext _context;

        public DashboardsController(GameStoreDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetXP")]
        [Authorize]
        public async Task<IActionResult> GetXP()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var xp = await _context.UserStatistics
            .Where(us => us.UserId == userId)
            .Select(us => us.TotalXP)
            .FirstOrDefaultAsync();
            return Ok(new { TotalXP = xp });
        }
        [HttpGet("GetStreak")]
        [Authorize]
        public async Task<IActionResult> GetStreak()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var streak = await _context.UserStreaks
                .Where(s => s.UserId == userId)
                .Select(s => new { s.CurrentStreak, s.MaxStreak })
                .FirstOrDefaultAsync();
            return Ok(new { CurrentStreak = streak?.CurrentStreak ?? 0, MaxStreak = streak?.MaxStreak ?? 0 });
        }
    }
}
