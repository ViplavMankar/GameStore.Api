using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GameStore.Api.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GameStore.Api.DTOs;
using GameStore.Api.Models;

namespace GameStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementsController : ControllerBase
    {
        private readonly GameStoreDbContext _context;

        public AchievementsController(GameStoreDbContext context)
        {
            _context = context;
        }
        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyAchievements()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // 🔹 Get unlocked achievements
            var unlockedIds = await _context.UserAchievements
                .Where(x => x.UserId == userId)
                .Select(x => x.AchievementId)
                .ToListAsync();

            // 🔹 Get all achievements
            var achievements = await _context.Achievements.ToListAsync();

            // 🔹 Map to DTO
            var result = achievements.Select(a => new AchievementDto
            {
                Name = a.Name,
                Description = a.Description,
                Icon = a.Icon,
                IsUnlocked = unlockedIds.Contains(a.Id)
            });

            return Ok(result);
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateAchievement([FromBody] CreateAchievementDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required");

            var achievement = new Achievement
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Icon = dto.Icon,
                ConditionType = dto.ConditionType,
                TargetValue = dto.TargetValue
            };

            _context.Achievements.Add(achievement);
            await _context.SaveChangesAsync();

            return Ok(achievement);
        }
    }
}
