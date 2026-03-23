using GameStore.Api.Data;
using GameStore.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly GameStoreDbContext _context;
        private readonly HttpClient _authClient;

        public LeaderboardController(GameStoreDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _authClient = httpClientFactory.CreateClient("AuthService");
        }

        [HttpGet("playtime")]
        public async Task<IActionResult> GetPlaytimeLeaderboard()
        {
            var users = await _context.UserStatistics
                .OrderByDescending(x => x.TotalSecondsPlayed)
                .Take(50)
                .ToListAsync();

            var result = users.Select((x, index) => new
            {
                Rank = index + 1,
                Username = ResolveUsernameAsync(x.UserId).Result,
                Value = x.TotalSecondsPlayed
            });

            return Ok(result);
        }
        [HttpGet("xp")]
        public async Task<IActionResult> GetXpLeaderboard()
        {
            var users = await _context.UserStatistics
                .OrderByDescending(x => x.TotalXP)
                .Take(50)
                .ToListAsync();

            var result = users.Select((x, index) => new
            {
                Rank = index + 1,
                Username = ResolveUsernameAsync(x.UserId).Result,
                Value = x.TotalXP
            });

            return Ok(result);
        }
        private async Task<string?> ResolveUsernameAsync(Guid? userId)
        {
            if (userId == null)
            {
                return "Anonymous";
            }
            try
            {
                var response = await _authClient.GetAsync($"api/users/{userId.ToString()}");
                if (!response.IsSuccessStatusCode)
                    return "Unknown";

                var user = await response.Content.ReadFromJsonAsync<UserReadDto>();
                return user?.Username ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
        private class UserReadDto
        {
            public string Id { get; set; }
            public string Username { get; set; } = string.Empty;
        }
    }
}
