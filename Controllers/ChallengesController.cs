using System.Security.Claims;
using GameStore.Api.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChallengesController : ControllerBase
    {
        private readonly GameStoreDbContext _context;

        public ChallengesController(GameStoreDbContext context)
        {
            _context = context;
        }
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayChallenges()
        {
            var today = DateTime.UtcNow.Date;

            var challenges = await _context.DailyChallenges
                .Where(x => x.Date == today)
                .ToListAsync();

            return Ok(challenges);
        }
        [HttpGet("my-challenges")]
        public async Task<IActionResult> GetMyChallenges()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); // however you extract it
            var today = DateTime.UtcNow.Date;

            var challenges = await _context.DailyChallenges
                .Where(x => x.Date == today)
                .ToListAsync();

            var progressList = await _context.UserDailyChallengeProgresses
                .Where(x => x.UserId == userId)
                .ToListAsync();

            var result = challenges.Select(ch =>
            {
                var progress = progressList
                    .FirstOrDefault(p => p.ChallengeId == ch.Id);

                return new
                {
                    ch.Id,
                    ch.Title,
                    ch.Description,
                    ch.TargetValue,
                    ch.XPReward,

                    CurrentProgress = progress?.CurrentProgress ?? 0,
                    IsCompleted = progress?.IsCompleted ?? false
                };
            });

            return Ok(result);
        }
    }
}
