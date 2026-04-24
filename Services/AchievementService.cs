using System;
using Microsoft.EntityFrameworkCore;
using GameStore.Api.Data;
using GameStore.Api.Models;
using GameStore.Api.DTOs;
using Microsoft.AspNetCore.SignalR;
using GameStore.Api.Hubs;

namespace GameStore.Api.Services;

public class AchievementService : IAchievementService
{
    private readonly GameStoreDbContext _context;
    private readonly IHubContext<RealtimeHub> _hub;

    public AchievementService(GameStoreDbContext context, IHubContext<RealtimeHub> hub)
    {
        _context = context;
        _hub = hub;
    }

    public async Task<List<AchievementUnlockedDto>> EvaluateAchievementsAsync(Guid userId)
    {
        var stats = await _context.UserStatistics
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (stats == null)
            return new List<AchievementUnlockedDto>();

        var unlockedIds = await _context.UserAchievements
            .Where(x => x.UserId == userId)
            .Select(x => x.AchievementId)
            .ToListAsync();

        var lockedAchievements = await _context.Achievements
            .Where(a => !unlockedIds.Contains(a.Id))
            .ToListAsync();

        var newlyUnlocked = lockedAchievements
            .Where(a => IsUnlocked(a, stats))
            .ToList();

        if (newlyUnlocked.Any())
        {
            var now = DateTime.UtcNow;

            var userAchievements = newlyUnlocked.Select(a => new UserAchievement
            {
                UserId = userId,
                AchievementId = a.Id,
                UnlockedAt = now
            });

            _context.UserAchievements.AddRange(userAchievements);
            await _context.SaveChangesAsync();

            foreach (var achievement in newlyUnlocked)
            {
                var dto = new AchievementUnlockedDto
                {
                    Id = achievement.Id,
                    Name = achievement.Name,
                    Description = achievement.Description
                };

                await _hub.Clients.User(userId.ToString())
                    .SendAsync("AchievementUnlocked", dto);
                // await _hub.Clients.All.SendAsync("AchievementUnlocked", dto);
            }
        }

        return newlyUnlocked.Select(a => new AchievementUnlockedDto
        {
            Id = a.Id,
            Name = a.Name,
            Description = a.Description
        }).ToList();
    }

    private bool IsUnlocked(Achievement achievement, UserStats stats)
    {
        return achievement.ConditionType switch
        {
            "GamesPlayed" => stats.GamesPlayed >= achievement.TargetValue,
            "PlayTime" => stats.TotalSecondsPlayed >= achievement.TargetValue,
            "Sessions" => stats.SessionsCount >= achievement.TargetValue,
            "Purchases" => stats.TotalPurchases >= achievement.TargetValue,
            _ => false
        };
    }
}