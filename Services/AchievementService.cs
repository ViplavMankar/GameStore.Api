using System;
using Microsoft.EntityFrameworkCore;
using GameStore.Api.Data;
using GameStore.Api.Models;
using GameStore.Api.DTOs;

namespace GameStore.Api.Services;

public class AchievementService : IAchievementService
{
    private readonly GameStoreDbContext _context;

    public AchievementService(GameStoreDbContext context)
    {
        _context = context;
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