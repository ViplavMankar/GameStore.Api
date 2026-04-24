using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GameStore.Api.Models;
using GameStore.Api.Data;
using System.Security.Claims;
using GameStore.Api.DTOs;
using Microsoft.EntityFrameworkCore;
using GameStore.Api.Services;
using Microsoft.AspNetCore.SignalR;
using GameStore.Api.Hubs;

namespace GameStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly GameStoreDbContext _db;
        private readonly IUserStatisticsService _userStatsService;
        private readonly IAchievementService _achievementService;
        private readonly HttpClient _authClient;
        private readonly StreakService _streakService;
        private readonly IHubContext<RealtimeHub> _hub;

        public SessionsController(GameStoreDbContext db,
                                IUserStatisticsService userStatsService,
                                IAchievementService achievementService,
                                StreakService streakService,
                                IHubContext<RealtimeHub> hub)
        {
            _db = db;
            _userStatsService = userStatsService;
            _achievementService = achievementService;
            _streakService = streakService;
            _hub = hub;
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
            var session = await _db.GameSessions
                .FirstOrDefaultAsync(x => x.Id == dto.SessionId);

            if (session == null)
                return NotFound();

            if (session.EndedAt != null)
                return BadRequest("Session already ended");

            session.EndedAt = DateTime.UtcNow;

            // var duration = (int)(session.EndedAt.Value - session.StartedAt).TotalSeconds;
            TimeSpan? duration = session.EndedAt - session.StartedAt;
            session.DurationSeconds = duration.HasValue
                ? (int)duration.Value.TotalSeconds
                : (int?)null;

            var today = DateTime.UtcNow.Date;

            var challenges = await _db.DailyChallenges
                .Where(x => x.Date == today)
                .ToListAsync();

            foreach (var challenge in challenges)
            {
                var progress = await _db.UserDailyChallengeProgresses
                    .FirstOrDefaultAsync(x =>
                        x.UserId == session.UserId &&
                        x.ChallengeId == challenge.Id);

                // 🔹 Lazy creation
                if (progress == null)
                {
                    progress = new UserDailyChallengeProgress
                    {
                        UserId = session.UserId,
                        ChallengeId = challenge.Id,
                        CurrentProgress = 0,
                        IsCompleted = false
                    };

                    _db.UserDailyChallengeProgresses.Add(progress);
                }

                // 🔥 Skip if already completed
                if (progress.IsCompleted)
                    continue;

                // 🔥 Update progress based on challenge type
                if (challenge.Title == "Play Games")
                {
                    progress.CurrentProgress += 1;
                }
                else if (challenge.Title == "Play Time")
                {
                    progress.CurrentProgress += (int)duration.Value.TotalSeconds;
                }

                // ✅ Check completion
                if (progress.CurrentProgress >= challenge.TargetValue)
                {
                    progress.IsCompleted = true;
                    progress.CompletedAt = DateTime.UtcNow;

                    // 🎉 GIVE XP
                    var stats = await _db.UserStatistics
                        .FirstOrDefaultAsync(x => x.UserId == session.UserId);
                    if (stats != null)
                        stats.TotalXP += challenge.XPReward;
                    else
                        Console.WriteLine("User stats does not exist");
                }
            }

            await _db.SaveChangesAsync();

            var streakResult = await _streakService.UpdateStreakAsync(session.UserId);

            if (streakResult.UpdatedToday)
            {
                await _hub.Clients
                    .User(session.UserId.ToString())
                    .SendAsync("StreakUpdated", new
                    {
                        current = streakResult.Streak.CurrentStreak,
                        max = streakResult.Streak.MaxStreak
                    });
            }

            // 🔥 STEP 1: Update UserStats
            await _userStatsService.UpdateAfterSessionAsync(session.UserId, session.GameId, (int)duration.Value.TotalSeconds);

            // 🔥 STEP 2: Evaluate Achievements
            var unlockedAchievements = await _achievementService
                .EvaluateAchievementsAsync(session.UserId);

            // 🔥 STEP 3: Return unlocked achievements to UI
            return Ok(unlockedAchievements);
        }
    }
}
